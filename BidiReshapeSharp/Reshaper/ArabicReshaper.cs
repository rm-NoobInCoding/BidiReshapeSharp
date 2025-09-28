using System.Text;
using System.Text.RegularExpressions;

namespace BidiReshapeSharp.Reshaper
{
    public partial class ArabicReshaper
    {

        [GeneratedRegex(@"[\u0610-\u061a\u064b-\u065f\u0670\u06d6-\u06dc\u06df-\u06e8\u06ea-\u06ed\u08d4-\u08e1\u08d4-\u08ed\u08e3-\u08ff]", RegexOptions.Compiled)]
        private static partial Regex HarakatRegex();

        private readonly ReshaperConfig _configuration;
        private readonly Dictionary<string, string[]> _letters;

        // For lazy loading the ligature regex
        private Regex _ligaturesRegex = null!;
        private Dictionary<int, string[]> _reGroupIndexToLigatureForms = null!;

        public ArabicReshaper(ReshaperConfig? configuration = null)
        {
            _configuration = configuration ?? new ReshaperConfig();

            _letters = _configuration.Language switch
            {
                "ArabicV2" => ArabicData.LettersArabicV2,
                "Kurdish" => ArabicData.LettersKurdish,
                _ => ArabicData.LettersArabic,
            };
        }

        private void EnsureLigaturesRegexLoaded()
        {
            if (_ligaturesRegex != null) return;

            var patterns = new List<string>();
            _reGroupIndexToLigatureForms = [];
            int index = 1;

            foreach (var ligatureRecord in ArabicData.AllLigatures)
            {
                if (!_configuration.GetBoolean(ligatureRecord.Name))
                {
                    continue;
                }
                _reGroupIndexToLigatureForms[index] = ligatureRecord.Forms;
                patterns.Add($"({ligatureRecord.MatchPattern})");
                index++;
            }

            _ligaturesRegex = new Regex(string.Join("|", patterns));
        }

        public string Reshape(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            var output = new List<ShapedGlyph>();
            var positionsHarakat = new Dictionary<int, List<string>>();

            bool deleteHarakat = _configuration.GetBoolean("delete_harakat");
            bool deleteTatweel = _configuration.GetBoolean("delete_tatweel");
            bool supportZwj = _configuration.GetBoolean("support_zwj");
            bool shiftHarakatPosition = _configuration.GetBoolean("shift_harakat_position");
            bool useUnshaped = _configuration.GetBoolean("use_unshaped_instead_of_isolated");

            var isolatedForm = useUnshaped ? GlyphForm.Unshaped : GlyphForm.Isolated;

            foreach (char c in text)
            {
                string letter = c.ToString();
                if (HarakatRegex().IsMatch(letter))
                {
                    if (!deleteHarakat)
                    {
                        int position = output.Count - 1;
                        if (shiftHarakatPosition)
                        {
                            position--;
                        }
                        if (!positionsHarakat.TryGetValue(position, out List<string>? value))
                        {
                            value = [];
                            positionsHarakat[position] = value;
                        }
                        if (shiftHarakatPosition)
                        {
                            value.Insert(0, letter);
                        }
                        else
                        {
                            value.Add(letter);
                        }
                    }
                }
                else if (c == ArabicData.TATWEEL && deleteTatweel) { /* Skip */ }
                else if (c == ArabicData.ZWJ && !supportZwj) { /* Skip */ }
                else if (!_letters.ContainsKey(letter))
                {
                    output.Add(new ShapedGlyph(letter, GlyphForm.NotSupported));
                }
                else if (output.Count == 0) // First letter
                {
                    output.Add(new ShapedGlyph(letter, isolatedForm));
                }
                else
                {
                    ShapedGlyph previousGlyph = output.Last();
                    if (previousGlyph.Form == GlyphForm.NotSupported ||
                        !ArabicData.ConnectsWithLetterBefore(letter, _letters) ||
                        !ArabicData.ConnectsWithLetterAfter(previousGlyph.Character, _letters) ||
                        (previousGlyph.Form == GlyphForm.Final && !ArabicData.ConnectsWithLettersBeforeAndAfter(previousGlyph.Character, _letters)))
                    {
                        output.Add(new ShapedGlyph(letter, isolatedForm));
                    }
                    else if (previousGlyph.Form == isolatedForm)
                    {
                        output[^1] = new ShapedGlyph(previousGlyph.Character, GlyphForm.Initial);
                        output.Add(new ShapedGlyph(letter, GlyphForm.Final));
                    }
                    else
                    {
                        output[^1] = new ShapedGlyph(previousGlyph.Character, GlyphForm.Medial);
                        output.Add(new ShapedGlyph(letter, GlyphForm.Final));
                    }
                }

                if (supportZwj && output.Count > 1 && output[^2].Character == ArabicData.ZWJ.ToString())
                {
                    output.RemoveAt(output.Count - 2);
                }
            }

            if (supportZwj && output.Count > 0 && output.Last().Character == ArabicData.ZWJ.ToString())
            {
                output.RemoveAt(output.Count - 1);
            }

            if (_configuration.GetBoolean("support_ligatures"))
            {
                EnsureLigaturesRegexLoaded();

                string cleanedText = HarakatRegex().Replace(text, "");
                if (deleteTatweel)
                {
                    cleanedText = cleanedText.Replace(ArabicData.TATWEEL.ToString(), "");
                }

                foreach (Match match in _ligaturesRegex.Matches(cleanedText))
                {
                    int groupIndex = -1;
                    for (int i = 1; i < match.Groups.Count; i++)
                    {
                        if (match.Groups[i].Success)
                        {
                            groupIndex = i;
                            break;
                        }
                    }
                    if (groupIndex == -1) continue;

                    string[] forms = _reGroupIndexToLigatureForms[groupIndex];
                    int start = match.Index;
                    int end = start + match.Length;

                    GlyphForm aForm = output[start].Form;
                    GlyphForm bForm = output[end - 1].Form;
                    GlyphForm? ligatureForm;

                    if (aForm == isolatedForm || aForm == GlyphForm.Initial)
                    {
                        ligatureForm = (bForm == isolatedForm || bForm == GlyphForm.Final) ? GlyphForm.Isolated : GlyphForm.Initial;
                    }
                    else
                    {
                        ligatureForm = (bForm == isolatedForm || bForm == GlyphForm.Final) ? GlyphForm.Final : GlyphForm.Medial;
                    }

                    if (ligatureForm.HasValue && forms[(int)ligatureForm.Value] != null)
                    {
                        output[start] = new ShapedGlyph(forms[(int)ligatureForm.Value], GlyphForm.NotSupported);
                        for (int i = start + 1; i < end; i++)
                        {
                            output[i] = new ShapedGlyph("", GlyphForm.NotSupported);
                        }
                    }
                }
            }

            var result = new StringBuilder();
            if (!deleteHarakat && positionsHarakat.TryGetValue(-1, out var leadingHarakat))
            {
                result.Append(string.Concat(leadingHarakat));
            }

            for (int i = 0; i < output.Count; i++)
            {
                ShapedGlyph glyph = output[i];
                if (!string.IsNullOrEmpty(glyph.Character))
                {
                    if (glyph.Form == GlyphForm.NotSupported || glyph.Form == GlyphForm.Unshaped)
                    {
                        result.Append(glyph.Character);
                    }
                    else
                    {
                        result.Append(_letters[glyph.Character][(int)glyph.Form]);
                    }
                }

                if (!deleteHarakat && positionsHarakat.TryGetValue(i, out var harakat))
                {
                    result.Append(string.Concat(harakat));
                }
            }

            return result.ToString();
        }


    }

}
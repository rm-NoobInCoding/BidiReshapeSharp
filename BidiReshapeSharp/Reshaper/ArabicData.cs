namespace BidiReshapeSharp.Reshaper
{

#pragma warning disable CS8625
#pragma warning disable CS8620

    /// <summary>
    /// Defines the positional forms for Arabic characters.
    /// The integer values match the original Python implementation.
    /// </summary>
    public enum GlyphForm
    {
        Isolated = 0,
        Initial = 1,
        Medial = 2,
        Final = 3,
        Unshaped = 255,
        NotSupported = -1
    }

    /// <summary>
    /// A struct to hold a character and its calculated contextual form.
    /// </summary>
    public struct ShapedGlyph(string character, GlyphForm form)
    {
        public string Character = character;
        public GlyphForm Form = form;
    }

    /// <summary>
    /// Represents a single ligature record, including its name for configuration,
    /// the text/regex pattern to match, and its contextual forms.
    /// </summary>
    public class LigatureRecord(string name, string matchPattern, string[] forms)
    {
        public string Name { get; } = name;
        public string MatchPattern { get; } = matchPattern;
        // Forms array order: [Isolated, Initial, Medial, Final]
        public string[] Forms { get; } = forms;
    }

    /// <summary>
    /// Contains all static data for Arabic script processing, including character forms,
    /// </summary>
    public static class ArabicData
    {
        // --- Constants ---
        public const char TATWEEL = '\u0640';
        public const char ZWJ = '\u200D';

        // --- LETTERS_ARABIC ---
        // The forms array is [Isolated, Initial, Medial, Final]
        // A null value indicates the form is not supported for that letter.
        public static readonly Dictionary<string, string[]> LettersArabic = new()
        {
            { "\u0621", new[] { "\uFE80", null, null, null } }, // HAMZA
            { "\u0622", new[] { "\uFE81", null, null, "\uFE82" } }, // ALEF WITH MADDA
            { "\u0623", new[] { "\uFE83", null, null, "\uFE84" } }, // ALEF WITH HAMZA ABOVE
            { "\u0624", new[] { "\uFE85", null, null, "\uFE86" } }, // WAW WITH HAMZA ABOVE
            { "\u0625", new[] { "\uFE87", null, null, "\uFE88" } }, // ALEF WITH HAMZA BELOW
            { "\u0626", new[] { "\uFE89", "\uFE8B", "\uFE8C", "\uFE8A" } }, // YEH WITH HAMZA ABOVE
            { "\u0627", new[] { "\uFE8D", null, null, "\uFE8E" } }, // ALEF
            { "\u0628", new[] { "\uFE8F", "\uFE91", "\uFE92", "\uFE90" } }, // BEH
            { "\u0629", new[] { "\uFE93", null, null, "\uFE94" } }, // TEH MARBUTA
            { "\u062A", new[] { "\uFE95", "\uFE97", "\uFE98", "\uFE96" } }, // TEH
            { "\u062B", new[] { "\uFE99", "\uFE9B", "\uFE9C", "\uFE9A" } }, // THEH
            { "\u062C", new[] { "\uFE9D", "\uFE9F", "\uFEA0", "\uFE9E" } }, // JEEM
            { "\u062D", new[] { "\uFEA1", "\uFEA3", "\uFEA4", "\uFEA2" } }, // HAH
            { "\u062E", new[] { "\uFEA5", "\uFEA7", "\uFEA8", "\uFEA6" } }, // KHAH
            { "\u062F", new[] { "\uFEA9", null, null, "\uFEAA" } }, // DAL
            { "\u0630", new[] { "\uFEAB", null, null, "\uFEAC" } }, // THAL
            { "\u0631", new[] { "\uFEAD", null, null, "\uFEAE" } }, // REH
            { "\u0632", new[] { "\uFEAF", null, null, "\uFEB0" } }, // ZAIN
            { "\u0633", new[] { "\uFEB1", "\uFEB3", "\uFEB4", "\uFEB2" } }, // SEEN
            { "\u0634", new[] { "\uFEB5", "\uFEB7", "\uFEB8", "\uFEB6" } }, // SHEEN
            { "\u0635", new[] { "\uFEB9", "\uFEBB", "\uFEBC", "\uFEBA" } }, // SAD
            { "\u0636", new[] { "\uFEBD", "\uFEBF", "\uFEC0", "\uFEBE" } }, // DAD
            { "\u0637", new[] { "\uFEC1", "\uFEC3", "\uFEC4", "\uFEC2" } }, // TAH
            { "\u0638", new[] { "\uFEC5", "\uFEC7", "\uFEC8", "\uFEC6" } }, // ZAH
            { "\u0639", new[] { "\uFEC9", "\uFECB", "\uFECC", "\uFECA" } }, // AIN
            { "\u063A", new[] { "\uFECD", "\uFECF", "\uFED0", "\uFECE" } }, // GHAIN
            { TATWEEL.ToString(), new[] { TATWEEL.ToString(), TATWEEL.ToString(), TATWEEL.ToString(), TATWEEL.ToString() } },
            { "\u0641", new[] { "\uFED1", "\uFED3", "\uFED4", "\uFED2" } }, // FEH
            { "\u0642", new[] { "\uFED5", "\uFED7", "\uFED8", "\uFED6" } }, // QAF
            { "\u0643", new[] { "\uFED9", "\uFEDB", "\uFEDC", "\uFEDA" } }, // KAF
            { "\u0644", new[] { "\uFEDD", "\uFEDF", "\uFEE0", "\uFEDE" } }, // LAM
            { "\u0645", new[] { "\uFEE1", "\uFEE3", "\uFEE4", "\uFEE2" } }, // MEEM
            { "\u0646", new[] { "\uFEE5", "\uFEE7", "\uFEE8", "\uFEE6" } }, // NOON
            { "\u0647", new[] { "\uFEE9", "\uFEEB", "\uFEEC", "\uFEEA" } }, // HEH
            { "\u0648", new[] { "\uFEED", null, null, "\uFEEE" } }, // WAW
            { "\u0649", new[] { "\uFEEF", "\uFBE8", "\uFBE9", "\uFEF0" } }, // ALEF MAKSURA
            { "\u064A", new[] { "\uFEF1", "\uFEF3", "\uFEF4", "\uFEF2" } }, // YEH
            { "\u0671", new[] { "\uFB50", null, null, "\uFB51" } }, // ALEF WASLA
            { "\u0677", new[] { "\uFBDD", null, null, null } }, // U WITH HAMZA ABOVE
            { "\u0679", new[] { "\uFB66", "\uFB68", "\uFB69", "\uFB67" } }, // TTEH
            { "\u067A", new[] { "\uFB5E", "\uFB60", "\uFB61", "\uFB5F" } }, // TTEHEH
            { "\u067B", new[] { "\uFB52", "\uFB54", "\uFB55", "\uFB53" } }, // BEEH
            { "\u067E", new[] { "\uFB56", "\uFB58", "\uFB59", "\uFB57" } }, // PEH
            { "\u067F", new[] { "\uFB62", "\uFB64", "\uFB65", "\uFB63" } }, // TEHEH
            { "\u0680", new[] { "\uFB5A", "\uFB5C", "\uFB5D", "\uFB5B" } }, // BEHEH
            { "\u0683", new[] { "\uFB76", "\uFB78", "\uFB79", "\uFB77" } }, // NYEH
            { "\u0684", new[] { "\uFB72", "\uFB74", "\uFB75", "\uFB73" } }, // DYEH
            { "\u0686", new[] { "\uFB7A", "\uFB7C", "\uFB7D", "\uFB7B" } }, // TCHEH
            { "\u0687", new[] { "\uFB7E", "\uFB80", "\uFB81", "\uFB7F" } }, // TCHEHEH
            { "\u0688", new[] { "\uFB88", null, null, "\uFB89" } }, // DDAL
            { "\u068C", new[] { "\uFB84", null, null, "\uFB85" } }, // DAHAL
            { "\u068D", new[] { "\uFB82", null, null, "\uFB83" } }, // DDAHAL
            { "\u068E", new[] { "\uFB86", null, null, "\uFB87" } }, // DUL
            { "\u0691", new[] { "\uFB8C", null, null, "\uFB8D" } }, // RREH
            { "\u0698", new[] { "\uFB8A", null, null, "\uFB8B" } }, // JEH
            { "\u06A4", new[] { "\uFB6A", "\uFB6C", "\uFB6D", "\uFB6B" } }, // VEH
            { "\u06A6", new[] { "\uFB6E", "\uFB70", "\uFB71", "\uFB6F" } }, // PEHEH
            { "\u06A9", new[] { "\uFB8E", "\uFB90", "\uFB91", "\uFB8F" } }, // KEHEH
            { "\u06AD", new[] { "\uFBD3", "\uFBD5", "\uFBD6", "\uFBD4" } }, // NG
            { "\u06AF", new[] { "\uFB92", "\uFB94", "\uFB95", "\uFB93" } }, // GAF
            { "\u06B1", new[] { "\uFB9A", "\uFB9C", "\uFB9D", "\uFB9B" } }, // NGOEH
            { "\u06B3", new[] { "\uFB96", "\uFB98", "\uFB99", "\uFB97" } }, // GUEH
            { "\u06BA", new[] { "\uFB9E", null, null, "\uFB9F" } }, // NOON GHUNNA
            { "\u06BB", new[] { "\uFBA0", "\uFBA2", "\uFBA3", "\uFBA1" } }, // RNOON
            { "\u06BE", new[] { "\uFBAA", "\uFBAC", "\uFBAD", "\uFBAB" } }, // HEH DOACHASHMEE
            { "\u06C0", new[] { "\uFBA4", null, null, "\uFBA5" } }, // HEH WITH YEH ABOVE
            { "\u06C1", new[] { "\uFBA6", "\uFBA8", "\uFBA9", "\uFBA7" } }, // HEH GOAL
            { "\u06C5", new[] { "\uFBE0", null, null, "\uFBE1" } }, // KIRGHIZ OE
            { "\u06C6", new[] { "\uFBD9", null, null, "\uFBDA" } }, // OE
            { "\u06C7", new[] { "\uFBD7", null, null, "\uFBD8" } }, // U
            { "\u06C8", new[] { "\uFBDB", null, null, "\uFBDC" } }, // YU
            { "\u06C9", new[] { "\uFBE2", null, null, "\uFBE3" } }, // KIRGHIZ YU
            { "\u06CB", new[] { "\uFBDE", null, null, "\uFBDF" } }, // VE
            { "\u06CC", new[] { "\uFBFC", "\uFBFE", "\uFBFF", "\uFBFD" } }, // FARSI YEH
            { "\u06D0", new[] { "\uFBE4", "\uFBE6", "\uFBE7", "\uFBE5" } }, // E
            { "\u06D2", new[] { "\uFBAE", null, null, "\uFBAF" } }, // YEH BARREE
            { "\u06D3", new[] { "\uFBB0", null, null, "\uFBB1" } }, // YEH BARREE WITH HAMZA ABOVE
            { ZWJ.ToString(), new[] { ZWJ.ToString(), ZWJ.ToString(), ZWJ.ToString(), ZWJ.ToString() } },
        };

        public static readonly Dictionary<string, string[]> LettersArabicV2 = new()
        {
            // ARABIC LETTER HAMZA
            {"\u0621", new[] {"\uFE80", "", "", ""} },
            // ARABIC LETTER ALEF WITH MADDA ABOVE
            {"\u0622", new[] {"\u0622", "", "", "\uFE82"} },
            // ARABIC LETTER ALEF WITH HAMZA ABOVE
            {"\u0623", new[] {"\u0623", "", "", "\uFE84"} },
            // ARABIC LETTER WAW WITH HAMZA ABOVE
            {"\u0624", new[] {"\u0624", "", "", "\uFE86"} },
            // ARABIC LETTER ALEF WITH HAMZA BELOW
            {"\u0625", new[] {"\u0625", "", "", "\uFE88"} },
            // ARABIC LETTER YEH WITH HAMZA ABOVE
            {"\u0626", new[] {"\u0626", "\uFE8B", "\uFE8C", "\uFE8A"} },
            // ARABIC LETTER ALEF
            {"\u0627", new[] {"\u0627", "", "", "\uFE8E"} },
            // ARABIC LETTER BEH
            {"\u0628", new[] {"\u0628", "\uFE91", "\uFE92", "\uFE90"} },
            // ARABIC LETTER TEH MARBUTA
            {"\u0629", new[] {"\u0629", "", "", "\uFE94"} },
            // ARABIC LETTER TEH
            {"\u062A", new[] {"\u062A", "\uFE97", "\uFE98", "\uFE96"} },
            // ARABIC LETTER THEH
            {"\u062B", new[] {"\u062B", "\uFE9B", "\uFE9C", "\uFE9A"} },
            // ARABIC LETTER JEEM
            {"\u062C", new[] {"\u062C", "\uFE9F", "\uFEA0", "\uFE9E"} },
            // ARABIC LETTER HAH
            {"\u062D", new[] {"\uFEA1", "\uFEA3", "\uFEA4", "\uFEA2"} },
            // ARABIC LETTER KHAH
            {"\u062E", new[] {"\u062E", "\uFEA7", "\uFEA8", "\uFEA6"} },
            // ARABIC LETTER DAL
            {"\u062F", new[] {"\u062F", "", "", "\uFEAA"} },
            // ARABIC LETTER THAL
            {"\u0630", new[] {"\u0630", "", "", "\uFEAC"} },
            // ARABIC LETTER REH
            {"\u0631", new[] {"\u0631", "", "", "\uFEAE"} },
            // ARABIC LETTER ZAIN
            {"\u0632", new[] {"\u0632", "", "", "\uFEB0"} },
            // ARABIC LETTER SEEN
            {"\u0633", new[] {"\u0633", "\uFEB3", "\uFEB4", "\uFEB2"} },
            // ARABIC LETTER SHEEN
            {"\u0634", new[] {"\u0634", "\uFEB7", "\uFEB8", "\uFEB6"} },
            // ARABIC LETTER SAD
            {"\u0635", new[] {"\u0635", "\uFEBB", "\uFEBC", "\uFEBA"} },
            // ARABIC LETTER DAD
            {"\u0636", new[] {"\u0636", "\uFEBF", "\uFEC0", "\uFEBE"} },
            // ARABIC LETTER TAH
            {"\u0637", new[] {"\u0637", "\uFEC3", "\uFEC4", "\uFEC2"} },
            // ARABIC LETTER ZAH
            {"\u0638", new[] {"\u0638", "\uFEC7", "\uFEC8", "\uFEC6"} },
            // ARABIC LETTER AIN
            {"\u0639", new[] {"\u0639", "\uFECB", "\uFECC", "\uFECA"} },
            // ARABIC LETTER GHAIN
            {"\u063A", new[] {"\u063A", "\uFECF", "\uFED0", "\uFECE"} },
            // ARABIC TATWEEL
            {TATWEEL.ToString(), new[] { TATWEEL.ToString(),   TATWEEL.ToString(),  TATWEEL.ToString(),  TATWEEL.ToString() } },
            // ARABIC LETTER FEH
            { "\u0641", new[] { "\u0641", "\uFED3", "\uFED4", "\uFED2" } },
            // ARABIC LETTER QAF
            { "\u0642", new[] { "\u0642", "\uFED7", "\uFED8", "\uFED6" } },
            // ARABIC LETTER KAF
            { "\u0643", new[] { "\u0643", "\uFEDB", "\uFEDC", "\uFEDA" } },
            // ARABIC LETTER LAM
            { "\u0644", new[] { "\u0644", "\uFEDF", "\uFEE0", "\uFEDE" } },
            // ARABIC LETTER MEEM
            { "\u0645", new[] { "\u0645", "\uFEE3", "\uFEE4", "\uFEE2" } },
            // ARABIC LETTER NOON
            { "\u0646", new[] { "\u0646", "\uFEE7", "\uFEE8", "\uFEE6" } },
            // ARABIC LETTER HEH
            { "\u0647", new[] { "\u0647", "\uFEEB", "\uFEEC", "\uFEEA" } },
            // ARABIC LETTER WAW
            { "\u0648", new[] { "\u0648", "", "", "\uFEEE" } },
            // ARABIC LETTER (UIGHUR KAZAKH KIRGHIZ)? ALEF MAKSURA
            { "\u0649", new[] { "\u0649", "\uFBE8", "\uFBE9", "\uFEF0" } },
            // ARABIC LETTER YEH
            { "\u064A", new[] { "\u064A", "\uFEF3", "\uFEF4", "\uFEF2" } },
            // ARABIC LETTER ALEF WASLA
            { "\u0671", new[] { "\u0671", "", "", "\uFB51" } },
            // ARABIC LETTER U WITH HAMZA ABOVE
            { "\u0677", new[] { "\u0677", "", "", "" } },
            // ARABIC LETTER TTEH
            { "\u0679", new[] { "\u0679", "\uFB68", "\uFB69", "\uFB67" } },
            // ARABIC LETTER TTEHEH
            { "\u067A", new[] { "\u067A", "\uFB60", "\uFB61", "\uFB5F" } },
            // ARABIC LETTER BEEH
            { "\u067B", new[] { "\u067B", "\uFB54", "\uFB55", "\uFB53" } },
            // ARABIC LETTER PEH
            { "\u067E", new[] { "\u067E", "\uFB58", "\uFB59", "\uFB57" } },
            // ARABIC LETTER TEHEH
            { "\u067F", new[] { "\u067F", "\uFB64", "\uFB65", "\uFB63" } },
            // ARABIC LETTER BEHEH
            { "\u0680", new[] { "\u0680", "\uFB5C", "\uFB5D", "\uFB5B" } },
            // ARABIC LETTER NYEH
            { "\u0683", new[] { "\u0683", "\uFB78", "\uFB79", "\uFB77" } },
            // ARABIC LETTER DYEH
            { "\u0684", new[] { "\u0684", "\uFB74", "\uFB75", "\uFB73" } },
            // ARABIC LETTER TCHEH
            { "\u0686", new[] { "\u0686", "\uFB7C", "\uFB7D", "\uFB7B" } },
            // ARABIC LETTER TCHEHEH
            { "\u0687", new[] { "\u0687", "\uFB80", "\uFB81", "\uFB7F" } },
            // ARABIC LETTER DDAL
            { "\u0688", new[] { "\u0688", "", "", "\uFB89" } },
            // ARABIC LETTER DAHAL
            { "\u068C", new[] { "\u068C", "", "", "\uFB85" } },
            // ARABIC LETTER DDAHAL
            { "\u068D", new[] { "\u068D", "", "", "\uFB83" } },
            // ARABIC LETTER DUL
            { "\u068E", new[] { "\u068E", "", "", "\uFB87" } },
            // ARABIC LETTER RREH
            { "\u0691", new[] { "\u0691", "", "", "\uFB8D" } },
            // ARABIC LETTER JEH
            { "\u0698", new[] { "\u0698", "", "", "\uFB8B" } },
            // ARABIC LETTER VEH
            { "\u06A4", new[] { "\u06A4", "\uFB6C", "\uFB6D", "\uFB6B" } },
            // ARABIC LETTER PEHEH
            { "\u06A6", new[] { "\u06A6", "\uFB70", "\uFB71", "\uFB6F" } },
            // ARABIC LETTER KEHEH
            { "\u06A9", new[] { "\u06A9", "\uFB90", "\uFB91", "\uFB8F" } },
            // ARABIC LETTER NG
            { "\u06AD", new[] { "\u06AD", "\uFBD5", "\uFBD6", "\uFBD4" } },
            // ARABIC LETTER GAF
            { "\u06AF", new[] { "\u06AF", "\uFB94", "\uFB95", "\uFB93" } },
            // ARABIC LETTER NGOEH
            { "\u06B1", new[] { "\u06B1", "\uFB9C", "\uFB9D", "\uFB9B" } },
            // ARABIC LETTER GUEH
            { "\u06B3", new[] { "\u06B3", "\uFB98", "\uFB99", "\uFB97" } },
            // ARABIC LETTER NOON GHUNNA
            { "\u06BA", new[] { "\u06BA", "", "", "\uFB9F" } },
            // ARABIC LETTER RNOON
            { "\u06BB", new[] { "\u06BB", "\uFBA2", "\uFBA3", "\uFBA1" } },
            // ARABIC LETTER HEH DOACHASHMEE
            { "\u06BE", new[] { "\u06BE", "\uFBAC", "\uFBAD", "\uFBAB" } },
            // ARABIC LETTER HEH WITH YEH ABOVE
            { "\u06C0", new[] { "\u06C0", "", "", "\uFBA5" } },
            // ARABIC LETTER HEH GOAL
            { "\u06C1", new[] { "\u06C1", "\uFBA8", "\uFBA9", "\uFBA7" } },
            // ARABIC LETTER KIRGHIZ OE
            { "\u06C5", new[] { "\u06C5", "", "", "\uFBE1" } },
            // ARABIC LETTER OE
            { "\u06C6", new[] { "\u06C6", "", "", "\uFBDA" } },
            // ARABIC LETTER U
            { "\u06C7", new[] { "\u06C7", "", "", "\uFBD8" } },
            // ARABIC LETTER YU
            { "\u06C8", new[] { "\u06C8", "", "", "\uFBDC" } },
            // ARABIC LETTER KIRGHIZ YU
            { "\u06C9", new[] { "\u06C9", "", "", "\uFBE3" } },
            // ARABIC LETTER VE
            { "\u06CB", new[] { "\u06CB", "", "", "\uFBDF" } },
            // ARABIC LETTER FARSI YEH
            { "\u06CC", new[] { "\u06CC", "\uFBFE", "\uFBFF", "\uFBFD" } },
            // ARABIC LETTER E
            { "\u06D0", new[] { "\u06D0", "\uFBE6", "\uFBE7", "\uFBE5" } },
            // ARABIC LETTER YEH BARREE
            { "\u06D2", new[] { "\u06D2", "", "", "\uFBAF" } },
            // ARABIC LETTER YEH BARREE WITH HAMZA ABOVE
            { "\u06D3", new[] { "\u06D3", "", "", "\uFBB1" } },
            // Kurdish letter YEAH
            { "\u06ce", new[] { "\uE004", "\uE005", "\uE006", "\uE004" } },
            // Kurdish letter Hamza same as arabic Teh without the point
            { "\u06d5", new[] { "\u06d5", "", "", "\uE000" } },
            // ZWJ
            { ZWJ.ToString(), new[] { ZWJ.ToString(), ZWJ.ToString(), ZWJ.ToString(), ZWJ.ToString() } },
        };

        public static readonly Dictionary<string, string[]> LettersKurdish = new()
        {
            // ARABIC LETTER HAMZA
            {"\u0621", new[] {"\uFE80", "", "", ""} },
            // ARABIC LETTER ALEF WITH MADDA ABOVE
            {"\u0622", new[] {"\u0622", "", "", "\uFE82"} },
            // ARABIC LETTER ALEF WITH HAMZA ABOVE
            { "\u0623", new[] { "\u0623", "", "", "\uFE84" } },
            // ARABIC LETTER WAW WITH HAMZA ABOVE
            { "\u0624", new[] { "\u0624", "", "", "\uFE86" } },
            // ARABIC LETTER ALEF WITH HAMZA BELOW
            { "\u0625", new[] { "\u0625", "", "", "\uFE88" } },
            // ARABIC LETTER YEH WITH HAMZA ABOVE
            { "\u0626", new[] { "\u0626", "\uFE8B", "\uFE8C", "\uFE8A" } },
            // ARABIC LETTER ALEF
            { "\u0627", new[] { "\u0627", "", "", "\uFE8E" } },
            // ARABIC LETTER BEH
            { "\u0628", new[] { "\u0628", "\uFE91", "\uFE92", "\uFE90" } },
            // ARABIC LETTER TEH MARBUTA
            { "\u0629", new[] { "\u0629", "", "", "\uFE94" } },
            // ARABIC LETTER TEH
            { "\u062A", new[] { "\u062A", "\uFE97", "\uFE98", "\uFE96" } },
            // ARABIC LETTER THEH
            { "\u062B", new[] { "\u062B", "\uFE9B", "\uFE9C", "\uFE9A" } },
            // ARABIC LETTER JEEM
            { "\u062C", new[] { "\u062C", "\uFE9F", "\uFEA0", "\uFE9E" } },
            // ARABIC LETTER HAH
            { "\u062D", new[] { "\uFEA1", "\uFEA3", "\uFEA4", "\uFEA2" } },
            // ARABIC LETTER KHAH
            { "\u062E", new[] { "\u062E", "\uFEA7", "\uFEA8", "\uFEA6" } },
            // ARABIC LETTER DAL
            { "\u062F", new[] { "\u062F", "", "", "\uFEAA" } },
            // ARABIC LETTER THAL
            { "\u0630", new[] { "\u0630", "", "", "\uFEAC" } },
            // ARABIC LETTER REH
            { "\u0631", new[] { "\u0631", "", "", "\uFEAE" } },
            // ARABIC LETTER ZAIN
            { "\u0632", new[] { "\u0632", "", "", "\uFEB0" } },
            // ARABIC LETTER SEEN
            { "\u0633", new[] { "\u0633", "\uFEB3", "\uFEB4", "\uFEB2" } },
            // ARABIC LETTER SHEEN
            { "\u0634", new[] { "\u0634", "\uFEB7", "\uFEB8", "\uFEB6" } },
            // ARABIC LETTER SAD
            { "\u0635", new[] { "\u0635", "\uFEBB", "\uFEBC", "\uFEBA" } },
            // ARABIC LETTER DAD
            { "\u0636", new[] { "\u0636", "\uFEBF", "\uFEC0", "\uFEBE" } },
            // ARABIC LETTER TAH
            { "\u0637", new[] { "\u0637", "\uFEC3", "\uFEC4", "\uFEC2" } },
            // ARABIC LETTER ZAH
            { "\u0638", new[] { "\u0638", "\uFEC7", "\uFEC8", "\uFEC6" } },
            // ARABIC LETTER AIN
            { "\u0639", new[] { "\u0639", "\uFECB", "\uFECC", "\uFECA" } },
            // ARABIC LETTER GHAIN
            { "\u063A", new[] { "\u063A", "\uFECF", "\uFED0", "\uFECE" } },
            // ARABIC TATWEEL
            {TATWEEL.ToString(), new[] { TATWEEL.ToString(),   TATWEEL.ToString(),  TATWEEL.ToString(),  TATWEEL.ToString() } },
            // ARABIC LETTER FEH
            { "\u0641", new[] { "\u0641", "\uFED3", "\uFED4", "\uFED2" } },
            // ARABIC LETTER QAF
            { "\u0642", new[] { "\u0642", "\uFED7", "\uFED8", "\uFED6" } },
            // ARABIC LETTER KAF
            { "\u0643", new[] { "\u0643", "\uFEDB", "\uFEDC", "\uFEDA" } },
            // ARABIC LETTER LAM
            { "\u0644", new[] { "\u0644", "\uFEDF", "\uFEE0", "\uFEDE" } },
            // ARABIC LETTER MEEM
            { "\u0645", new[] { "\u0645", "\uFEE3", "\uFEE4", "\uFEE2" } },
            // ARABIC LETTER NOON
            { "\u0646", new[] { "\u0646", "\uFEE7", "\uFEE8", "\uFEE6" } },
            // ARABIC LETTER HEH
            { "\u0647", new[] { "\uFBAB", "\uFBAB", "\uFBAB", "\uFBAB" } },
            // ARABIC LETTER WAW
            { "\u0648", new[] { "\u0648", "", "", "\uFEEE" } },
            // ARABIC LETTER (UIGHUR KAZAKH KIRGHIZ)? ALEF MAKSURA
            { "\u0649", new[] { "\u0649", "\uFBE8", "\uFBE9", "\uFEF0" } },
            // ARABIC LETTER YEH
            { "\u064A", new[] { "\u064A", "\uFEF3", "\uFEF4", "\uFEF2" } },
            // ARABIC LETTER ALEF WASLA
            { "\u0671", new[] { "\u0671", "", "", "\uFB51" } },
            // ARABIC LETTER U WITH HAMZA ABOVE
            { "\u0677", new[] { "\u0677", "", "", "" } },
            // ARABIC LETTER TTEH
            { "\u0679", new[] { "\u0679", "\uFB68", "\uFB69", "\uFB67" } },
            // ARABIC LETTER TTEHEH
            { "\u067A", new[] { "\u067A", "\uFB60", "\uFB61", "\uFB5F" } },
            // ARABIC LETTER BEEH
            { "\u067B", new[] { "\u067B", "\uFB54", "\uFB55", "\uFB53" } },
            // ARABIC LETTER PEH
            { "\u067E", new[] { "\u067E", "\uFB58", "\uFB59", "\uFB57" } },
            // ARABIC LETTER TEHEH
            { "\u067F", new[] { "\u067F", "\uFB64", "\uFB65", "\uFB63" } },
            // ARABIC LETTER BEHEH
            { "\u0680", new[] { "\u0680", "\uFB5C", "\uFB5D", "\uFB5B" } },
            // ARABIC LETTER NYEH
            { "\u0683", new[] { "\u0683", "\uFB78", "\uFB79", "\uFB77" } },
            // ARABIC LETTER DYEH
            { "\u0684", new[] { "\u0684", "\uFB74", "\uFB75", "\uFB73" } },
            // ARABIC LETTER TCHEH
            { "\u0686", new[] { "\u0686", "\uFB7C", "\uFB7D", "\uFB7B" } },
            // ARABIC LETTER TCHEHEH
            { "\u0687", new[] { "\u0687", "\uFB80", "\uFB81", "\uFB7F" } },
            // ARABIC LETTER DDAL
            { "\u0688", new[] { "\u0688", "", "", "\uFB89" } },
            // ARABIC LETTER DAHAL
            { "\u068C", new[] { "\u068C", "", "", "\uFB85" } },
            // ARABIC LETTER DDAHAL
            { "\u068D", new[] { "\u068D", "", "", "\uFB83" } },
            // ARABIC LETTER DUL
            { "\u068E", new[] { "\u068E", "", "", "\uFB87" } },
            // ARABIC LETTER RREH
            { "\u0691", new[] { "\u0691", "", "", "\uFB8D" } },
            // ARABIC LETTER JEH
            { "\u0698", new[] { "\u0698", "", "", "\uFB8B" } },
            // ARABIC LETTER VEH
            { "\u06A4", new[] { "\u06A4", "\uFB6C", "\uFB6D", "\uFB6B" } },
            // ARABIC LETTER PEHEH
            { "\u06A6", new[] { "\u06A6", "\uFB70", "\uFB71", "\uFB6F" } },
            // ARABIC LETTER KEHEH
            { "\u06A9", new[] { "\u06A9", "\uFB90", "\uFB91", "\uFB8F" } },
            // ARABIC LETTER NG
            { "\u06AD", new[] { "\u06AD", "\uFBD5", "\uFBD6", "\uFBD4" } },
            // ARABIC LETTER GAF
            { "\u06AF", new[] { "\u06AF", "\uFB94", "\uFB95", "\uFB93" } },
            // ARABIC LETTER NGOEH
            { "\u06B1", new[] { "\u06B1", "\uFB9C", "\uFB9D", "\uFB9B" } },
            // ARABIC LETTER GUEH
            { "\u06B3", new[] { "\u06B3", "\uFB98", "\uFB99", "\uFB97" } },
            // ARABIC LETTER NOON GHUNNA
            { "\u06BA", new[] { "\u06BA", "", "", "\uFB9F" } },
            // ARABIC LETTER RNOON
            { "\u06BB", new[] { "\u06BB", "\uFBA2", "\uFBA3", "\uFBA1" } },
            // ARABIC LETTER HEH DOACHASHMEE
            { "\u06BE", new[] { "\u06BE", "\uFBAC", "\uFBAD", "\uFBAB" } },
            // ARABIC LETTER HEH WITH YEH ABOVE
            { "\u06C0", new[] { "\u06C0", "", "", "\uFBA5" } },
            // ARABIC LETTER HEH GOAL
            { "\u06C1", new[] { "\u06C1", "\uFBA8", "\uFBA9", "\uFBA7" } },
            // ARABIC LETTER KIRGHIZ OE
            { "\u06C5", new[] { "\u06C5", "", "", "\uFBE1" } },
            // ARABIC LETTER OE
            { "\u06C6", new[] { "\u06C6", "", "", "\uFBDA" } },
            // ARABIC LETTER U
            { "\u06C7", new[] { "\u06C7", "", "", "\uFBD8" } },
            // ARABIC LETTER YU
            { "\u06C8", new[] { "\u06C8", "", "", "\uFBDC" } },
            // ARABIC LETTER KIRGHIZ YU
            { "\u06C9", new[] { "\u06C9", "", "", "\uFBE3" } },
            // ARABIC LETTER VE
            { "\u06CB", new[] { "\u06CB", "", "", "\uFBDF" } },
            // ARABIC LETTER FARSI YEH
            { "\u06CC", new[] { "\u06CC", "\uFBFE", "\uFBFF", "\uFBFD" } },
            // ARABIC LETTER E
            { "\u06D0", new[] { "\u06D0", "\uFBE6", "\uFBE7", "\uFBE5" } },
            // ARABIC LETTER YEH BARREE
            { "\u06D2", new[] { "\u06D2", "", "", "\uFBAF" } },
            // ARABIC LETTER YEH BARREE WITH HAMZA ABOVE
            { "\u06D3", new[] { "\u06D3", "", "", "\uFBB1" } },
            // Kurdish letter YEAH
            { "\u06ce", new[] { "\uE004", "\uE005", "\uE006", "\uE004" } },
            // Kurdish letter Hamza same as arabic Teh without the point
            { "\u06d5", new[] { "\u06d5", "", "", "\uE000" } },
            // ZWJ
            { ZWJ.ToString(), new[] { ZWJ.ToString(), ZWJ.ToString(), ZWJ.ToString(), ZWJ.ToString() } },
        };

        // --- Ligature Data ---
        private static readonly List<LigatureRecord> SentencesLigatures = [

            new LigatureRecord("BISMILLAH AR-RAHMAN AR-RAHEEM", "\u0628\u0633\u0645\u0020\u0627\u0644\u0644\u0647\u0020\u0627\u0644\u0631\u062D\u0645\u0646\u0020\u0627\u0644\u0631\u062D\u064A\u0645", ["\uFDFD", null, null, null]),
            new LigatureRecord("JALLAJALALOUHOU", "\u062C\u0644\u0020\u062C\u0644\u0627\u0644\u0647", ["\uFDFB", null, null, null]),
            new LigatureRecord("SALLALLAHOU ALAYHE WASALLAM", "\u0635\u0644\u0649\u0020\u0627\u0644\u0644\u0647\u0020\u0639\u064A\u0647\u0020\u0648\u0633\u0644\u0645", ["\uFDFA", null, null, null]),
        ];
        
        private static readonly List<LigatureRecord> WordsLigatures = [
            new LigatureRecord("ALLAH", "\u0627\u0644\u0644\u0647", ["\uFDF2", null, null, null]),
            new LigatureRecord("AKBAR", "\u0623\u0643\u0628\u0631", ["\uFDF3", null, null, null]),
            new LigatureRecord("ALAYHE", "\u0639\u064A\u0647", ["\uFDF7", null, null, null]),
            new LigatureRecord("MOHAMMAD", "\u0645\u062D\u0645\u062F", ["\uFDF4", null, null, null]),
            new LigatureRecord("RASOUL", "\u0631\u0633\u0648\u0644", ["\uFDF6", null, null, null]),
            new LigatureRecord("SALAM", "\u0635\u0644\u0639\u0645", ["\uFDF5", null, null, null]),
            new LigatureRecord("SALLA", "\u0635\u0644\u0649", ["\uFDF9", null, null, null]),
            new LigatureRecord("WASALLAM", "\u0648\u0633\u0644\u0645", ["\uFDF8", null, null, null]),
            new LigatureRecord("RIAL SIGN", "\u0631[\u06CC\u064A]\u0627\u0644", ["\uFDFC", null, null, null]),
        ];
        
        private static readonly List<LigatureRecord> LettersLigatures = [
            new LigatureRecord("ARABIC LIGATURE AIN WITH ALEF MAKSURA", "\u0639\u0649", ["\uFCF7", "", "", "\uFD13"]),
            new LigatureRecord("ARABIC LIGATURE AIN WITH JEEM", "\u0639\u062C", ["\uFC29", "\uFCBA", "", ""]),
            new LigatureRecord("ARABIC LIGATURE AIN WITH JEEM WITH MEEM", "\u0639\u062C\u0645", ["", "\uFDC4", "", "\uFD75"]),
            new LigatureRecord("ARABIC LIGATURE AIN WITH MEEM", "\u0639\u0645", ["\uFC2A", "\uFCBB", "", ""]),
            new LigatureRecord("ARABIC LIGATURE AIN WITH MEEM WITH ALEF MAKSURA", "\u0639\u0645\u0649", ["", "", "", "\uFD78"]),
            new LigatureRecord("ARABIC LIGATURE AIN WITH MEEM WITH MEEM", "\u0639\u0645\u0645", ["", "\uFD77", "", "\uFD76"]),
            new LigatureRecord("ARABIC LIGATURE AIN WITH MEEM WITH YEH", "\u0639\u0645\u064A", ["", "", "", "\uFDB6"]),
            new LigatureRecord("ARABIC LIGATURE AIN WITH YEH", "\u0639\u064A", ["\uFCF8", "", "", "\uFD14"]),
            new LigatureRecord("ARABIC LIGATURE ALEF MAKSURA WITH SUPERSCRIPT ALEF", "\u0649\u0670", ["\uFC5D", "", "", "\uFC90"]),
            new LigatureRecord("ARABIC LIGATURE ALEF WITH FATHATAN", "\u0627\u064B", ["\uFD3D", "", "", "\uFD3C"]),
            new LigatureRecord("ARABIC LIGATURE BEH WITH ALEF MAKSURA", "\u0628\u0649", ["\uFC09", "", "", "\uFC6E"]),
            new LigatureRecord("ARABIC LIGATURE BEH WITH HAH", "\u0628\u062D", ["\uFC06", "\uFC9D", "", ""]),
            new LigatureRecord("ARABIC LIGATURE BEH WITH HAH WITH YEH", "\u0628\u062D\u064A", ["", "", "", "\uFDC2"]),
            new LigatureRecord("ARABIC LIGATURE BEH WITH HEH", "\u0628\u0647", ["", "\uFCA0", "\uFCE2", ""]),
            new LigatureRecord("ARABIC LIGATURE BEH WITH JEEM", "\u0628\u062C", ["\uFC05", "\uFC9C", "", ""]),
            new LigatureRecord("ARABIC LIGATURE BEH WITH KHAH", "\u0628\u062E", ["\uFC07", "\uFC9E", "", ""]),
            new LigatureRecord("ARABIC LIGATURE BEH WITH KHAH WITH YEH", "\u0628\u062E\u064A", ["", "", "", "\uFD9E"]),
            new LigatureRecord("ARABIC LIGATURE BEH WITH MEEM", "\u0628\u0645", ["\uFC08", "\uFC9F", "\uFCE1", "\uFC6C"]),
            new LigatureRecord("ARABIC LIGATURE BEH WITH NOON", "\u0628\u0646", ["", "", "", "\uFC6D"]),
            new LigatureRecord("ARABIC LIGATURE BEH WITH REH", "\u0628\u0631", ["", "", "", "\uFC6A"]),
            new LigatureRecord("ARABIC LIGATURE BEH WITH YEH", "\u0628\u064A", ["\uFC0A", "", "", "\uFC6F"]),
            new LigatureRecord("ARABIC LIGATURE BEH WITH ZAIN", "\u0628\u0632", ["", "", "", "\uFC6B"]),
            new LigatureRecord("ARABIC LIGATURE DAD WITH ALEF MAKSURA", "\u0636\u0649", ["\uFD07", "", "", "\uFD23"]),
            new LigatureRecord("ARABIC LIGATURE DAD WITH HAH", "\u0636\u062D", ["\uFC23", "\uFCB5", "", ""]),
            new LigatureRecord("ARABIC LIGATURE DAD WITH HAH WITH ALEF MAKSURA", "\u0636\u062D\u0649", ["", "", "", "\uFD6E"]),
            new LigatureRecord("ARABIC LIGATURE DAD WITH HAH WITH YEH", "\u0636\u062D\u064A", ["", "", "", "\uFDAB"]),
            new LigatureRecord("ARABIC LIGATURE DAD WITH JEEM", "\u0636\u062C", ["\uFC22", "\uFCB4", "", ""]),
            new LigatureRecord("ARABIC LIGATURE DAD WITH KHAH", "\u0636\u062E", ["\uFC24", "\uFCB6", "", ""]),
            new LigatureRecord("ARABIC LIGATURE DAD WITH KHAH WITH MEEM", "\u0636\u062E\u0645", ["", "\uFD70", "", "\uFD6F"]),
            new LigatureRecord("ARABIC LIGATURE DAD WITH MEEM", "\u0636\u0645", ["\uFC25", "\uFCB7", "", ""]),
            new LigatureRecord("ARABIC LIGATURE DAD WITH REH", "\u0636\u0631", ["\uFD10", "", "", "\uFD2C"]),
            new LigatureRecord("ARABIC LIGATURE DAD WITH YEH", "\u0636\u064A", ["\uFD08", "", "", "\uFD24"]),
            new LigatureRecord("ARABIC LIGATURE FEH WITH ALEF MAKSURA", "\u0641\u0649", ["\uFC31", "", "", "\uFC7C"]),
            new LigatureRecord("ARABIC LIGATURE FEH WITH HAH", "\u0641\u062D", ["\uFC2E", "\uFCBF", "", ""]),
            new LigatureRecord("ARABIC LIGATURE FEH WITH JEEM", "\u0641\u062C", ["\uFC2D", "\uFCBE", "", ""]),
            new LigatureRecord("ARABIC LIGATURE FEH WITH KHAH", "\u0641\u062E", ["\uFC2F", "\uFCC0", "", ""]),
            new LigatureRecord("ARABIC LIGATURE FEH WITH KHAH WITH MEEM", "\u0641\u062E\u0645", ["", "\uFD7D", "", "\uFD7C"]),
            new LigatureRecord("ARABIC LIGATURE FEH WITH MEEM", "\u0641\u0645", ["\uFC30", "\uFCC1", "", ""]),
            new LigatureRecord("ARABIC LIGATURE FEH WITH MEEM WITH YEH", "\u0641\u0645\u064A", ["", "", "", "\uFDC1"]),
            new LigatureRecord("ARABIC LIGATURE FEH WITH YEH", "\u0641\u064A", ["\uFC32", "", "", "\uFC7D"]),
            new LigatureRecord("ARABIC LIGATURE GHAIN WITH ALEF MAKSURA", "\u063A\u0649", ["\uFCF9", "", "", "\uFD15"]),
            new LigatureRecord("ARABIC LIGATURE GHAIN WITH JEEM", "\u063A\u062C", ["\uFC2B", "\uFCBC", "", ""]),
            new LigatureRecord("ARABIC LIGATURE GHAIN WITH MEEM", "\u063A\u0645", ["\uFC2C", "\uFCBD", "", ""]),
            new LigatureRecord("ARABIC LIGATURE GHAIN WITH MEEM WITH ALEF MAKSURA", "\u063A\u0645\u0649", ["", "", "", "\uFD7B"]),
            new LigatureRecord("ARABIC LIGATURE GHAIN WITH MEEM WITH MEEM", "\u063A\u0645\u0645", ["", "", "", "\uFD79"]),
            new LigatureRecord("ARABIC LIGATURE GHAIN WITH MEEM WITH YEH", "\u063A\u0645\u064A", ["", "", "", "\uFD7A"]),
            new LigatureRecord("ARABIC LIGATURE GHAIN WITH YEH", "\u063A\u064A", ["\uFCFA", "", "", "\uFD16"]),
            new LigatureRecord("ARABIC LIGATURE HAH WITH ALEF MAKSURA", "\u062D\u0649", ["\uFCFF", "", "", "\uFD1B"]),
            new LigatureRecord("ARABIC LIGATURE HAH WITH JEEM", "\u062D\u062C", ["\uFC17", "\uFCA9", "", ""]),
            new LigatureRecord("ARABIC LIGATURE HAH WITH JEEM WITH YEH", "\u062D\u062C\u064A", ["", "", "", "\uFDBF"]),
            new LigatureRecord("ARABIC LIGATURE HAH WITH MEEM", "\u062D\u0645", ["\uFC18", "\uFCAA", "", ""]),
            new LigatureRecord("ARABIC LIGATURE HAH WITH MEEM WITH ALEF MAKSURA", "\u062D\u0645\u0649", ["", "", "", "\uFD5B"]),
            new LigatureRecord("ARABIC LIGATURE HAH WITH MEEM WITH YEH", "\u062D\u0645\u064A", ["", "", "", "\uFD5A"]),
            new LigatureRecord("ARABIC LIGATURE HAH WITH YEH", "\u062D\u064A", ["\uFD00", "", "", "\uFD1C"]),
            new LigatureRecord("ARABIC LIGATURE HEH WITH ALEF MAKSURA", "\u0647\u0649", ["\uFC53", "", "", ""]),
            new LigatureRecord("ARABIC LIGATURE HEH WITH JEEM", "\u0647\u062C", ["\uFC51", "\uFCD7", "", ""]),
            new LigatureRecord("ARABIC LIGATURE HEH WITH MEEM", "\u0647\u0645", ["\uFC52", "\uFCD8", "", ""]),
            new LigatureRecord("ARABIC LIGATURE HEH WITH MEEM WITH JEEM", "\u0647\u0645\u062C", ["", "\uFD93", "", ""]),
            new LigatureRecord("ARABIC LIGATURE HEH WITH MEEM WITH MEEM", "\u0647\u0645\u0645", ["", "\uFD94", "", ""]),
            new LigatureRecord("ARABIC LIGATURE HEH WITH SUPERSCRIPT ALEF", "\u0647\u0670", ["", "\uFCD9", "", ""]),
            new LigatureRecord("ARABIC LIGATURE HEH WITH YEH", "\u0647\u064A", ["\uFC54", "", "", ""]),
            new LigatureRecord("ARABIC LIGATURE JEEM WITH ALEF MAKSURA", "\u062C\u0649", ["\uFD01", "", "", "\uFD1D"]),
            new LigatureRecord("ARABIC LIGATURE JEEM WITH HAH", "\u062C\u062D", ["\uFC15", "\uFCA7", "", ""]),
            new LigatureRecord("ARABIC LIGATURE JEEM WITH HAH WITH ALEF MAKSURA", "\u062C\u062D\u0649", ["", "", "", "\uFDA6"]),
            new LigatureRecord("ARABIC LIGATURE JEEM WITH HAH WITH YEH", "\u062C\u062D\u064A", ["", "", "", "\uFDBE"]),
            new LigatureRecord("ARABIC LIGATURE JEEM WITH MEEM", "\u062C\u0645", ["\uFC16", "\uFCA8", "", ""]),
            new LigatureRecord("ARABIC LIGATURE JEEM WITH MEEM WITH ALEF MAKSURA", "\u062C\u0645\u0649", ["", "", "", "\uFDA7"]),
            new LigatureRecord("ARABIC LIGATURE JEEM WITH MEEM WITH HAH", "\u062C\u0645\u062D", ["", "\uFD59", "", "\uFD58"]),
            new LigatureRecord("ARABIC LIGATURE JEEM WITH MEEM WITH YEH", "\u062C\u0645\u064A", ["", "", "", "\uFDA5"]),
            new LigatureRecord("ARABIC LIGATURE JEEM WITH YEH", "\u062C\u064A", ["\uFD02", "", "", "\uFD1E"]),
            new LigatureRecord("ARABIC LIGATURE KAF WITH ALEF", "\u0643\u0627", ["\uFC37", "", "", "\uFC80"]),
            new LigatureRecord("ARABIC LIGATURE KAF WITH ALEF MAKSURA", "\u0643\u0649", ["\uFC3D", "", "", "\uFC83"]),
            new LigatureRecord("ARABIC LIGATURE KAF WITH HAH", "\u0643\u062D", ["\uFC39", "\uFCC5", "", ""]),
            new LigatureRecord("ARABIC LIGATURE KAF WITH JEEM", "\u0643\u062C", ["\uFC38", "\uFCC4", "", ""]),
            new LigatureRecord("ARABIC LIGATURE KAF WITH KHAH", "\u0643\u062E", ["\uFC3A", "\uFCC6", "", ""]),
            new LigatureRecord("ARABIC LIGATURE KAF WITH LAM", "\u0643\u0644", ["\uFC3B", "\uFCC7", "\uFCEB", "\uFC81"]),
            new LigatureRecord("ARABIC LIGATURE KAF WITH MEEM", "\u0643\u0645", ["\uFC3C", "\uFCC8", "\uFCEC", "\uFC82"]),
            new LigatureRecord("ARABIC LIGATURE KAF WITH MEEM WITH MEEM", "\u0643\u0645\u0645", ["", "\uFDC3", "", "\uFDBB"]),
            new LigatureRecord("ARABIC LIGATURE KAF WITH MEEM WITH YEH", "\u0643\u0645\u064A", ["", "", "", "\uFDB7"]),
            new LigatureRecord("ARABIC LIGATURE KAF WITH YEH", "\u0643\u064A", ["\uFC3E", "", "", "\uFC84"]),
            new LigatureRecord("ARABIC LIGATURE KHAH WITH ALEF MAKSURA", "\u062E\u0649", ["\uFD03", "", "", "\uFD1F"]),
            new LigatureRecord("ARABIC LIGATURE KHAH WITH HAH", "\u062E\u062D", ["\uFC1A", "", "", ""]),
            new LigatureRecord("ARABIC LIGATURE KHAH WITH JEEM", "\u062E\u062C", ["\uFC19", "\uFCAB", "", ""]),
            new LigatureRecord("ARABIC LIGATURE KHAH WITH MEEM", "\u062E\u0645", ["\uFC1B", "\uFCAC", "", ""]),
            new LigatureRecord("ARABIC LIGATURE KHAH WITH YEH", "\u062E\u064A", ["\uFD04", "", "", "\uFD20"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH ALEF", "\u0644\u0627", ["\uFEFB", "", "", "\uFEFC"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH ALEF MAKSURA", "\u0644\u0649", ["\uFC43", "", "", "\uFC86"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH ALEF WITH HAMZA ABOVE", "\u0644\u0623", ["\uFEF7", "", "", "\uFEF8"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH ALEF WITH HAMZA BELOW", "\u0644\u0625", ["\uFEF9", "", "", "\uFEFA"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH ALEF WITH MADDA ABOVE", "\u0644\u0622", ["\uFEF5", "", "", "\uFEF6"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH HAH", "\u0644\u062D", ["\uFC40", "\uFCCA", "", ""]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH HAH WITH ALEF MAKSURA", "\u0644\u062D\u0649", ["", "", "", "\uFD82"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH HAH WITH MEEM", "\u0644\u062D\u0645", ["", "\uFDB5", "", "\uFD80"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH HAH WITH YEH", "\u0644\u062D\u064A", ["", "", "", "\uFD81"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH HEH", "\u0644\u0647", ["", "\uFCCD", "", ""]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH JEEM", "\u0644\u062C", ["\uFC3F", "\uFCC9", "", ""]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH JEEM WITH JEEM", "\u0644\u062C\u062C", ["", "\uFD83", "", "\uFD84"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH JEEM WITH MEEM", "\u0644\u062C\u0645", ["", "\uFDBA", "", "\uFDBC"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH JEEM WITH YEH", "\u0644\u062C\u064A", ["", "", "", "\uFDAC"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH KHAH", "\u0644\u062E", ["\uFC41", "\uFCCB", "", ""]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH KHAH WITH MEEM", "\u0644\u062E\u0645", ["", "\uFD86", "", "\uFD85"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH MEEM", "\u0644\u0645", ["\uFC42", "\uFCCC", "\uFCED", "\uFC85"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH MEEM WITH HAH", "\u0644\u0645\u062D", ["", "\uFD88", "", "\uFD87"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH MEEM WITH YEH", "\u0644\u0645\u064A", ["", "", "", "\uFDAD"]),
            new LigatureRecord("ARABIC LIGATURE LAM WITH YEH", "\u0644\u064A", ["\uFC44", "", "", "\uFC87"]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH ALEF", "\u0645\u0627", ["", "", "", "\uFC88"]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH ALEF MAKSURA", "\u0645\u0649", ["\uFC49", "", "", ""]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH HAH", "\u0645\u062D", ["\uFC46", "\uFCCF", "", ""]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH HAH WITH JEEM", "\u0645\u062D\u062C", ["", "\uFD89", "", ""]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH HAH WITH MEEM", "\u0645\u062D\u0645", ["", "\uFD8A", "", ""]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH HAH WITH YEH", "\u0645\u062D\u064A", ["", "", "", "\uFD8B"]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH JEEM", "\u0645\u062C", ["\uFC45", "\uFCCE", "", ""]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH JEEM WITH HAH", "\u0645\u062C\u062D", ["", "\uFD8C", "", ""]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH JEEM WITH KHAH", "\u0645\u062C\u062E", ["", "\uFD92", "", ""]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH JEEM WITH MEEM", "\u0645\u062C\u0645", ["", "\uFD8D", "", ""]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH JEEM WITH YEH", "\u0645\u062C\u064A", ["", "", "", "\uFDC0"]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH KHAH", "\u0645\u062E", ["\uFC47", "\uFCD0", "", ""]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH KHAH WITH JEEM", "\u0645\u062E\u062C", ["", "\uFD8E", "", ""]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH KHAH WITH MEEM", "\u0645\u062E\u0645", ["", "\uFD8F", "", ""]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH KHAH WITH YEH", "\u0645\u062E\u064A", ["", "", "", "\uFDB9"]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH MEEM", "\u0645\u0645", ["\uFC48", "\uFCD1", "", "\uFC89"]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH MEEM WITH YEH", "\u0645\u0645\u064A", ["", "", "", "\uFDB1"]),
            new LigatureRecord("ARABIC LIGATURE MEEM WITH YEH", "\u0645\u064A", ["\uFC4A", "", "", ""]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH ALEF MAKSURA", "\u0646\u0649", ["\uFC4F", "", "", "\uFC8E"]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH HAH", "\u0646\u062D", ["\uFC4C", "\uFCD3", "", ""]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH HAH WITH ALEF MAKSURA", "\u0646\u062D\u0649", ["", "", "", "\uFD96"]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH HAH WITH MEEM", "\u0646\u062D\u0645", ["", "\uFD95", "", ""]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH HAH WITH YEH", "\u0646\u062D\u064A", ["", "", "", "\uFDB3"]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH HEH", "\u0646\u0647", ["", "\uFCD6", "\uFCEF", ""]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH JEEM", "\u0646\u062C", ["\uFC4B", "\uFCD2", "", ""]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH JEEM WITH ALEF MAKSURA", "\u0646\u062C\u0649", ["", "", "", "\uFD99"]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH JEEM WITH HAH", "\u0646\u062C\u062D", ["", "\uFDB8", "", "\uFDBD"]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH JEEM WITH MEEM", "\u0646\u062C\u0645", ["", "\uFD98", "", "\uFD97"]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH JEEM WITH YEH", "\u0646\u062C\u064A", ["", "", "", "\uFDC7"]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH KHAH", "\u0646\u062E", ["\uFC4D", "\uFCD4", "", ""]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH MEEM", "\u0646\u0645", ["\uFC4E", "\uFCD5", "\uFCEE", "\uFC8C"]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH MEEM WITH ALEF MAKSURA", "\u0646\u0645\u0649", ["", "", "", "\uFD9B"]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH MEEM WITH YEH", "\u0646\u0645\u064A", ["", "", "", "\uFD9A"]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH NOON", "\u0646\u0646", ["", "", "", "\uFC8D"]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH REH", "\u0646\u0631", ["", "", "", "\uFC8A"]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH YEH", "\u0646\u064A", ["\uFC50", "", "", "\uFC8F"]),
            new LigatureRecord("ARABIC LIGATURE NOON WITH ZAIN", "\u0646\u0632", ["", "", "", "\uFC8B"]),
            new LigatureRecord("ARABIC LIGATURE QAF WITH ALEF MAKSURA", "\u0642\u0649", ["\uFC35", "", "", "\uFC7E"]),
            new LigatureRecord("ARABIC LIGATURE QAF WITH HAH", "\u0642\u062D", ["\uFC33", "\uFCC2", "", ""]),
            new LigatureRecord("ARABIC LIGATURE QAF WITH MEEM", "\u0642\u0645", ["\uFC34", "\uFCC3", "", ""]),
            new LigatureRecord("ARABIC LIGATURE QAF WITH MEEM WITH HAH", "\u0642\u0645\u062D", ["", "\uFDB4", "", "\uFD7E"]),
            new LigatureRecord("ARABIC LIGATURE QAF WITH MEEM WITH MEEM", "\u0642\u0645\u0645", ["", "", "", "\uFD7F"]),
            new LigatureRecord("ARABIC LIGATURE QAF WITH MEEM WITH YEH", "\u0642\u0645\u064A", ["", "", "", "\uFDB2"]),
            new LigatureRecord("ARABIC LIGATURE QAF WITH YEH", "\u0642\u064A", ["\uFC36", "", "", "\uFC7F"]),
            new LigatureRecord("ARABIC LIGATURE QALA USED AS KORANIC STOP SIGN", "\u0642\u0644\u06D2", ["\uFDF1", "", "", ""]),
            new LigatureRecord("ARABIC LIGATURE REH WITH SUPERSCRIPT ALEF", "\u0631\u0670", ["\uFC5C", "", "", ""]),
            new LigatureRecord("ARABIC LIGATURE SAD WITH ALEF MAKSURA", "\u0635\u0649", ["\uFD05", "", "", "\uFD21"]),
            new LigatureRecord("ARABIC LIGATURE SAD WITH HAH", "\u0635\u062D", ["\uFC20", "\uFCB1", "", ""]),
            new LigatureRecord("ARABIC LIGATURE SAD WITH HAH WITH HAH", "\u0635\u062D\u062D", ["", "\uFD65", "", "\uFD64"]),
            new LigatureRecord("ARABIC LIGATURE SAD WITH HAH WITH YEH", "\u0635\u062D\u064A", ["", "", "", "\uFDA9"]),
            new LigatureRecord("ARABIC LIGATURE SAD WITH KHAH", "\u0635\u062E", ["", "\uFCB2", "", ""]),
            new LigatureRecord("ARABIC LIGATURE SAD WITH MEEM", "\u0635\u0645", ["\uFC21", "\uFCB3", "", ""]),
            new LigatureRecord("ARABIC LIGATURE SAD WITH MEEM WITH MEEM", "\u0635\u0645\u0645", ["", "\uFDC5", "", "\uFD66"]),
            new LigatureRecord("ARABIC LIGATURE SAD WITH REH", "\u0635\u0631", ["\uFD0F", "", "", "\uFD2B"]),
            new LigatureRecord("ARABIC LIGATURE SAD WITH YEH", "\u0635\u064A", ["\uFD06", "", "", "\uFD22"]),
            new LigatureRecord("ARABIC LIGATURE SALLA USED AS KORANIC STOP SIGN", "\u0635\u0644\u06D2", ["\uFDF0", "", "", ""]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH ALEF MAKSURA", "\u0633\u0649", ["\uFCFB", "", "", "\uFD17"]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH HAH", "\u0633\u062D", ["\uFC1D", "\uFCAE", "\uFD35", ""]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH HAH WITH JEEM", "\u0633\u062D\u062C", ["", "\uFD5C", "", ""]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH HEH", "\u0633\u0647", ["", "\uFD31", "\uFCE8", ""]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH JEEM", "\u0633\u062C", ["\uFC1C", "\uFCAD", "\uFD34", ""]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH JEEM WITH ALEF MAKSURA", "\u0633\u062C\u0649", ["", "", "", "\uFD5E"]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH JEEM WITH HAH", "\u0633\u062C\u062D", ["", "\uFD5D", "", ""]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH KHAH", "\u0633\u062E", ["\uFC1E", "\uFCAF", "\uFD36", ""]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH KHAH WITH ALEF MAKSURA", "\u0633\u062E\u0649", ["", "", "", "\uFDA8"]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH KHAH WITH YEH", "\u0633\u062E\u064A", ["", "", "", "\uFDC6"]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH MEEM", "\u0633\u0645", ["\uFC1F", "\uFCB0", "\uFCE7", ""]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH MEEM WITH HAH", "\u0633\u0645\u062D", ["", "\uFD60", "", "\uFD5F"]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH MEEM WITH JEEM", "\u0633\u0645\u062C", ["", "\uFD61", "", ""]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH MEEM WITH MEEM", "\u0633\u0645\u0645", ["", "\uFD63", "", "\uFD62"]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH REH", "\u0633\u0631", ["\uFD0E", "", "", "\uFD2A"]),
            new LigatureRecord("ARABIC LIGATURE SEEN WITH YEH", "\u0633\u064A", ["\uFCFC", "", "", "\uFD18"]),

        // Arabic ligatures with Shadda, the order of characters doesn"t matter
		    new LigatureRecord("ARABIC LIGATURE SHADDA WITH DAMMATAN ISOLATED FORM", "(?:\u064C\u0651|\u0651\u064C)", ["\uFC5E", "\uFC5E", "\uFC5E", "\uFC5E"]),
            new LigatureRecord("ARABIC LIGATURE SHADDA WITH KASRATAN ISOLATED FORM", "(?:\u064D\u0651|\u0651\u064D)",["\uFC5F", "\uFC5F", "\uFC5F", "\uFC5F"]),
            new LigatureRecord("ARABIC LIGATURE SHADDA WITH FATHA ISOLATED FORM", "(?:\u064E\u0651|\u0651\u064E)",["\uFC60", "\uFC60", "\uFC60", "\uFC60"]),
            new LigatureRecord("ARABIC LIGATURE SHADDA WITH DAMMA ISOLATED FORM", "(?:\u064F\u0651|\u0651\u064F)",["\uFC61", "\uFC61", "\uFC61", "\uFC61"]),
            new LigatureRecord("ARABIC LIGATURE SHADDA WITH KASRA ISOLATED FORM", "(?:\u0650\u0651|\u0651\u0650)",["\uFC62", "\uFC62", "\uFC62", "\uFC62"]),
            new LigatureRecord("ARABIC LIGATURE SHADDA WITH SUPERSCRIPT ALEF", "(?:\u0651\u0670|\u0670\u0651)", ["\uFC63", "", "", ""]),

        // There is a special case when they are with Tatweel
		    new LigatureRecord("ARABIC LIGATURE SHADDA WITH FATHA MEDIAL FORM", "\u0640(?:\u064E\u0651|\u0651\u064E)",["\uFCF2", "\uFCF2", "\uFCF2", "\uFCF2"]),
            new LigatureRecord("ARABIC LIGATURE SHADDA WITH DAMMA MEDIAL FORM", "\u0640(?:\u064F\u0651|\u0651\u064F)",["\uFCF3", "\uFCF3", "\uFCF3", "\uFCF3"]),
            new LigatureRecord("ARABIC LIGATURE SHADDA WITH KASRA MEDIAL FORM", "\u0640(?:\u0650\u0651|\u0651\u0650)",["\uFCF4", "\uFCF4", "\uFCF4", "\uFCF4"]),

        // Repeated with different keys to be backward compatible
		    new LigatureRecord("ARABIC LIGATURE SHADDA WITH FATHA", "\u0640(?:\u064E\u0651|\u0651\u064E)",["\uFCF2", "\uFCF2", "\uFCF2", "\uFCF2"]),
            new LigatureRecord("ARABIC LIGATURE SHADDA WITH DAMMA", "\u0640(?:\u064F\u0651|\u0651\u064F)",["\uFCF3", "\uFCF3", "\uFCF3", "\uFCF3"]),
            new LigatureRecord("ARABIC LIGATURE SHADDA WITH KASRA", "\u0640(?:\u0650\u0651|\u0651\u0650)",["\uFCF4", "\uFCF4", "\uFCF4", "\uFCF4"]),

            new LigatureRecord("ARABIC LIGATURE SHEEN WITH ALEF MAKSURA", "\u0634\u0649", ["\uFCFD", "", "", "\uFD19"]),
            new LigatureRecord("ARABIC LIGATURE SHEEN WITH HAH", "\u0634\u062D", ["\uFD0A", "\uFD2E", "\uFD38", "\uFD26"]),
            new LigatureRecord("ARABIC LIGATURE SHEEN WITH HAH WITH MEEM", "\u0634\u062D\u0645", ["", "\uFD68", "", "\uFD67"]),
            new LigatureRecord("ARABIC LIGATURE SHEEN WITH HAH WITH YEH", "\u0634\u062D\u064A", ["", "", "", "\uFDAA"]),
            new LigatureRecord("ARABIC LIGATURE SHEEN WITH HEH", "\u0634\u0647", ["", "\uFD32", "\uFCEA", ""]),
            new LigatureRecord("ARABIC LIGATURE SHEEN WITH JEEM", "\u0634\u062C", ["\uFD09", "\uFD2D", "\uFD37", "\uFD25"]),
            new LigatureRecord("ARABIC LIGATURE SHEEN WITH JEEM WITH YEH", "\u0634\u062C\u064A", ["", "", "", "\uFD69"]),
            new LigatureRecord("ARABIC LIGATURE SHEEN WITH KHAH", "\u0634\u062E", ["\uFD0B", "\uFD2F", "\uFD39", "\uFD27"]),
            new LigatureRecord("ARABIC LIGATURE SHEEN WITH MEEM", "\u0634\u0645", ["\uFD0C", "\uFD30", "\uFCE9", "\uFD28"]),
            new LigatureRecord("ARABIC LIGATURE SHEEN WITH MEEM WITH KHAH", "\u0634\u0645\u062E", ["", "\uFD6B", "", "\uFD6A"]),
            new LigatureRecord("ARABIC LIGATURE SHEEN WITH MEEM WITH MEEM", "\u0634\u0645\u0645", ["", "\uFD6D", "", "\uFD6C"]),
            new LigatureRecord("ARABIC LIGATURE SHEEN WITH REH", "\u0634\u0631", ["\uFD0D", "", "", "\uFD29"]),
            new LigatureRecord("ARABIC LIGATURE SHEEN WITH YEH", "\u0634\u064A", ["\uFCFE", "", "", "\uFD1A"]),
            new LigatureRecord("ARABIC LIGATURE TAH WITH ALEF MAKSURA", "\u0637\u0649", ["\uFCF5", "", "", "\uFD11"]),
            new LigatureRecord("ARABIC LIGATURE TAH WITH HAH", "\u0637\u062D", ["\uFC26", "\uFCB8", "", ""]),
            new LigatureRecord("ARABIC LIGATURE TAH WITH MEEM", "\u0637\u0645", ["\uFC27", "\uFD33", "\uFD3A", ""]),
            new LigatureRecord("ARABIC LIGATURE TAH WITH MEEM WITH HAH", "\u0637\u0645\u062D", ["", "\uFD72", "", "\uFD71"]),
            new LigatureRecord("ARABIC LIGATURE TAH WITH MEEM WITH MEEM", "\u0637\u0645\u0645", ["", "\uFD73", "", ""]),
            new LigatureRecord("ARABIC LIGATURE TAH WITH MEEM WITH YEH", "\u0637\u0645\u064A", ["", "", "", "\uFD74"]),
            new LigatureRecord("ARABIC LIGATURE TAH WITH YEH", "\u0637\u064A", ["\uFCF6", "", "", "\uFD12"]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH ALEF MAKSURA", "\u062A\u0649", ["\uFC0F", "", "", "\uFC74"]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH HAH", "\u062A\u062D", ["\uFC0C", "\uFCA2", "", ""]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH HAH WITH JEEM", "\u062A\u062D\u062C", ["", "\uFD52", "", "\uFD51"]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH HAH WITH MEEM", "\u062A\u062D\u0645", ["", "\uFD53", "", ""]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH HEH", "\u062A\u0647", ["", "\uFCA5", "\uFCE4", ""]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH JEEM", "\u062A\u062C", ["\uFC0B", "\uFCA1", "", ""]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH JEEM WITH ALEF MAKSURA", "\u062A\u062C\u0649", ["", "", "", "\uFDA0"]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH JEEM WITH MEEM", "\u062A\u062C\u0645", ["", "\uFD50", "", ""]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH JEEM WITH YEH", "\u062A\u062C\u064A", ["", "", "", "\uFD9F"]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH KHAH", "\u062A\u062E", ["\uFC0D", "\uFCA3", "", ""]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH KHAH WITH ALEF MAKSURA", "\u062A\u062E\u0649", ["", "", "", "\uFDA2"]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH KHAH WITH MEEM", "\u062A\u062E\u0645", ["", "\uFD54", "", ""]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH KHAH WITH YEH", "\u062A\u062E\u064A", ["", "", "", "\uFDA1"]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH MEEM", "\u062A\u0645", ["\uFC0E", "\uFCA4", "\uFCE3", "\uFC72"]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH MEEM WITH ALEF MAKSURA", "\u062A\u0645\u0649", ["", "", "", "\uFDA4"]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH MEEM WITH HAH", "\u062A\u0645\u062D", ["", "\uFD56", "", ""]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH MEEM WITH JEEM", "\u062A\u0645\u062C", ["", "\uFD55", "", ""]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH MEEM WITH KHAH", "\u062A\u0645\u062E", ["", "\uFD57", "", ""]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH MEEM WITH YEH", "\u062A\u0645\u064A", ["", "", "", "\uFDA3"]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH NOON", "\u062A\u0646", ["", "", "", "\uFC73"]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH REH", "\u062A\u0631", ["", "", "", "\uFC70"]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH YEH", "\u062A\u064A", ["\uFC10", "", "", "\uFC75"]),
            new LigatureRecord("ARABIC LIGATURE TEH WITH ZAIN", "\u062A\u0632", ["", "", "", "\uFC71"]),
            new LigatureRecord("ARABIC LIGATURE THAL WITH SUPERSCRIPT ALEF", "\u0630\u0670", ["\uFC5B", "", "", ""]),
            new LigatureRecord("ARABIC LIGATURE THEH WITH ALEF MAKSURA", "\u062B\u0649", ["\uFC13", "", "", "\uFC7A"]),
            new LigatureRecord("ARABIC LIGATURE THEH WITH HEH", "\u062B\u0647", ["", "", "\uFCE6", ""]),
            new LigatureRecord("ARABIC LIGATURE THEH WITH JEEM", "\u062B\u062C", ["\uFC11", "", "", ""]),
            new LigatureRecord("ARABIC LIGATURE THEH WITH MEEM", "\u062B\u0645", ["\uFC12", "\uFCA6", "\uFCE5", "\uFC78"]),
            new LigatureRecord("ARABIC LIGATURE THEH WITH NOON", "\u062B\u0646", ["", "", "", "\uFC79"]),
            new LigatureRecord("ARABIC LIGATURE THEH WITH REH", "\u062B\u0631", ["", "", "", "\uFC76"]),
            new LigatureRecord("ARABIC LIGATURE THEH WITH YEH", "\u062B\u064A", ["\uFC14", "", "", "\uFC7B"]),
            new LigatureRecord("ARABIC LIGATURE THEH WITH ZAIN", "\u062B\u0632", ["", "", "", "\uFC77"]),
            new LigatureRecord("ARABIC LIGATURE UIGHUR KIRGHIZ YEH WITH HAMZA ABOVE WITH ALEF MAKSURA", "\u0626\u0649", ["\uFBF9", "\uFBFB", "", "\uFBFA"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH ALEF MAKSURA", "\u064A\u0649", ["\uFC59", "", "", "\uFC95"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAH", "\u064A\u062D", ["\uFC56", "\uFCDB", "", ""]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAH WITH YEH", "\u064A\u062D\u064A", ["", "", "", "\uFDAE"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH AE", "\u0626\u06D5", ["\uFBEC", "", "", "\uFBED"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH ALEF", "\u0626\u0627", ["\uFBEA", "", "", "\uFBEB"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH ALEF MAKSURA", "\u0626\u0649", ["\uFC03", "", "", "\uFC68"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH E", "\u0626\u06D0", ["\uFBF6", "\uFBF8", "", "\uFBF7"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH HAH", "\u0626\u062D", ["\uFC01", "\uFC98", "", ""]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH HEH", "\u0626\u0647", ["", "\uFC9B", "\uFCE0", ""]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH JEEM", "\u0626\u062C", ["\uFC00", "\uFC97", "", ""]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH KHAH", "\u0626\u062E", ["", "\uFC99", "", ""]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH MEEM", "\u0626\u0645", ["\uFC02", "\uFC9A", "\uFCDF", "\uFC66"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH NOON", "\u0626\u0646", ["", "", "", "\uFC67"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH OE", "\u0626\u06C6", ["\uFBF2", "", "", "\uFBF3"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH REH", "\u0626\u0631", ["", "", "", "\uFC64"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH U", "\u0626\u06C7", ["\uFBF0", "", "", "\uFBF1"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH WAW", "\u0626\u0648", ["\uFBEE", "", "", "\uFBEF"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH YEH", "\u0626\u064A", ["\uFC04", "", "", "\uFC69"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH YU", "\u0626\u06C8", ["\uFBF4", "", "", "\uFBF5"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HAMZA ABOVE WITH ZAIN", "\u0626\u0632", ["", "", "", "\uFC65"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH HEH", "\u064A\u0647", ["", "\uFCDE", "\uFCF1", ""]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH JEEM", "\u064A\u062C", ["\uFC55", "\uFCDA", "", ""]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH JEEM WITH YEH", "\u064A\u062C\u064A", ["", "", "", "\uFDAF"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH KHAH", "\u064A\u062E", ["\uFC57", "\uFCDC", "", ""]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH MEEM", "\u064A\u0645", ["\uFC58", "\uFCDD", "\uFCF0", "\uFC93"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH MEEM WITH MEEM", "\u064A\u0645\u0645", ["", "\uFD9D", "", "\uFD9C"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH MEEM WITH YEH", "\u064A\u0645\u064A", ["", "", "", "\uFDB0"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH NOON", "\u064A\u0646", ["", "", "", "\uFC94"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH REH", "\u064A\u0631", ["", "", "", "\uFC91"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH YEH", "\u064A\u064A", ["\uFC5A", "", "", "\uFC96"]),
            new LigatureRecord("ARABIC LIGATURE YEH WITH ZAIN", "\u064A\u0632", ["", "", "", "\uFC92"]),
            new LigatureRecord("ARABIC LIGATURE ZAH WITH MEEM", "\u0638\u0645", ["\uFC28", "\uFCB9", "\uFD3B", ""]),
        ];
        
        public static readonly List<LigatureRecord> AllLigatures = [.. SentencesLigatures, .. WordsLigatures, .. LettersLigatures];

        /// <summary>
        /// Checks if a letter can connect to a preceding letter.
        /// </summary>
        public static bool ConnectsWithLetterBefore(string letter, Dictionary<string, string[]> letters)
        {
            if (!letters.TryGetValue(letter, out var forms)) return false;
            return !string.IsNullOrEmpty(forms[(int)GlyphForm.Final]) || !string.IsNullOrEmpty(forms[(int)GlyphForm.Medial]);
        }

        /// <summary>
        /// Checks if a letter can connect to a following letter.
        /// </summary>
        public static bool ConnectsWithLetterAfter(string letter, Dictionary<string, string[]> letters)
        {
            if (!letters.TryGetValue(letter, out var forms)) return false;
            return !string.IsNullOrEmpty(forms[(int)GlyphForm.Initial]) || !string.IsNullOrEmpty(forms[(int)GlyphForm.Medial]);
        }

        /// <summary>
        /// Checks if a letter can connect to both preceding and following letters.
        /// </summary>
        public static bool ConnectsWithLettersBeforeAndAfter(string letter, Dictionary<string, string[]> letters)
        {
            if (!letters.TryGetValue(letter, out var forms)) return false;
            return !string.IsNullOrEmpty(forms[(int)GlyphForm.Medial]);
        }
    }
}

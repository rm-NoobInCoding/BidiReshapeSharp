using Icu;

namespace BidiReshapeSharp.Bidi
{

    /// <summary>
    /// A C# implementation of the Unicode Bidirectional Algorithm (UBA).
    /// </summary>
    public static class Bidi
    {
        #region Private Data Structures

        private class BidiCharacter
        {
            public required string Char { get; set; }
            public int Level { get; set; }
            public required string BidiType { get; set; }
            public required string OriginalBidiType { get; set; }
        }

        private class LevelRun
        {
            public required string Sor { get; set; }
            public required string Eor { get; set; }
            public int Start { get; set; }
            public int Length { get; set; }
        }

        private class BidiStorage
        {
            public int BaseLevel { get; set; }
            public string BaseDir { get; set; } = "";
            public List<BidiCharacter> Chars { get; } = [];
            public List<LevelRun> Runs { get; } = [];
        }

        #endregion

        #region Constants and Mappings

        private static readonly Dictionary<string, int> ParagraphLevels = new()
        {
            { "L", 0 }, { "AL", 1 }, { "R", 1 }
        };

        private const int ExplicitLevelLimit = 62;

        private static int LeastGreaterOdd(int x) => x + 1 | 1;
        private static int LeastGreaterEven(int x) => x + 2 & ~1;

        private static readonly Dictionary<string, (Func<int, int>, string)> X2X5Mappings = new()
        {
            { "RLE", (LeastGreaterOdd, "N") },
            { "LRE", (LeastGreaterEven, "N") },
            { "RLO", (LeastGreaterOdd, "R") },
            { "LRO", (LeastGreaterEven, "L") }
        };

        private static readonly HashSet<string> X6Ignored = [.. X2X5Mappings.Keys, "BN", "PDF", "B"];

        private static readonly HashSet<string> X9Removed = [.. X2X5Mappings.Keys, "BN", "PDF"];

        private static string EmbeddingDirection(int x) => x % 2 == 0 ? "L" : "R";

        #endregion

        #region Main Public Method

        /// <summary>
        /// Reorders a string to its correct display layout according to the Unicode Bidirectional Algorithm.
        /// </summary>
        /// <param name="text">The string to process.</param>
        /// <param name="upperIsRtl">If true, treats uppercase Latin characters as strong 'R' types for debugging.</param>
        /// <param name="baseDir">Overrides the auto-detected paragraph direction. Can be 'L' or 'R'.</param>
        /// <returns>A string with characters rearranged for correct bidirectional display.</returns>
        public static string GetDisplay(string text, bool upperIsRtl = false, string? baseDir = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var storage = new BidiStorage
            {
                BaseLevel = baseDir == null
                    ? GetBaseLevel(text, upperIsRtl)
                    : ParagraphLevels[baseDir]
            };
            storage.BaseDir = storage.BaseLevel == 0 ? "L" : "R";

            GetEmbeddingLevels(text, storage, upperIsRtl);
            ExplicitEmbedAndOverrides(storage);
            if (storage.Chars.Count == 0) return string.Empty;

            ResolveWeakTypes(storage);
            ResolveNeutralTypes(storage);
            ResolveImplicitLevels(storage);
            ReorderResolvedLevels(storage);
            ApplyMirroring(storage);

            return string.Concat(storage.Chars.Select(c => c.Char));
        }

        #endregion

        #region Unicode Data Helpers

        private static string GetBidiType(string s)
        {
            int codePoint = char.ConvertToUtf32(s, 0);
            var direction = Character.CharDirection(codePoint);

            return direction switch
            {
                Character.UCharDirection.LEFT_TO_RIGHT => "L",
                Character.UCharDirection.RIGHT_TO_LEFT => "R",
                Character.UCharDirection.EUROPEAN_NUMBER => "EN",
                Character.UCharDirection.EUROPEAN_NUMBER_SEPARATOR => "ES",
                Character.UCharDirection.EUROPEAN_NUMBER_TERMINATOR => "ET",
                Character.UCharDirection.ARABIC_NUMBER => "AN",
                Character.UCharDirection.COMMON_NUMBER_SEPARATOR => "CS",
                Character.UCharDirection.BLOCK_SEPARATOR => "B",
                Character.UCharDirection.SEGMENT_SEPARATOR => "S",
                Character.UCharDirection.WHITE_SPACE_NEUTRAL => "WS",
                Character.UCharDirection.OTHER_NEUTRAL => "ON",
                Character.UCharDirection.LEFT_TO_RIGHT_EMBEDDING => "LRE",
                Character.UCharDirection.LEFT_TO_RIGHT_OVERRIDE => "LRO",
                Character.UCharDirection.RIGHT_TO_LEFT_ARABIC => "AL",
                Character.UCharDirection.RIGHT_TO_LEFT_EMBEDDING => "RLE",
                Character.UCharDirection.RIGHT_TO_LEFT_OVERRIDE => "RLO",
                Character.UCharDirection.POP_DIRECTIONAL_FORMAT => "PDF",
                Character.UCharDirection.DIR_NON_SPACING_MARK => "NSM",
                Character.UCharDirection.BOUNDARY_NEUTRAL => "BN",
                Character.UCharDirection.FIRST_STRONG_ISOLATE => "FSI",
                Character.UCharDirection.LEFT_TO_RIGHT_ISOLATE => "LRI",
                Character.UCharDirection.RIGHT_TO_LEFT_ISOLATE => "RLI",
                Character.UCharDirection.POP_DIRECTIONAL_ISOLATE => "PDI",
                _ => "ON",// Default for unhandled cases
            };
        }

        #endregion

        #region Algorithm Steps (P, X, W, N, I, L rules)

        private static IEnumerable<string> EnumerateRunes(string text)
        {
            for (int i = 0; i < text.Length; ++i)
            {
                if (char.IsHighSurrogate(text[i]))
                {
                    if (i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]))
                    {
                        yield return text.Substring(i, 2);
                        i++;
                    }
                    else { yield return "?"; } // Invalid surrogate
                }
                else
                {
                    yield return text[i].ToString();
                }
            }
        }

        private static int GetBaseLevel(string text, bool upperIsRtl)
        {
            foreach (var chStr in EnumerateRunes(text))
            {
                if (upperIsRtl && chStr.All(char.IsUpper))
                    return 1;

                string bidiType = GetBidiType(chStr);
                if (bidiType == "AL" || bidiType == "R")
                    return 1;
                if (bidiType == "L")
                    return 0;
            }
            return 0; // P3: Default to LTR
        }

        private static void GetEmbeddingLevels(string text, BidiStorage storage, bool upperIsRtl)
        {
            foreach (var chStr in EnumerateRunes(text))
            {
                string bidiType = upperIsRtl && chStr.All(char.IsUpper)
                    ? "R"
                    : GetBidiType(chStr);

                storage.Chars.Add(new BidiCharacter
                {
                    Char = chStr,
                    Level = storage.BaseLevel,
                    BidiType = bidiType,
                    OriginalBidiType = bidiType
                });
            }
        }

        private static void ExplicitEmbedAndOverrides(BidiStorage storage)
        {
            var levels = new Stack<(int, string)>();
            int embeddingLevel = storage.BaseLevel;
            string directionalOverride = "N";
            int overflowCounter = 0;
            int almostOverflowCounter = 0;

            foreach (var ch in storage.Chars)
            {
                string bidiType = ch.BidiType;

                if (X2X5Mappings.TryGetValue(bidiType, out var mapping))
                {
                    if (overflowCounter > 0)
                    {
                        overflowCounter++;
                        continue;
                    }

                    int newLevel = mapping.Item1(embeddingLevel);
                    if (newLevel < ExplicitLevelLimit)
                    {
                        levels.Push((embeddingLevel, directionalOverride));
                        embeddingLevel = newLevel;
                        directionalOverride = mapping.Item2;
                    }
                    else if (embeddingLevel == ExplicitLevelLimit - 2)
                    {
                        almostOverflowCounter++;
                    }
                    else
                    {
                        overflowCounter++;
                    }
                }
                else if (bidiType == "PDF")
                {
                    if (overflowCounter > 0)
                    {
                        overflowCounter--;
                    }
                    else if (almostOverflowCounter > 0 && embeddingLevel != ExplicitLevelLimit - 1)
                    {
                        almostOverflowCounter--;
                    }
                    else if (levels.Count > 0)
                    {
                        (embeddingLevel, directionalOverride) = levels.Pop();
                    }
                }
                else if (bidiType == "B")
                {
                    levels.Clear();
                    overflowCounter = almostOverflowCounter = 0;
                    embeddingLevel = storage.BaseLevel;
                    ch.Level = storage.BaseLevel;
                    directionalOverride = "N";
                }
                else if (!X6Ignored.Contains(bidiType))
                {
                    ch.Level = embeddingLevel;
                    if (directionalOverride != "N")
                    {
                        ch.BidiType = directionalOverride;
                    }
                }
            }

            // X9: Remove formatting characters
            storage.Chars.RemoveAll(ch => X9Removed.Contains(ch.OriginalBidiType));

            CalcLevelRuns(storage);
        }

        private static void CalcLevelRuns(BidiStorage storage)
        {
            storage.Runs.Clear();
            if (storage.Chars.Count == 0) return;

            static string CalcRunLevel(int b1, int b2) => Math.Max(b1, b2) % 2 == 0 ? "L" : "R";

            string sor = CalcRunLevel(storage.BaseLevel, storage.Chars[0].Level);
            int runStart = 0;
            int prevLevel = storage.Chars[0].Level;

            for (int i = 1; i < storage.Chars.Count; i++)
            {
                int currLevel = storage.Chars[i].Level;
                if (currLevel != prevLevel)
                {
                    string eor = CalcRunLevel(prevLevel, currLevel);
                    storage.Runs.Add(new LevelRun
                    {
                        Sor = sor,
                        Eor = eor,
                        Start = runStart,
                        Length = i - runStart
                    });
                    sor = eor;
                    runStart = i;
                    prevLevel = currLevel;
                }
            }

            // Add the last run
            string lastEor = CalcRunLevel(prevLevel, storage.BaseLevel);
            storage.Runs.Add(new LevelRun
            {
                Sor = sor,
                Eor = lastEor,
                Start = runStart,
                Length = storage.Chars.Count - runStart
            });
        }

        private static void ResolveWeakTypes(BidiStorage storage)
        {
            foreach (var run in storage.Runs)
            {
                var chars = storage.Chars.GetRange(run.Start, run.Length);
                if (chars.Count == 0) continue;

                // W1: Non-spacing marks
                string prevType = run.Sor;
                foreach (var ch in chars)
                {
                    if (ch.BidiType == "NSM") ch.BidiType = prevType;
                    prevType = ch.BidiType;
                }

                // W2: European numbers
                string prevStrong = run.Sor;
                foreach (var ch in chars)
                {
                    if (ch.BidiType == "EN" && prevStrong == "AL") ch.BidiType = "AN";
                    if (ch.BidiType == "R" || ch.BidiType == "L" || ch.BidiType == "AL")
                        prevStrong = ch.BidiType;
                }

                // W3: AL to R
                foreach (var ch in chars.Where(c => c.BidiType == "AL")) ch.BidiType = "R";

                // W4: Separators
                for (int i = 1; i < chars.Count - 1; i++)
                {
                    if (chars[i].BidiType == "ES" && chars[i - 1].BidiType == "EN" && chars[i + 1].BidiType == "EN")
                        chars[i].BidiType = "EN";
                    if (chars[i].BidiType == "CS" && chars[i - 1].BidiType == chars[i + 1].BidiType &&
                        (chars[i - 1].BidiType == "EN" || chars[i - 1].BidiType == "AN"))
                        chars[i].BidiType = chars[i - 1].BidiType;
                }

                // W5: European terminators
                for (int i = 0; i < chars.Count; i++)
                {
                    if (chars[i].BidiType == "EN")
                    {
                        for (int j = i - 1; j >= 0 && chars[j].BidiType == "ET"; j--) chars[j].BidiType = "EN";
                        for (int j = i + 1; j < chars.Count && chars[j].BidiType == "ET"; j++) chars[j].BidiType = "EN";
                    }
                }

                // W6: Other neutrals
                foreach (var ch in chars.Where(c => c.BidiType == "ET" || c.BidiType == "ES" || c.BidiType == "CS"))
                    ch.BidiType = "ON";

                // W7: EN to L
                prevStrong = run.Sor;
                foreach (var ch in chars)
                {
                    if (ch.BidiType == "EN" && prevStrong == "L") ch.BidiType = "L";
                    if (ch.BidiType == "L" || ch.BidiType == "R") prevStrong = ch.BidiType;
                }
            }
        }

        private static void ResolveNeutralTypes(BidiStorage storage)
        {
            var neutralTypes = new HashSet<string> { "B", "S", "WS", "ON" };
            foreach (var run in storage.Runs)
            {
                if (run.Length == 0) continue;
                var runChars = storage.Chars.GetRange(run.Start, run.Length);

                for (int i = 0; i < run.Length;)
                {
                    var ch = runChars[i];
                    if (!neutralTypes.Contains(ch.BidiType))
                    {
                        i++;
                        continue;
                    }

                    int seqStart = i;
                    int seqEnd = i;
                    while (seqEnd + 1 < run.Length && neutralTypes.Contains(runChars[seqEnd + 1].BidiType))
                    {
                        seqEnd++;
                    }

                    string prevType = seqStart == 0 ? run.Sor : runChars[seqStart - 1].BidiType;
                    string nextType = seqEnd == run.Length - 1 ? run.Eor : runChars[seqEnd + 1].BidiType;

                    if (prevType == "AN" || prevType == "EN") prevType = "R";
                    if (nextType == "AN" || nextType == "EN") nextType = "R";

                    string newType = prevType == nextType ? prevType : EmbeddingDirection(ch.Level);

                    for (int j = seqStart; j <= seqEnd; j++)
                    {
                        runChars[j].BidiType = newType;
                    }

                    i = seqEnd + 1;
                }
            }
        }

        private static void ResolveImplicitLevels(BidiStorage storage)
        {
            foreach (var ch in storage.Chars)
            {
                if (EmbeddingDirection(ch.Level) == "L") // Even level
                {
                    if (ch.BidiType == "R") ch.Level += 1;
                    else if (ch.BidiType == "AN" || ch.BidiType == "EN") ch.Level += 2;
                }
                else // Odd level
                {
                    if (ch.BidiType != "R") ch.Level += 1;
                }
            }
        }

        private static void ReorderResolvedLevels(BidiStorage storage)
        {
            // L1: Reset certain characters to the base level
            bool shouldReset = true;
            for (int i = storage.Chars.Count - 1; i >= 0; i--)
            {
                var ch = storage.Chars[i];
                if (ch.OriginalBidiType == "B" || ch.OriginalBidiType == "S")
                {
                    ch.Level = storage.BaseLevel;
                    shouldReset = true;
                }
                else if (shouldReset && (ch.OriginalBidiType == "WS" || ch.OriginalBidiType == "BN"))
                {
                    ch.Level = storage.BaseLevel;
                }
                else
                {
                    shouldReset = false;
                }
            }

            // L2: Reverse sequences
            int lineStart = 0;
            for (int i = 0; i < storage.Chars.Count; i++)
            {
                if (i + 1 == storage.Chars.Count || storage.Chars[i].OriginalBidiType == "B")
                {
                    int lineEnd = storage.Chars[i].OriginalBidiType == "B" ? i - 1 : i;
                    if (lineEnd >= lineStart)
                    {
                        var lineChars = storage.Chars.GetRange(lineStart, lineEnd - lineStart + 1);
                        int highestLevel = 0;
                        int lowestOddLevel = ExplicitLevelLimit;

                        foreach (var ch in lineChars)
                        {
                            if (ch.Level > highestLevel) highestLevel = ch.Level;
                            if (ch.Level % 2 != 0 && ch.Level < lowestOddLevel) lowestOddLevel = ch.Level;
                        }

                        for (int level = highestLevel; level >= lowestOddLevel; level--)
                        {
                            int start = -1;
                            for (int j = 0; j <= lineChars.Count; j++)
                            {
                                if (j < lineChars.Count && lineChars[j].Level >= level)
                                {
                                    if (start == -1) start = j;
                                }
                                else
                                {
                                    if (start != -1)
                                    {
                                        int count = j - start;
                                        storage.Chars.Reverse(lineStart + start, count);
                                        start = -1;
                                    }
                                }
                            }
                        }
                    }
                    lineStart = i + 1;
                }
            }
        }

        private static void ApplyMirroring(BidiStorage storage)
        {
            foreach (var ch in storage.Chars)
            {
                if (EmbeddingDirection(ch.Level) == "R" && Mirrored.MirroredChars.TryGetValue(ch.Char, out var mirrored))
                {
                    ch.Char = mirrored;
                }
            }
        }
        #endregion
    }
}

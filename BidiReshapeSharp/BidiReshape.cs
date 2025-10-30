namespace BidiReshapeSharp
{
    /// <summary>
    /// Provides methods for reshaping and displaying bidirectional text (e.g., Arabic or Persian)
    /// in a visually correct form. Supports both string and file input.
    /// </summary>
    public static class BidiReshape
    {
        /// <summary>
        /// Reshapes and reorders the given text using the specified configuration so it displays correctly.
        /// </summary>
        /// <param name="text">The input text to process.</param>
        /// <param name="config">The reshaper configuration for controlling shaping behavior.</param>
        /// <returns>The reshaped and bidirectionally-corrected text.</returns>
        public static string ProcessString(string text, Reshaper.ReshaperConfig config)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            var reshaper = new Reshaper.ArabicReshaper(config);
            var reshapedText = reshaper.Reshape(text);
            return BidiSharp.Bidi.LogicalToVisual(reshapedText);

        }

        /// <summary>
        /// Reshapes and reorders the given text using the default configuration.
        /// </summary>
        /// <param name="text">The input text to process.</param>
        /// <returns>The reshaped and bidirectionally-corrected text.</returns>
        public static string ProcessString(string text)
        {
            return ProcessString(text, new Reshaper.ReshaperConfig());
        }

        /// <summary>
        /// Reshapes and reorders the given text in-place using the specified configuration.
        /// </summary>
        /// <param name="text">The input text to process (modified in place).</param>
        /// <param name="config">The reshaper configuration for controlling shaping behavior.</param>
        public static void ProcessStringInPlace(ref string text, Reshaper.ReshaperConfig config)
        {
            text = ProcessString(text, config);
        }

        /// <summary>
        /// Reshapes and reorders the given text in-place using the default configuration.
        /// </summary>
        /// <param name="text">The input text to process (modified in place).</param>
        public static void ProcessStringInPlace(ref string text)
        {
            text = ProcessString(text);
        }

        /// <summary>
        /// Processes the contents of a file by reshaping and reordering its text,
        /// then writes the processed result to the output file.
        /// </summary>
        /// <param name="inputFilePath">The path to the input file.</param>
        /// <param name="outputFilePath">The path to the output file where processed text will be written.</param>
        /// <param name="config">The reshaper configuration for controlling shaping behavior.</param>
        /// <param name="processLineByLine">If true, processes the file line by line; otherwise, processes the entire file as one string.</param>
        public static void ProcessFile(string inputFilePath, string outputFilePath, Reshaper.ReshaperConfig config, bool processLineByLine = false)
        {
            if (processLineByLine)
            {
                var lines = File.ReadAllLines(inputFilePath);
                foreach (var (line, index) in lines.Select((value, i) => (value, i)))
                {
                    lines[index] = ProcessString(line, config);
                }
                File.WriteAllLines(outputFilePath, lines);
                return;
            }

            var text = File.ReadAllText(inputFilePath);
            var processedText = ProcessString(text, config);
            File.WriteAllText(outputFilePath, processedText);
        }

        /// <summary>
        /// Processes the contents of a file by reshaping and reordering its text
        /// using the default configuration, then writes the processed result to the output file.
        /// </summary>
        /// <param name="inputFilePath">The path to the input file.</param>
        /// <param name="outputFilePath">The path to the output file where processed text will be written.</param>
        /// <param name="processLineByLine">If true, processes the file line by line; otherwise, processes the entire file as one string.</param>
        public static void ProcessFile(string inputFilePath, string outputFilePath, bool processLineByLine = false)
        {
            ProcessFile(inputFilePath, outputFilePath, new Reshaper.ReshaperConfig(), processLineByLine);
        }
    }
}

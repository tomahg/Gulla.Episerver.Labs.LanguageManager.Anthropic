namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    /// <summary>
    /// Builds the system prompt used to instruct Claude to translate text.
    /// Pure and dependency-free so it can be unit-tested in isolation.
    /// </summary>
    public static class AnthropicSystemPrompt
    {
        /// <summary>
        /// Instruction appended to every system prompt to suppress any preamble
        /// (e.g. "Here is the translation:") so the output contains only the translation.
        /// </summary>
        public const string PreambleGuard = "Output only the translation, with no preamble or explanation.";

        /// <summary>
        /// Build the system prompt for a translation request.
        /// </summary>
        /// <param name="fromLanguageName">The name of the source language.</param>
        /// <param name="toLanguageName">The name of the destination language.</param>
        /// <param name="extraPrompt">Optional additional instruction (e.g. tone/formality). Ignored when null or empty.</param>
        public static string Build(string fromLanguageName, string toLanguageName, string? extraPrompt)
        {
            var prompt = $"You are a translator. Translate from {fromLanguageName} to {toLanguageName}.";

            if (!string.IsNullOrEmpty(extraPrompt))
            {
                prompt += " " + extraPrompt;
            }

            prompt += " " + PreambleGuard;

            return prompt;
        }
    }
}

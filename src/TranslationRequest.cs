namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    /// <summary>
    /// A provider-agnostic description of one translation turn. Built purely from
    /// options, language names, and an already-resolved extra prompt via <see cref="From"/>,
    /// so the request shape is assertable without calling the API or the CMS.
    /// </summary>
    public sealed record TranslationRequest(
        string Model,
        int MaxTokens,
        string SystemPrompt,
        string UserText)
    {
        /// <summary>
        /// Build a <see cref="TranslationRequest"/> from the addon options, the
        /// source/target language names, and the resolved extra prompt (which the
        /// caller obtains from the Extra Prompt Resolver).
        /// </summary>
        public static TranslationRequest From(
            LanguageManagerAnthropicOptions options,
            string fromLanguageName,
            string toLanguageName,
            string text,
            string? extraPrompt)
        {
            return new TranslationRequest(
                options.AnthropicModel,
                options.AnthropicMaxTokens,
                AnthropicSystemPrompt.Build(fromLanguageName, toLanguageName, extraPrompt),
                text);
        }
    }
}

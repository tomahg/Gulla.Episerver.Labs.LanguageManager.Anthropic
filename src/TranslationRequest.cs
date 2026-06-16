namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    /// <summary>
    /// A provider-agnostic description of one translation turn. Built purely from
    /// options and language names via <see cref="From"/>, so the request shape is
    /// assertable without calling the API.
    /// </summary>
    public sealed record TranslationRequest(
        string Model,
        int MaxTokens,
        double? Temperature,
        string SystemPrompt,
        string UserText)
    {
        /// <summary>
        /// Build a <see cref="TranslationRequest"/> from the addon options and the
        /// source/target language names. <see cref="Temperature"/> stays null when
        /// the option is unset, so it is omitted from the API request.
        /// </summary>
        public static TranslationRequest From(
            LanguageManagerAnthropicOptions options,
            string fromLanguageName,
            string toLanguageName,
            string text)
        {
            return new TranslationRequest(
                options.AnthropicModel,
                options.AnthropicMaxTokens,
                options.AnthropicTemperature,
                AnthropicSystemPrompt.Build(fromLanguageName, toLanguageName, options.AnthropicExtraPrompt),
                text);
        }
    }
}

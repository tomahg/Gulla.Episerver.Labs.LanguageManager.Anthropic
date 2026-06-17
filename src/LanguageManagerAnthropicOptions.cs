namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    public class LanguageManagerAnthropicOptions
    {
        public string? AnthropicApiKey { get; set; }

        public string AnthropicModel { get; set; } = AnthropicModels.ClaudeHaiku4_5;

        public int AnthropicMaxTokens { get; set; } = 8192;

        /// <summary>
        /// Optional extra instruction appended to the translation system prompt
        /// (e.g. tone or formality). Used when no CMS-configured extra prompt applies.
        /// </summary>
        public string? AnthropicExtraPrompt { get; set; }

        /// <summary>
        /// Optional content id of a CMS page that holds the extra prompt. When set,
        /// <see cref="AnthropicExtraPromptPagePropertyName"/> must also be set. The
        /// value of that property is then used as the extra prompt instead of
        /// <see cref="AnthropicExtraPrompt"/>, so editors can manage it in the CMS.
        /// </summary>
        public int? AnthropicExtraPromptPageContentReference { get; set; }

        /// <summary>
        /// Optional name of the property on the configured page that holds the extra
        /// prompt. When set, <see cref="AnthropicExtraPromptPageContentReference"/>
        /// must also be set.
        /// </summary>
        public string? AnthropicExtraPromptPagePropertyName { get; set; }
    }
}

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    public class LanguageManagerAnthropicOptions
    {
        public string? AnthropicApiKey { get; set; }

        public string AnthropicModel { get; set; } = AnthropicModels.ClaudeHaiku4_5;

        public int AnthropicMaxTokens { get; set; } = 8192;

        /// <summary>
        /// Optional sampling temperature. When null (the default), no temperature is sent.
        /// Note: Opus 4.7/4.8 reject temperature and will return HTTP 400 if a value is set.
        /// Sonnet and Haiku models support it.
        /// </summary>
        public double? AnthropicTemperature { get; set; }

        public string? AnthropicExtraPrompt { get; set; }
    }
}

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    /// <summary>
    /// Identifiers for the Claude models that are currently available for use with this addon.
    /// <para>
    /// <see cref="LanguageManagerAnthropicOptions.AnthropicModel"/> is free text, so any valid model
    /// identifier (including a dated snapshot or a newer model not listed here) can be configured.
    /// These constants are provided for convenience and IntelliSense when configuring from code.
    /// </para>
    /// </summary>
    public static class AnthropicModels
    {
        /// <summary>Claude Opus 4.8 — most capable.</summary>
        public const string ClaudeOpus4_8 = "claude-opus-4-8";

        /// <summary>Claude Opus 4.7.</summary>
        public const string ClaudeOpus4_7 = "claude-opus-4-7";

        /// <summary>Claude Sonnet 4.6 — strong balance of speed and quality.</summary>
        public const string ClaudeSonnet4_6 = "claude-sonnet-4-6";

        /// <summary>Claude Sonnet 4.5.</summary>
        public const string ClaudeSonnet4_5 = "claude-sonnet-4-5";

        /// <summary>
        /// Claude Haiku 4.5 — fastest and most cost-effective.
        /// This is the default model for this addon.
        /// </summary>
        public const string ClaudeHaiku4_5 = "claude-haiku-4-5";
    }
}

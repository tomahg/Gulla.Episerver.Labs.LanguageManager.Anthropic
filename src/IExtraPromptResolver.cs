namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    /// <summary>
    /// Resolves the effective extra prompt for a translation: either a value managed
    /// in the CMS (when a page and property are configured) or the static
    /// <see cref="LanguageManagerAnthropicOptions.AnthropicExtraPrompt"/>.
    /// EPiServer-free so callers stay testable; the CMS-backed implementation lives
    /// behind this seam.
    /// </summary>
    public interface IExtraPromptResolver
    {
        string? Resolve();
    }
}

using System;
using EPiServer;
using EPiServer.Core;
using Microsoft.Extensions.Options;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    /// <summary>
    /// CMS-backed <see cref="IExtraPromptResolver"/>. When both the page reference and
    /// the property name are configured, it reads the extra prompt from that page's
    /// property (untyped, assuming a text property); otherwise it returns the static
    /// <see cref="LanguageManagerAnthropicOptions.AnthropicExtraPrompt"/>. Resolved per
    /// call so editors' changes take effect without redeploy.
    /// </summary>
    public class CmsExtraPromptResolver : IExtraPromptResolver
    {
        private readonly IOptions<LanguageManagerAnthropicOptions> _options;
        private readonly IContentLoader _contentLoader;

        public CmsExtraPromptResolver(
            IOptions<LanguageManagerAnthropicOptions> options,
            IContentLoader contentLoader)
        {
            _options = options;
            _contentLoader = contentLoader;
        }

        public string? Resolve()
        {
            var options = _options.Value;

            var pageId = options.AnthropicExtraPromptPageContentReference;
            var propertyName = options.AnthropicExtraPromptPagePropertyName;

            var hasPage = pageId.HasValue;
            var hasProperty = !string.IsNullOrWhiteSpace(propertyName);

            // Neither configured: use the static extra prompt.
            if (!hasPage && !hasProperty)
            {
                return options.AnthropicExtraPrompt;
            }

            // Half-configured: surface the mistake loudly.
            if (hasPage != hasProperty)
            {
                throw new ArgumentException(
                    "Both AnthropicExtraPromptPageContentReference and AnthropicExtraPromptPagePropertyName must be set together, or neither.");
            }

            // Both configured: read the property value from the page (CMS-only precedence).
            var contentLink = new ContentReference(pageId!.Value);
            if (!_contentLoader.TryGet<IContentData>(contentLink, out var content))
            {
                throw new InvalidOperationException(
                    $"Could not load content '{pageId.Value}' configured for the Anthropic extra prompt.");
            }

            var property = content.Property[propertyName];
            if (property == null)
            {
                throw new InvalidOperationException(
                    $"Property '{propertyName}' was not found on content '{pageId.Value}' configured for the Anthropic extra prompt.");
            }

            // An empty value is allowed (means: no extra instruction).
            return property.Value?.ToString() ?? string.Empty;
        }
    }
}

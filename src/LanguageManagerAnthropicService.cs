using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    /// <summary>
    /// Orchestrator: resolves the effective extra prompt, builds a
    /// <see cref="TranslationRequest"/> from options and the source/target language
    /// names, then sends it via the Anthropic Translation Client. Holds no SDK or CMS
    /// knowledge, so it can be tested with fakes.
    /// </summary>
    public class LanguageManagerAnthropicService
    {
        private readonly IOptions<LanguageManagerAnthropicOptions> _options;
        private readonly IAnthropicTranslationClient _client;
        private readonly IExtraPromptResolver _extraPromptResolver;

        public LanguageManagerAnthropicService(
            IOptions<LanguageManagerAnthropicOptions> options,
            IAnthropicTranslationClient client,
            IExtraPromptResolver extraPromptResolver)
        {
            _options = options;
            _client = client;
            _extraPromptResolver = extraPromptResolver;
        }

        /// <summary>
        /// Translate text from one language to another using Anthropic Claude.
        /// </summary>
        /// <param name="text">The text to translate</param>
        /// <param name="fromLanguageName">The name of the source language</param>
        /// <param name="toLanguageName">The name of the destination language</param>
        public Task<TranslationOutcome> TranslateText(string text, string fromLanguageName, string toLanguageName)
        {
            var extraPrompt = _extraPromptResolver.Resolve();
            var request = TranslationRequest.From(_options.Value, fromLanguageName, toLanguageName, text, extraPrompt);
            return _client.Send(request);
        }
    }
}

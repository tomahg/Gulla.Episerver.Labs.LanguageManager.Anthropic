using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    /// <summary>
    /// Orchestrator: builds a <see cref="TranslationRequest"/> from options and the
    /// source/target language names, then sends it via the Anthropic Translation Client.
    /// Holds no SDK knowledge, so it can be tested with a fake client.
    /// </summary>
    public class LanguageManagerAnthropicService
    {
        private readonly IOptions<LanguageManagerAnthropicOptions> _options;
        private readonly IAnthropicTranslationClient _client;

        public LanguageManagerAnthropicService(
            IOptions<LanguageManagerAnthropicOptions> options,
            IAnthropicTranslationClient client)
        {
            _options = options;
            _client = client;
        }

        /// <summary>
        /// Translate text from one language to another using Anthropic Claude.
        /// </summary>
        /// <param name="text">The text to translate</param>
        /// <param name="fromLanguageName">The name of the source language</param>
        /// <param name="toLanguageName">The name of the destination language</param>
        public Task<TranslationOutcome> TranslateText(string text, string fromLanguageName, string toLanguageName)
        {
            var request = TranslationRequest.From(_options.Value, fromLanguageName, toLanguageName, text);
            return _client.Send(request);
        }
    }
}

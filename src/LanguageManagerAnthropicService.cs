using System.Linq;
using System.Threading.Tasks;
using Anthropic;
using Anthropic.Models.Messages;
using Microsoft.Extensions.Options;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    public class LanguageManagerAnthropicService
    {
        private readonly IOptions<LanguageManagerAnthropicOptions> _options;

        public LanguageManagerAnthropicService(IOptions<LanguageManagerAnthropicOptions> options)
        {
            if (options.Value.AnthropicApiKey == null)
            {
                throw new System.ArgumentException("Missing Anthropic API Key for Gulla.Episerver.Labs.LanguageManager.Anthropic!");
            }

            _options = options;
        }

        /// <summary>
        /// Translate text from one language to another using Anthropic Claude.
        /// </summary>
        /// <param name="text">The text to translate</param>
        /// <param name="fromLanguageName">The name of the source language</param>
        /// <param name="toLanguageName">The name of the destination language</param>
        /// <returns></returns>
        public async Task<string> TranslateText(string text, string fromLanguageName, string toLanguageName)
        {
            var options = _options.Value;

            var systemPrompt = AnthropicSystemPrompt.Build(fromLanguageName, toLanguageName, options.AnthropicExtraPrompt);

            var client = new AnthropicClient { ApiKey = options.AnthropicApiKey };

            var parameters = new MessageCreateParams
            {
                Model = options.AnthropicModel,
                MaxTokens = options.AnthropicMaxTokens,
                System = systemPrompt,
                // Temperature is only sent when set; null is omitted from the request.
                Temperature = options.AnthropicTemperature,
                Messages = [new() { Role = Role.User, Content = text }]
            };

            var response = await client.Messages.Create(parameters);

            var candidateTexts = response.Content
                .Select(block => block.Value)
                .OfType<TextBlock>()
                .Select(block => block.Text);

            return AnthropicResponse.ExtractTranslation(candidateTexts);
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anthropic;
using Anthropic.Models.Messages;
using Microsoft.Extensions.Options;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    /// <summary>
    /// Real <see cref="IAnthropicTranslationClient"/> adapter. Owns all Anthropic SDK
    /// coupling: API key, request mapping, and response extraction.
    /// </summary>
    public class AnthropicTranslationClient : IAnthropicTranslationClient
    {
        private readonly AnthropicClient _client;

        public AnthropicTranslationClient(IOptions<LanguageManagerAnthropicOptions> options)
        {
            var apiKey = options.Value.AnthropicApiKey
                ?? throw new ArgumentException("Missing Anthropic API Key for Gulla.Episerver.Labs.LanguageManager.Anthropic!");

            _client = new AnthropicClient { ApiKey = apiKey };
        }

        public async Task<string> Send(TranslationRequest request, CancellationToken cancellationToken = default)
        {
            var parameters = new MessageCreateParams
            {
                Model = request.Model,
                MaxTokens = request.MaxTokens,
                System = request.SystemPrompt,
                // Temperature is only sent when set; null is omitted from the request.
                Temperature = request.Temperature,
                Messages = [new() { Role = Role.User, Content = request.UserText }]
            };

            var response = await _client.Messages.Create(parameters);

            var candidateTexts = response.Content
                .Select(block => block.Value)
                .OfType<TextBlock>()
                .Select(block => block.Text);

            return AnthropicResponse.ExtractTranslation(candidateTexts);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using Gulla.Episerver.Labs.LanguageManager.Anthropic;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic.Tests
{
    /// <summary>
    /// Test double for <see cref="IAnthropicTranslationClient"/>. Captures the request
    /// it was sent and returns a canned result, so the orchestrator's build-and-send
    /// wiring can be asserted without calling the API.
    /// </summary>
    public sealed class FakeAnthropicTranslationClient : IAnthropicTranslationClient
    {
        private readonly string _result;

        public FakeAnthropicTranslationClient(string result = "translated")
        {
            _result = result;
        }

        public TranslationRequest? LastRequest { get; private set; }

        public Task<string> Send(TranslationRequest request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(_result);
        }
    }
}

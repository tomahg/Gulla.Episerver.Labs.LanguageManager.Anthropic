using System.Threading;
using System.Threading.Tasks;
using Gulla.Episerver.Labs.LanguageManager.Anthropic;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic.Tests
{
    /// <summary>
    /// Test double for <see cref="IAnthropicTranslationClient"/>. Captures the request
    /// it was sent and returns a canned outcome, so the orchestrator's build-and-send
    /// wiring can be asserted without calling the API.
    /// </summary>
    public sealed class FakeAnthropicTranslationClient : IAnthropicTranslationClient
    {
        private readonly TranslationOutcome _result;

        public FakeAnthropicTranslationClient(TranslationOutcome? result = null)
        {
            _result = result ?? TranslationOutcome.Success("translated");
        }

        public TranslationRequest? LastRequest { get; private set; }

        public Task<TranslationOutcome> Send(TranslationRequest request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(_result);
        }
    }
}

using Gulla.Episerver.Labs.LanguageManager.Anthropic;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic.Tests
{
    /// <summary>
    /// Test double for <see cref="IExtraPromptResolver"/> returning a canned value,
    /// so the orchestrator can be tested without the CMS.
    /// </summary>
    public sealed class FakeExtraPromptResolver : IExtraPromptResolver
    {
        private readonly string? _value;

        public FakeExtraPromptResolver(string? value = null)
        {
            _value = value;
        }

        public string? Resolve() => _value;
    }
}

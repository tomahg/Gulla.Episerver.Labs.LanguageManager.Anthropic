using Xunit;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic.Tests
{
    public class TranslationRequestTests
    {
        private static LanguageManagerAnthropicOptions Options() => new()
        {
            AnthropicApiKey = "key",
            AnthropicModel = AnthropicModels.ClaudeSonnet4_6,
            AnthropicMaxTokens = 1234,
        };

        [Fact]
        public void From_CarriesModelAndMaxTokens()
        {
            var req = TranslationRequest.From(Options(), "English", "Norwegian", "hello", null);

            Assert.Equal(AnthropicModels.ClaudeSonnet4_6, req.Model);
            Assert.Equal(1234, req.MaxTokens);
        }

        [Fact]
        public void From_CarriesUserTextVerbatim()
        {
            var req = TranslationRequest.From(Options(), "English", "Norwegian", "hello world", null);

            Assert.Equal("hello world", req.UserText);
        }

        [Fact]
        public void From_BuildsSystemPromptFromLanguagesAndResolvedExtraPrompt()
        {
            var req = TranslationRequest.From(Options(), "English", "Norwegian", "hi", "Make it formal.");

            Assert.Equal(
                AnthropicSystemPrompt.Build("English", "Norwegian", "Make it formal."),
                req.SystemPrompt);
        }

        [Fact]
        public void From_UsesResolvedExtraPrompt_NotOptionsExtraPrompt()
        {
            var options = Options();
            options.AnthropicExtraPrompt = "static (should be ignored here)";

            var req = TranslationRequest.From(options, "English", "Norwegian", "hi", "resolved");

            Assert.Equal(
                AnthropicSystemPrompt.Build("English", "Norwegian", "resolved"),
                req.SystemPrompt);
        }
    }
}

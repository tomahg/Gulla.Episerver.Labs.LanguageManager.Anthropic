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
            var req = TranslationRequest.From(Options(), "English", "Norwegian", "hello");

            Assert.Equal(AnthropicModels.ClaudeSonnet4_6, req.Model);
            Assert.Equal(1234, req.MaxTokens);
        }

        [Fact]
        public void From_CarriesUserTextVerbatim()
        {
            var req = TranslationRequest.From(Options(), "English", "Norwegian", "hello world");

            Assert.Equal("hello world", req.UserText);
        }

        [Fact]
        public void From_LeavesTemperatureNull_WhenOptionUnset()
        {
            var req = TranslationRequest.From(Options(), "English", "Norwegian", "hi");

            Assert.Null(req.Temperature);
        }

        [Fact]
        public void From_PassesTemperatureThrough_WhenOptionSet()
        {
            var options = Options();
            options.AnthropicTemperature = 0.5;

            var req = TranslationRequest.From(options, "English", "Norwegian", "hi");

            Assert.Equal(0.5, req.Temperature);
        }

        [Fact]
        public void From_BuildsSystemPromptFromLanguagesAndExtraPrompt()
        {
            var options = Options();
            options.AnthropicExtraPrompt = "Make it formal.";

            var req = TranslationRequest.From(options, "English", "Norwegian", "hi");

            Assert.Equal(
                AnthropicSystemPrompt.Build("English", "Norwegian", "Make it formal."),
                req.SystemPrompt);
        }
    }
}

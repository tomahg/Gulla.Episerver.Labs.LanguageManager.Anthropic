using Xunit;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic.Tests
{
    public class AnthropicResponseTests
    {
        [Fact]
        public void ExtractTranslation_ReturnsSuccessWithFirstText_WhenPresent()
        {
            var outcome = AnthropicResponse.ExtractTranslation(new[] { "Bonjour" });

            Assert.True(outcome.IsSuccess);
            Assert.Equal("Bonjour", outcome.Text);
        }

        [Fact]
        public void ExtractTranslation_SkipsBlankBlocks_AndReturnsFirstNonEmpty()
        {
            var outcome = AnthropicResponse.ExtractTranslation(new[] { null, "", "   ", "Hola" });

            Assert.True(outcome.IsSuccess);
            Assert.Equal("Hola", outcome.Text);
        }

        [Fact]
        public void ExtractTranslation_ReturnsFailure_WhenNoBlocks()
        {
            var outcome = AnthropicResponse.ExtractTranslation(new string?[0]);

            Assert.False(outcome.IsSuccess);
            Assert.Equal(AnthropicResponse.NoTranslationMessage, outcome.Text);
        }

        [Fact]
        public void ExtractTranslation_ReturnsFailure_WhenAllBlocksBlank()
        {
            var outcome = AnthropicResponse.ExtractTranslation(new[] { null, "", "   " });

            Assert.False(outcome.IsSuccess);
            Assert.Equal(AnthropicResponse.NoTranslationMessage, outcome.Text);
        }
    }
}

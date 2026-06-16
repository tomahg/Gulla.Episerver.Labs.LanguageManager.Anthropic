using Xunit;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic.Tests
{
    public class AnthropicResponseTests
    {
        [Fact]
        public void ExtractTranslation_ReturnsFirstText_WhenPresent()
        {
            var result = AnthropicResponse.ExtractTranslation(new[] { "Bonjour" });

            Assert.Equal("Bonjour", result);
        }

        [Fact]
        public void ExtractTranslation_SkipsBlankBlocks_AndReturnsFirstNonEmpty()
        {
            var result = AnthropicResponse.ExtractTranslation(new[] { null, "", "   ", "Hola" });

            Assert.Equal("Hola", result);
        }

        [Fact]
        public void ExtractTranslation_ReturnsError_WhenNoBlocks()
        {
            var result = AnthropicResponse.ExtractTranslation(new string?[0]);

            Assert.Equal(AnthropicResponse.ErrorMessage, result);
        }

        [Fact]
        public void ExtractTranslation_ReturnsError_WhenAllBlocksBlank()
        {
            var result = AnthropicResponse.ExtractTranslation(new[] { null, "", "   " });

            Assert.Equal(AnthropicResponse.ErrorMessage, result);
        }
    }
}

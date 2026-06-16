using Xunit;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic.Tests
{
    public class AnthropicSystemPromptTests
    {
        [Fact]
        public void Build_IncludesSourceAndTargetLanguage()
        {
            var prompt = AnthropicSystemPrompt.Build("English", "Norwegian", null);

            Assert.Contains("from English to Norwegian", prompt);
        }

        [Fact]
        public void Build_AlwaysEndsWithPreambleGuard()
        {
            var prompt = AnthropicSystemPrompt.Build("English", "Norwegian", null);

            Assert.EndsWith(AnthropicSystemPrompt.PreambleGuard, prompt);
        }

        [Fact]
        public void Build_AppendsExtraPrompt_WhenProvided()
        {
            var prompt = AnthropicSystemPrompt.Build("English", "Norwegian", "Make it formal.");

            Assert.Contains("Make it formal.", prompt);
            // Extra prompt sits between the instruction and the preamble guard.
            Assert.EndsWith("Make it formal. " + AnthropicSystemPrompt.PreambleGuard, prompt);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Build_OmitsExtraPrompt_WhenNullOrEmpty(string? extraPrompt)
        {
            var prompt = AnthropicSystemPrompt.Build("English", "Norwegian", extraPrompt);

            Assert.Equal(
                "You are a translator. Translate from English to Norwegian. " + AnthropicSystemPrompt.PreambleGuard,
                prompt);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("Make it formal.")]
        public void Build_HasNoDoubleSpaces(string? extraPrompt)
        {
            var prompt = AnthropicSystemPrompt.Build("English", "Norwegian", extraPrompt);

            Assert.DoesNotContain("  ", prompt);
        }
    }
}

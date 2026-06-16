using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Xunit;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic.Tests
{
    public class LanguageManagerAnthropicServiceTests
    {
        private static LanguageManagerAnthropicService Service(
            LanguageManagerAnthropicOptions options,
            IAnthropicTranslationClient client)
            => new(Options.Create(options), client);

        [Fact]
        public async Task TranslateText_SendsRequestBuiltFromOptionsAndLanguages()
        {
            var options = new LanguageManagerAnthropicOptions
            {
                AnthropicApiKey = "key",
                AnthropicModel = AnthropicModels.ClaudeHaiku4_5,
                AnthropicMaxTokens = 4096,
                AnthropicExtraPrompt = "Keep it short.",
            };
            var fake = new FakeAnthropicTranslationClient();

            await Service(options, fake).TranslateText("hello", "English", "Norwegian");

            Assert.NotNull(fake.LastRequest);
            Assert.Equal("hello", fake.LastRequest!.UserText);
            Assert.Equal(AnthropicModels.ClaudeHaiku4_5, fake.LastRequest.Model);
            Assert.Equal(4096, fake.LastRequest.MaxTokens);
            Assert.Null(fake.LastRequest.Temperature);
            Assert.Equal(
                AnthropicSystemPrompt.Build("English", "Norwegian", "Keep it short."),
                fake.LastRequest.SystemPrompt);
        }

        [Fact]
        public async Task TranslateText_ReturnsClientResult()
        {
            var options = new LanguageManagerAnthropicOptions { AnthropicApiKey = "key" };
            var fake = new FakeAnthropicTranslationClient("Hei");

            var result = await Service(options, fake).TranslateText("Hello", "English", "Norwegian");

            Assert.Equal("Hei", result);
        }
    }
}

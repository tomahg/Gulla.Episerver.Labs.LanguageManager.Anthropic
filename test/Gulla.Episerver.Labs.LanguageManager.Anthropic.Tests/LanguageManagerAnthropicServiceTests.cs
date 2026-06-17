using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Xunit;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic.Tests
{
    public class LanguageManagerAnthropicServiceTests
    {
        private static LanguageManagerAnthropicService Service(
            LanguageManagerAnthropicOptions options,
            IAnthropicTranslationClient client,
            IExtraPromptResolver resolver)
            => new(Options.Create(options), client, resolver);

        [Fact]
        public async Task TranslateText_SendsRequestBuiltFromOptionsAndResolvedExtraPrompt()
        {
            var options = new LanguageManagerAnthropicOptions
            {
                AnthropicApiKey = "key",
                AnthropicModel = AnthropicModels.ClaudeHaiku4_5,
                AnthropicMaxTokens = 4096,
            };
            var fakeClient = new FakeAnthropicTranslationClient();
            var resolver = new FakeExtraPromptResolver("Keep it short.");

            await Service(options, fakeClient, resolver).TranslateText("hello", "English", "Norwegian");

            Assert.NotNull(fakeClient.LastRequest);
            Assert.Equal("hello", fakeClient.LastRequest!.UserText);
            Assert.Equal(AnthropicModels.ClaudeHaiku4_5, fakeClient.LastRequest.Model);
            Assert.Equal(4096, fakeClient.LastRequest.MaxTokens);
            Assert.Equal(
                AnthropicSystemPrompt.Build("English", "Norwegian", "Keep it short."),
                fakeClient.LastRequest.SystemPrompt);
        }

        [Fact]
        public async Task TranslateText_ReturnsClientOutcome()
        {
            var options = new LanguageManagerAnthropicOptions { AnthropicApiKey = "key" };
            var fakeClient = new FakeAnthropicTranslationClient(TranslationOutcome.Success("Hei"));
            var resolver = new FakeExtraPromptResolver();

            var outcome = await Service(options, fakeClient, resolver).TranslateText("Hello", "English", "Norwegian");

            Assert.True(outcome.IsSuccess);
            Assert.Equal("Hei", outcome.Text);
        }
    }
}

using System;
using System.Globalization;
using System.Threading.Tasks;
using EPiServer.Labs.LanguageManager.Business.Providers;
using EPiServer.Labs.LanguageManager.Models;
using EPiServer.ServiceLocation;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    public class AnthropicTranslatorProvider : IMachineTranslatorProvider
    {
        private Injected<LanguageManagerAnthropicService> _anthropicService;

        public string DisplayName => "Anthropic Translator";

        public bool Initialize(ITranslatorProviderConfig config)
        {
            return true;
        }

        public async Task<TranslateTextResult> TranslateAsync(string inputText, string fromLang, string toLang)
        {
            var translateTextResult = new TranslateTextResult()
            {
                IsSuccess = true,
                Text = ""
            };

            if (string.IsNullOrWhiteSpace(inputText))
            {
                return translateTextResult;
            }

            try
            {
                var fromLanguageName = new CultureInfo(fromLang).DisplayName;
                var toLanguageName = new CultureInfo(toLang).DisplayName;
                var outcome = await _anthropicService.Service.TranslateText(inputText, fromLanguageName, toLanguageName);
                translateTextResult.IsSuccess = outcome.IsSuccess;
                translateTextResult.Text = outcome.Text;
            }
            catch (Exception e)
            {
                translateTextResult.IsSuccess = false;
                translateTextResult.Text = "An unexpected error occured: " + e.Message;
            }

            return translateTextResult;
        }
    }
}

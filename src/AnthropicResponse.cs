using System.Collections.Generic;
using System.Linq;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    /// <summary>
    /// Extracts the translated text from an Anthropic response.
    /// Pure and dependency-free so it can be unit-tested in isolation.
    /// </summary>
    public static class AnthropicResponse
    {
        /// <summary>
        /// Returned when the response contains no usable translated text.
        /// </summary>
        public const string ErrorMessage = "Error translating text";

        /// <summary>
        /// Return the first non-empty text from the response's text blocks,
        /// or <see cref="ErrorMessage"/> when none is present.
        /// </summary>
        /// <param name="candidateTexts">The text of each text block in the response, in order.</param>
        public static string ExtractTranslation(IEnumerable<string?> candidateTexts)
        {
            var translatedText = candidateTexts.FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));
            return translatedText ?? ErrorMessage;
        }
    }
}

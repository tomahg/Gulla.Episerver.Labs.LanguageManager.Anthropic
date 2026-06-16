using System.Collections.Generic;
using System.Linq;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    /// <summary>
    /// Decides the <see cref="TranslationOutcome"/> from an Anthropic response.
    /// This is the single place that decides what counts as a failed translation.
    /// Pure and dependency-free so it can be unit-tested in isolation.
    /// </summary>
    public static class AnthropicResponse
    {
        /// <summary>
        /// Failure message used when the response contains no usable translated text.
        /// </summary>
        public const string NoTranslationMessage = "No translation was returned.";

        /// <summary>
        /// Return a successful outcome with the first non-empty text from the response's
        /// text blocks, or a failure outcome when none is present.
        /// </summary>
        /// <param name="candidateTexts">The text of each text block in the response, in order.</param>
        public static TranslationOutcome ExtractTranslation(IEnumerable<string?> candidateTexts)
        {
            var translatedText = candidateTexts.FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));

            return translatedText is null
                ? TranslationOutcome.Failure(NoTranslationMessage)
                : TranslationOutcome.Success(translatedText);
        }
    }
}

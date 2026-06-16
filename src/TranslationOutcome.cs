namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    /// <summary>
    /// The result of a translation turn: either the translated text, or a failure
    /// with a message. Makes "no usable translation" an explicit, honest state
    /// instead of a sentinel string treated as success.
    /// </summary>
    public sealed record TranslationOutcome(bool IsSuccess, string Text)
    {
        public static TranslationOutcome Success(string text) => new(true, text);

        public static TranslationOutcome Failure(string message) => new(false, message);
    }
}

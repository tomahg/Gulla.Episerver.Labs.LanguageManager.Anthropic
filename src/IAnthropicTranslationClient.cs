using System.Threading;
using System.Threading.Tasks;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    /// <summary>
    /// Seam over the Anthropic Messages API: sends a <see cref="TranslationRequest"/>
    /// to Claude and returns the translated text. The real implementation wraps the
    /// Anthropic SDK; tests substitute a fake.
    /// </summary>
    public interface IAnthropicTranslationClient
    {
        Task<TranslationOutcome> Send(TranslationRequest request, CancellationToken cancellationToken = default);
    }
}

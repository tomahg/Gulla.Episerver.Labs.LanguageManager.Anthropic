# PRD: Anthropic Claude translator provider for EPiServer.Labs.LanguageManager

> Status: ready-for-agent
> Note: published as a file because no issue tracker / triage-label vocabulary is configured in this workspace. Move to the tracker and apply the `ready-for-agent` label when available.

## Problem Statement

Optimizely CMS 12 editors use the EPiServer.Labs.LanguageManager add-on to auto-translate content between languages. Today there is a sibling add-on that performs these translations using OpenAI (`Gulla.Episerver.Labs.LanguageManager.OpenAi`). Teams that have standardized on Anthropic Claude — for procurement, data-processing, quality, or cost reasons — have no equivalent option, and cannot use their existing Anthropic API key or preferred Claude models for in-CMS translation.

## Solution

A new NuGet add-on, `Gulla.Episerver.Labs.LanguageManager.Anthropic`, that registers an Anthropic-backed `IMachineTranslatorProvider` with LanguageManager. It behaves exactly like the OpenAI add-on from the editor's perspective — appearing as a selectable translator provider ("Anthropic Translator") in the Languages gadget — but routes translation requests to the Anthropic Messages API via the official Anthropic .NET SDK. Developers configure it with a single `.AddLanguageManagerAnthropic(...)` call and/or `appsettings.json`, choosing the Claude model, output limit, optional temperature, and an optional extra instruction.

## User Stories

1. As a developer, I want to install the add-on from NuGet, so that I can add Anthropic-backed translation to my Optimizely CMS 12 site.
2. As a developer, I want to register the provider with a single `AddLanguageManagerAnthropic()` extension call in `ConfigureServices`, so that wiring is minimal and consistent with the OpenAI add-on.
3. As a developer, I want to supply my Anthropic API key in `Startup` code, so that I can configure the add-on programmatically.
4. As a developer, I want to supply configuration via `appsettings.json` under `Gulla:LanguageManagerAnthropic`, so that I can keep secrets and settings out of code.
5. As a developer, I want `appsettings.json` values to override values set in `Startup`, so that environment-specific overrides behave predictably (matching the OpenAI add-on).
6. As a developer, I want the API key to be the only required setting, so that I can get started with sensible defaults.
7. As a developer, I want a clear error if I forget to configure the API key, so that misconfiguration fails fast and obviously.
8. As a developer, I want to choose which Claude model is used, so that I can balance quality, speed, and cost for my use case.
9. As a developer, I want a default model that is fast and cost-effective (`claude-haiku-4-5`), so that translation is cheap and responsive out of the box.
10. As a developer, I want strongly-typed constants for the currently available models, so that I get IntelliSense and avoid typos when configuring from code.
11. As a developer, I want the model setting to remain free text, so that I can pin a dated snapshot or adopt a newly released model that isn't yet in the constants.
12. As a developer, I want the model constants to exclude deprecated/retired models, so that I am not steered toward models that will stop working.
13. As a developer, I want to set an optional temperature, so that I can tune translation determinism/variance where the model supports it.
14. As a developer, I want temperature to be omitted from the request when I don't set it, so that the default (Haiku) and temperature-incompatible models (Opus 4.7/4.8) work without errors.
15. As a developer, I want documentation to warn me that Opus 4.7/4.8 reject temperature, so that I don't accidentally cause HTTP 400 errors.
16. As a developer, I want to configure the maximum output tokens, so that I can accommodate longer content when needed.
17. As a developer, I want a sensible default output limit (8192), so that typical CMS field translations never truncate while staying within the non-streaming timeout comfort zone.
18. As a developer, I want to add an extra instruction to the prompt (e.g. tone/formality), so that translations match my editorial voice.
19. As a content editor, I want "Anthropic Translator" to appear in LanguageManager's translator-provider picker, so that I can select it as the translation engine.
20. As a content editor, I want to auto-translate content via the Languages gadget, so that I can produce localized content quickly.
21. As a content editor, I want empty/whitespace input to return successfully with empty output, so that blank fields don't produce errors or spurious text.
22. As a content editor, I want translation output to contain only the translated text, so that no "Here is the translation:" preamble leaks into my content.
23. As a content editor, I want the source and target language to be recognized from culture codes, so that translation targets the correct language.
24. As a content editor, I want translation failures to surface as a non-successful result with a readable message, so that I understand something went wrong instead of getting silent or corrupt content.
25. As a maintainer, I want the add-on to use the official Anthropic .NET SDK, so that API/version/header concerns are handled and maintained upstream.
26. As a maintainer, I want the translation prompt-building logic isolated in a pure, dependency-free unit, so that I can unit-test it without network calls.
27. As a maintainer, I want the response-extraction logic isolated in a pure unit, so that I can unit-test parsing and the error fallback without network calls.
28. As a maintainer, I want the package metadata (id, description, tags, repo URL, MIT license) to mirror the OpenAI add-on conventions, so that the sibling packages are consistent.
29. As a maintainer, I want the project to target .NET 10, so that the add-on is on the current framework.

## Implementation Decisions

**Integration seam**
- Implement `IMachineTranslatorProvider` from EPiServer.Labs.LanguageManager. `DisplayName` is `"Anthropic Translator"`. `Initialize(...)` returns `true`. `Translate(inputText, fromLang, toLang)` returns a `TranslateTextResult`.
- The provider is a thin adapter: it resolves the translation service via `Injected<>`, maps `fromLang`/`toLang` culture codes to display names via `CultureInfo`, calls the service synchronously (`.Result`, matching the synchronous provider contract and the OpenAI add-on), and wraps everything in try/catch that sets `IsSuccess = false` and a readable message on failure. Whitespace-only input short-circuits to a successful empty result.

**API access**
- Use the official Anthropic .NET SDK (`Anthropic` NuGet package), referenced with a floating range `[12.29.0, 13.0.0)`. The SDK manages the `anthropic-version` header and transport.
- Call the Messages API (`client.Messages.Create`) in non-streaming mode (single request/response), consistent with the synchronous provider contract.

**Deep modules to extract (testable in isolation)**
- **Prompt builder** — a pure static function that builds the system prompt from `(fromLanguageName, toLanguageName, extraPrompt)`. It always appends the preamble guard. Decision-encoding shape (from the implemented prototype):
  - Base: `You are a translator. Translate from {from} to {to}.`
  - If extra prompt is non-empty: append ` {extraPrompt}`
  - Always append: ` Output only the translation, with no preamble or explanation.`
- **Response extractor** — a pure static function that takes the response content blocks and returns the first text block's text, or the literal fallback `"Error translating text"` when no non-empty text is present.
- **`LanguageManagerAnthropicService`** becomes a thin shell: build the system prompt (via the prompt builder), construct `MessageCreateParams`, call the SDK, and run the result through the response extractor.

**Request construction**
- System parameter carries the translation instruction (prompt builder output); the user message carries the raw text to translate. This system/user split (vs. the OpenAI add-on's single concatenated user message) improves translation fidelity and suppresses preamble.
- `MaxTokens` comes from configuration (default 8192).
- `Temperature` is sent only when configured (nullable; null is omitted from the request). This prevents HTTP 400 on temperature-incompatible models when temperature is left unset.
- `Model` comes from configuration (default `claude-haiku-4-5`).

**Configuration**
- Options type `LanguageManagerAnthropicOptions`:
  - `AnthropicApiKey` (string, required)
  - `AnthropicModel` (string, default `claude-haiku-4-5`)
  - `AnthropicMaxTokens` (int, default 8192)
  - `AnthropicTemperature` (nullable double, default null → omitted)
  - `AnthropicExtraPrompt` (nullable string)
- DI extension `AddLanguageManagerAnthropic(...)` (with and without a setup action) registers the service as transient and binds options from the `Gulla:LanguageManagerAnthropic` configuration section after applying the code-based setup action, so `appsettings.json` overrides `Startup` values.
- Missing API key throws `ArgumentException` from the service constructor (fail fast), mirroring the OpenAI add-on.

**Model constants**
- `AnthropicModels` static class exposes string constants for the currently available, non-deprecated models, two most-recent per family where applicable:
  - `ClaudeOpus4_8`, `ClaudeOpus4_7` (temperature-incompatible — documented)
  - `ClaudeSonnet4_6`, `ClaudeSonnet4_5`
  - `ClaudeHaiku4_5` (default)
  - Fable 5 and Mythos 5 are intentionally excluded (not generally available for this use case); Haiku has a single current entry because its predecessors are retired.
- `AnthropicModel` remains free text so any other valid identifier can be used.

**Packaging**
- Target framework `net10.0` (the OpenAI add-on uses `net8.0`); `Nullable` enabled; `Library` output.
- Same EPiServer package ranges as the OpenAI add-on: `EPiServer.Labs.LanguageManager [5.0.0, 6.0.0)`, `EPiServer.CMS.UI [12.0.2, 13.0.0)`.
- Package id/namespace `Gulla.Episerver.Labs.LanguageManager.Anthropic`, version `1.0.0`, MIT license, tags and description mirroring the OpenAI add-on.

**Open verification items (not blockers)**
- Confirm `MessageCreateParams.Model` accepts the configured model string directly (implicit conversion) vs. requiring an explicit wrapper.
- Confirm CMS 12 package set restores cleanly on `net10.0`.

## Testing Decisions

**What makes a good test here:** tests assert externally observable behavior of the pure modules — given inputs, the produced string / extracted value — not internal implementation details, SDK internals, or network behavior. No live API calls.

**Modules under test:**
- **Prompt builder** — cases: correct `from`/`to` interpolation; extra prompt appended with a single separating space when present and omitted when null/empty; the preamble guard (`Output only the translation, with no preamble or explanation.`) is always present and last; no double spaces.
- **Response extractor** — cases: returns the first text block's text when present; returns `"Error translating text"` when there are no text blocks or the text is empty/whitespace.

**Not unit-tested:** `AnthropicTranslatorProvider` (adapter over `Injected<>`/`.Result`/`CultureInfo`), `ServiceCollectionExtensions` (DI/config binding), options POCO, and constants — these are integration-level or declarative.

**Prior art:** the sibling OpenAI add-on ships no tests, so a new test project is introduced for this add-on; keep it minimal and focused on the two pure modules.

## Out of Scope

- Streaming responses (the provider contract is synchronous; non-streaming single request is sufficient for field-level translation).
- Prompt caching, batching, retries/backoff beyond SDK defaults, and token-usage reporting.
- Glossary/terminology management, translation memory, or per-field prompt customization beyond the single extra-prompt setting.
- UI/gadget changes in LanguageManager itself.
- Supporting models that are deprecated/retired, or restricted-access models (Fable 5, Mythos 5).
- Automatic model-specific validation (e.g. blocking temperature on Opus 4.7/4.8); this is documented rather than enforced, so the setting stays simple and forward-compatible.
- Screenshots/images referenced by the README (to be supplied separately).

## Further Notes

- The add-on intentionally diverges structurally from the OpenAI sibling (official SDK instead of hand-rolled `HttpClient`/Newtonsoft; system+user prompt split; nullable omit-when-unset temperature; required `max_tokens` option), while preserving the same configuration ergonomics and provider seam.
- Temperature support is model-dependent: Opus 4.7/4.8 reject it (HTTP 400); Sonnet and Haiku accept it. The omit-when-null design plus README guidance keeps the default and Opus paths working without enforcement code.
- `max_tokens` is a ceiling, not a target; billing is per generated token. The 8192 default gives generous headroom for field-level content while staying under the non-streaming HTTP-timeout zone (~16K).

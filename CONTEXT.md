# Domain Context

Vocabulary for the Anthropic translator add-on. Use these terms in code and discussion.

## Translator Provider
The seam LanguageManager exposes for machine translation (`IMachineTranslatorProvider`). Our adapter into it is **`AnthropicTranslatorProvider`** — it maps culture codes to language names, calls the Orchestrator, and shapes the `TranslateTextResult` LanguageManager expects. It is instantiated by LanguageManager (not DI), so it reaches the Orchestrator via `Injected<>`.

## Translation Request
A provider-agnostic value describing one translation turn: model, max tokens, system prompt, and the user text. Represented by the **`TranslationRequest`** record and built purely via `TranslationRequest.From(options, fromLanguageName, toLanguageName, text, extraPrompt)`. The caller passes in the already-resolved extra prompt (from the Extra Prompt Resolver); this is where options + language names + extra prompt are turned into a concrete request.

## Extra Prompt Resolver
**`IExtraPromptResolver`** — resolves the effective extra prompt for a translation. The CMS-backed **`CmsExtraPromptResolver`** implements the precedence: when both `AnthropicExtraPromptPageContentReference` (a content id) and `AnthropicExtraPromptPagePropertyName` are configured, it loads that page via `IContentLoader` and reads the named property (untyped, assuming a text property) — the **CMS-only** rule, where the page value wins (empty allowed). When neither is configured, it returns the static `AnthropicExtraPrompt`. Exactly one of the two configured, a page that can't be loaded, or a property that doesn't exist all throw — surfaced as a failed translation via the provider's `try/catch`. Resolved per call, so editor changes apply without redeploy. EPiServer coupling lives only here.

## Anthropic Translation Client (the seam)
**`IAnthropicTranslationClient`** — sends a Translation Request to Claude and returns the translated text. The real adapter **`AnthropicTranslationClient`** wraps the Anthropic SDK (it owns all SDK coupling: API key, `MessageCreateParams`, content-block extraction). **`FakeAnthropicTranslationClient`** stands in for it in tests. This is the only place the SDK is touched.

## Orchestrator
**`LanguageManagerAnthropicService`** — builds a Translation Request from options + language names and sends it via the Anthropic Translation Client. Holds no SDK knowledge; testable with a fake client.

## System Prompt builder
**`AnthropicSystemPrompt`** — pure construction of the translation system prompt, including the preamble guard.

## Translation Outcome
**`TranslationOutcome`** — the result of a translation turn: either the translated text (`Success`) or a failure message (`Failure`). The single honest representation of "no usable translation," replacing a sentinel string treated as success. The seam and Orchestrator return it; the Translator Provider maps it to `TranslateTextResult`. Infrastructure errors (network, auth, missing key) remain a separate channel via the provider's `try/catch`.

## Response extraction
**`AnthropicResponse`** — pure decision of the Translation Outcome from the response's text blocks; the single place that decides what counts as a failed translation (used inside the real Anthropic Translation Client).

## Model identifiers
**`AnthropicModels`** — constants for currently available Claude models; `AnthropicModel` configuration remains free text.

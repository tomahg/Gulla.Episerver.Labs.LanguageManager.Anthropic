# Domain Context

Vocabulary for the Anthropic translator add-on. Use these terms in code and discussion.

## Translator Provider
The seam LanguageManager exposes for machine translation (`IMachineTranslatorProvider`). Our adapter into it is **`AnthropicTranslatorProvider`** — it maps culture codes to language names, calls the Orchestrator, and shapes the `TranslateTextResult` LanguageManager expects. It is instantiated by LanguageManager (not DI), so it reaches the Orchestrator via `Injected<>`.

## Translation Request
A provider-agnostic value describing one translation turn: model, max tokens, optional temperature, system prompt, and the user text. Represented by the **`TranslationRequest`** record and built purely via `TranslationRequest.From(options, fromLanguageName, toLanguageName, text)`. This is where options + language names are turned into a concrete request.

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

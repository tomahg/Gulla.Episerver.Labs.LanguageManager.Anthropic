# Gulla.Episerver.Labs.LanguageManager.Anthropic 🤖

This addon for Optimizely CMS enables EPiServer.Labs.LanguageManager to auto-translate content using Anthropic Claude.

## Installation

The command below will install the addon in your Optimizely project.

```
dotnet add package Gulla.Episerver.Labs.LanguageManager.Anthropic
```

## Configuration

For this addon to work, you will have to call the `.AddLanguageManagerAnthropic()` extension method in the Startup.ConfigureServices method.

Below is a code snippet with all possible configuration options. The only required configuration is `AnthropicApiKey`.

```csharp
.AddLanguageManagerAnthropic(x => {
    x.AnthropicApiKey = "**********";
    x.AnthropicModel = AnthropicModels.ClaudeHaiku4_5;
    x.AnthropicMaxTokens = 8192;
    x.AnthropicTemperature = 1.0;
    x.AnthropicExtraPrompt = "Make the translation super, super formal.";
})
```

The default values are

-   AnthropicModel = "claude-haiku-4-5"
-   AnthropicMaxTokens = 8192
-   AnthropicTemperature = _not set_ (no temperature is sent unless you set one)

`AnthropicModel` accepts any valid model identifier as free text. For convenience, the `AnthropicModels` class exposes constants for the currently available models:

-   `AnthropicModels.ClaudeOpus4_8` — most capable (does **not** support `AnthropicTemperature`)
-   `AnthropicModels.ClaudeOpus4_7` — (does **not** support `AnthropicTemperature`)
-   `AnthropicModels.ClaudeSonnet4_6`
-   `AnthropicModels.ClaudeSonnet4_5`
-   `AnthropicModels.ClaudeHaiku4_5` — fastest and most cost-effective (default)

> ⚠️ Setting `AnthropicTemperature` together with an Opus 4.7/4.8 model will cause the API to return an error, as those models do not accept a temperature parameter. Leave `AnthropicTemperature` unset for those models, or use a Sonnet/Haiku model.

You can also configure this addon using `appsettings.json`. A configuration setting specified in `appsettings.json` will override any configuration configured in `Startup.cs`. See the example below:

```JSON
  "Gulla": {
    "LanguageManagerAnthropic": {
      "AnthropicApiKey": "**********",
      "AnthropicModel": "claude-haiku-4-5",
      "AnthropicMaxTokens": 8192,
      "AnthropicTemperature": 1.0,
      "AnthropicExtraPrompt": "Make the translation super, super formal."
    }
  }
```

In order for LanguageManager to use this Anthropic Provider, configure it like this.
![Configure translator provider](img/translator-provider.png)

## Usage

Add the Languages gadget, and auto-translate all you want!
![Auto-translate](img/auto-translate.png)

## Contribute

You are welcome to register an issue or create a pull request if you see something that should be improved.

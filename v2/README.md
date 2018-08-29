## V2

This is Sample project for Azure Functions v2 attribute style, precompiled.

## Getting Started

### Configure Azure Environment

Use `~/Terraform/v2` within Cloud Shell

[![Launch Cloud Shell](https://shell.azure.com/images/launchcloudshell.png "Launch Cloud Shell")](https://shell.azure.com)


### Sync remote environment variables

install azure functions cli.

```
npm i -g azure-functions-cli
```

sync

```powershell
cd src/AzureFunctionsIntroduction
func azure functionapp fetch-app-settings function-v2-function
Copy-Item ./appsettings.json local.settings.json -Force
```

## Build

use Dockerfile to build .NET Core 2.0 dll.

```
docker build -t azurefunctionsintroduction .
```

## Samples

This sample include following.

FunctionName | Language | Pre-compiled? | Description
---- | ---- | ----  | ----
AppSettingsWebhookCSharp | C# | Yes | Reference ```Application Settings > App Setting``` of Web Apps Sample code.
CSharpCompilerSlackOuthookCSharp | C# | Yes | Slack C# Code Roslyn Evaluation Sample. (```@C#: Enumerable.Range(10, 20).Aggregate((x, y) => x + y)```)
CSharpCompilerWebhookCSharp | C# | Yes | Generic Webhook C# Code Roslyn Evaluation Sample.
EventGridWebhookCSharp | C# | Yes | Webhook triggered by EventGrid event.
GenericWebhookCSharpExtensionMethod | C# | Yes | Extension Method usage Sample code.
GithubMergedBranchSweepTimer | C# | Yes | Delete branches you forgot to delete after merged. Run by cron. Handle parameter with Environment Variables.
GithubMergedBranchSweepTrigger | C# | Yes | Delete branches you forgot to delete after merged. Run by Http Post Request.Handle parameter with json body.
GithubWebhookCSharp | C# | Yes | Github Webhook Sample code.
KeyVaultWebhookCSharp | C# | Yes | KeyVault Secret read and use.
LineBotWebhookCSharp | C# | Yes | Line Bot Webhook Sample code with Emergency Evacuation info with sent info.
SSLCertificateExpireCheck | C# | Yes | SSL Certificate Checker. Often introduce in AWS Lambda but you can do with C# + AzureFucntions, too! 
VSTSWebhookCSharp | C# | Yes | Visual Studio Team Service (VSTS) Webhook trigger Sample code.
WebhookCSharpGithubOctokit | C# | Yes | NuGet package reference sample for Octokit.
WebhookCSharpSendToChatWork | C# | Yes | Chatwork Notification Sample code.
WebhookCSharpSendToSlack | C# | Yes | Slack Notification Sample code.

## License

[MIT](https://github.com/guitarrapc/AzureFunctionsIntroduction/blob/master/LICENSE)

# AzureFunctionsIntroduction

This is Sample project for Azure Functions. May this repository help you understand Azure Functions better.

# Recommend Azure Functions settings for stability

- Add Dynamic Memory from default 128MB to 256MB.

![](images/MemoryAllocation.png)

- Stop nesting multiple Azure Functions, and use ```#load "<YourCoolLogic.csx>"``` to load shared code.

# What you can know

This sample include following.

FunctionName | Language | Description
---- | ---- | ----
AppSettingsWebhookCSharp | C# | Reference ```Application Settings > App Setting``` of Web Apps Sample code.
ExternalCsxWebhookCSharp | C# | Reference external .csx usage Sample code.
GenericWebhookCSharpExtensionMethod | C# | Extension Method usage Sample code.
GithubWebhookCSharp | C# | Github Webhook Sample code.
LineBotWebhookCSharp | C# | Line Bot Webhook Sample code.
VSTSWebhookCSharp | C# | Visual Studio Team Service (VSTS) Webhook Sample code.
WebhookCSharpGithubOctokit | C# | NuGet package reference sample for Octokit.
WebhookCSharpSendToChatWork | C# | Chatwork Notification Sample code.
WebhookCSharpSendToSlack | C# | Slack Notification Sample code.

# GitHub Integration Sample

You may find this repository structure is fit with Azure Functions CI by Github.

This repogitory Sync with Azure Functions by GitHub Integration.

# Reference

http://tech.guitarrapc.com/archive/category/AzureFunctions

# License

[MIT](https://github.com/guitarrapc/AzureFunctionsIntroduction/blob/master/LICENSE)

# v1 CSharp Scripting : Obsolated

This is Sample project for Azure Functions v1, CSharp Scripting. 

# Samples

This sample include following.

DirectoryName | FunctionName | Language | Pre-compiled? | Description
---- | ---- | ---- | ----  | ----
CsxSamples | AppSettingsWebhookCSharp | C# | No | Reference ```Application Settings > App Setting``` of Web Apps Sample code.
CsxSamples | CSharpCompilerSlackOuthookCSharp | C# | No | Slack Interactive C# Code Roslyn Evaluation Sample. (```@C#: Enumerable.Range(10, 20).Aggregate((x, y) => x + y)```)
CsxSamples | CSharpCompilerWebhookCSharp | C# | No | Generic Webhook C# Code Roslyn Evaluation Sample.
CsxSamples | DotNetFrameworkVersionResponseCSharp | C# | No | Retrurn .NET Framework Friendly Name by passing .NET Framework Release Registry Value.
CsxSamples | ExternalCsxWebhookCSharp | C# | No | Reference external .csx usage Sample code.
CsxSamples | GenericWebhookCSharpExtensionMethod | C# | No | Extension Method usage Sample code.
CsxSamples | GithubWebhookCSharp | C# | No | Github Webhook Sample code.
CsxSamples | LineBotWebhookCSharp | C# | No | Line Bot Webhook Sample code with Emergency Evacuation info with sent info.
CsxSamples | SSLCertificateExpireCheck | C# | No | SSL Certificate Checker. Often introduce in AWS Lambda but you can do with C# + AzureFucntions, too! 
CsxSamples | VSTSWebhookCSharp | C# | No | Visual Studio Team Service (VSTS) Webhook Sample code.
CsxSamples | WebhookCSharpGithubOctokit | C# | No | NuGet package reference sample for Octokit.
CsxSamples | WebhookCSharpSendToChatWork | C# | No | Chatwork Notification Sample code.
CsxSamples | WebhookCSharpSendToSlack | C# | No | Slack Notification Sample code.

# GitHub Integration Sample

You may find this directory structure is fit with Azure Functions CI by Github. This repogitory Sync with Azure Functions by GitHub Integration.

Default structure can be publish with Precompiled Functions.

If you want to use CsxSamples then replace whole root items with `CsxSamples/`. It allows you to deploy .csx samples by Azure Github Continuous Deployemnt.

![](../images/CsxDeployment.png)

# License

[MIT](https://github.com/guitarrapc/AzureFunctionsIntroduction/blob/master/LICENSE)

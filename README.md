# AzureFunctionsIntroduction

This is Sample project for Azure Functions. May this repository help you understand Azure Functions better.

# Recommend Azure Functions settings for stability

Description | Screenshot
---- | ----
[Concider to select Dynamic Service Plan if possible.]((https://azure.microsoft.com/en-us/documentation/articles/functions-scale/)) <br/>This will bring you best cost efficiency and scalability | ![](images/DynamicServicePlan.png)
Keep your Azure Functions Runtime version up-to-date. <br/>Expecially higher than 0.5 is neccesary.<br/>You can apply just in a seconds.<br/>[Release note will be here.](https://github.com/Azure/azure-webjobs-sdk-script/releases) | ![](images/AzureFunctionsRuntimeVersion.png)
Make sure your functions in AppService will be less than 1536MB. [This is limiation of Dynamic Service Plan](https://azure.microsoft.com/en-us/documentation/articles/functions-scale/). <br/>In case you exceed 1536MB, use App Service Plan. | ![](images/DynamicServicePlanMemory.png)
[Keep Function App Platform 32bit (don't change to 64bit)](http://stackoverflow.com/questions/36653122/is-there-any-difference-between-platform-32-bit-or-platform-64-bit-for-azure) | ![](images/PlatformSetting.png)
Add Dynamic Memory from default 128MB to 256MB. | ![](images/MemoryAllocation.png)
Stop nesting multiple Azure Functions, use ```#load "<YourCoolLogic.csx>"``` to load shared code. <br/> This is efficient and speedier way to call functions.  | ![](images/LoadCsx.png)

# What you can know

This sample include following.

FunctionName | Language | Description
---- | ---- | ----
AppSettingsWebhookCSharp | C# | Reference ```Application Settings > App Setting``` of Web Apps Sample code.
CSharpCompilerSlackOuthookCSharp | C# | Slack Interactive C# Code Roslyn Evaluation Sample. (```@C#: Enumerable.Range(10, 20).Aggregate((x, y) => x + y)```)
CSharpCompilerWebhookCSharp | C# | Generic Webhook C# Code Roslyn Evaluation Sample.
DotNetFrameworkVersionResponseCSharp | C# | Retrurn .NET Framework Friendly Name by passing .NET Framework Release Registry Value.
ExternalCsxWebhookCSharp | C# | Reference external .csx usage Sample code.
GenericWebhookCSharpExtensionMethod | C# | Extension Method usage Sample code.
GithubWebhookCSharp | C# | Github Webhook Sample code.
LineBotWebhookCSharp | C# | Line Bot Webhook Sample code with Emergency Evacuation info with sent info.
SSLCertificateExpireCheck | C# | SSL Certificate Checker. Often introduce in AWS Lambda but you can do with C# + AzureFucntions, too! 
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

# AzureFunctionsIntroduction

This is Sample project for Azure Functions. May this repository help you understand Azure Functions better.

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

# More Reference

http://tech.guitarrapc.com/archive/category/AzureFunctions

# Recommend Azure Functions settings for stability

These settings are my recommendation with using AzureFunctions.

Description | Screenshot
---- | ----
[Concider to select Dynamic Service Plan if possible.]((https://azure.microsoft.com/en-us/documentation/articles/functions-scale/)) <br/>This will bring you best cost efficiency and scalability | ![](images/DynamicServicePlan.png)
Keep your Azure Functions Runtime version up-to-date. Actually there's no meaning concider downtime because apply will be done in just a seconds.<br/>[Release note will be here.](https://github.com/Azure/azure-webjobs-sdk-script/releases) | ![](images/AzureFunctionsRuntimeVersionUpgrade.png)<br/>![](images/AzureFunctionsRuntimeVersion.png)
[Keep Function App Platform 32bit (don't change to 64bit)](http://stackoverflow.com/questions/36653122/is-there-any-difference-between-platform-32-bit-or-platform-64-bit-for-azure) | ![](images/PlatformSetting.png)
Do not run out memory, add Dynamic Memory if needed! Default 128MB will be run out easiry. Upgrade to 256MB or higher as your app requires.<br/> Dynamic Plan pricing is relates to Memory size, but less meanful to concider.<br/>[AzureFunctions pricing is here.](https://azure.microsoft.com/en-us/pricing/details/functions/)  | ![](images/MemoryAllocation.png)
Make sure your functions in AppService will be less than 1536MB. [This is limiation of Dynamic Service Plan](https://azure.microsoft.com/en-us/documentation/articles/functions-scale/). <br/>In case you exceed 1536MB there's 2 options.<br/> - Divide to separate functions. <br/>- Combine all functions to single App Service Plan. | ![](images/DynamicServicePlanMemory.png)
Stop nesting multiple Azure Functions, use ```#load "<YourCoolLogic.csx>"``` to load shared code. <br/> This is efficient and speedier way to call functions.  | ![](images/LoadCsx.png)
Use AppSettings to store secret values. This eliminate sensitive value in the source code.<br/>You can load it with Environment Variable or ConfigurationManager. <br/> C# sample with screenShot: ```GetEnvironmentVariable("Secret_Value")``` or ```ConfigurationManager.AppSettings["Secret_Value"];```<br/>[See C# Dev Samples for more details](https://azure.microsoft.com/en-us/documentation/articles/functions-reference-csharp/) | ![](images/SecretValue.png) 

# Not Recommend

There's are possible but I never recommend. These settings will bring complexity 

Description | Screenshot
---- | ----
Default TimeZone is UTC, but you can use LocalTime zone with ```WEBSITE_TIME_ZONE``` into Application Settings.<br/>You can obtain all timezone string with ```System.TimeZoneInfo.GetSystemTimeZones()``` <br/>Detail is here : [Changing the server time zone on Azure Web Apps](https://blogs.msdn.microsoft.com/tomholl/2015/04/06/changing-the-server-time-zone-on-azure-web-apps/)  | ![](images/AzureFunctionsTimeZone.png)

# License

[MIT](https://github.com/guitarrapc/AzureFunctionsIntroduction/blob/master/LICENSE)

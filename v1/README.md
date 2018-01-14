# V1 : Obsorated

This is Sample project for Azure Functions v1, precompiled.

v1 is obsolated and I never recomend this style.

# Samples

This sample include following.

DirectoryName | FunctionName | Language | Pre-compiled? | Description
---- | ---- | ---- | ----  | ----
src | AppSettingsWebhookCSharp | C# | Yes | Reference ```Application Settings > App Setting``` of Web Apps Sample code.
src | CSharpCompilerSlackOuthookCSharp | C# | Yes | Slack C# Code Roslyn Evaluation Sample. (```@C#: Enumerable.Range(10, 20).Aggregate((x, y) => x + y)```)
src | CSharpCompilerWebhookCSharp | C# | Yes | Generic Webhook C# Code Roslyn Evaluation Sample.
src | DotNetFrameworkVersionResponseCSharp | C# | Yes | Retrurn .NET Framework Friendly Name by passing .NET Framework Release Registry Value.
src | GenericWebhookCSharpExtensionMethod | C# | Yes | Extension Method usage Sample code.
src | GithubWebhookCSharp | C# | Yes | Github Webhook Sample code.
src | LineBotWebhookCSharp | C# | Yes | Line Bot Webhook Sample code with Emergency Evacuation info with sent info.
src | PreCompiledFunctionSample | C# | Yes | Basic sample of PreCompiled Function. Build artifact will published right under root as PreCompiledFunctionSample.
src | PreCompileEnvironmentVariables | C# | Yes | Basic sample of PreCompiled Function with Logger. Build artifact will published right under root as PreCompileEnvironmentVariables.
src | SSLCertificateExpireCheck | C# | Yes | SSL Certificate Checker. Often introduce in AWS Lambda but you can do with C# + AzureFucntions, too! 
src | VSTSWebhookCSharp | C# | Yes | Visual Studio Team Service (VSTS) Webhook trigger Sample code.
src | WebhookCSharpGithubOctokit | C# | Yes | NuGet package reference sample for Octokit.
src | WebhookCSharpSendToChatWork | C# | Yes | Chatwork Notification Sample code.
src | WebhookCSharpSendToSlack | C# | Yes | Slack Notification Sample code.

# GitHub Integration Sample

You may find this directory structure is fit with Azure Functions CI by Github. This repogitory Sync with Azure Functions by GitHub Integration.

Default structure can be publish with Precompiled Functions.

If you want to use CsxSamples then replace whole root items with `CsxSamples/`. It allows you to deploy .csx samples by Azure Github Continuous Deployemnt.

![](../images/CsxDeployment.png)

# Precompiled functions reference

Refer basic information with https://github.com/Azure/azure-webjobs-sdk-script/wiki/Precompiled-functions .

Following additional tips will be useful for first step.

Description | Screenshot
---- | ----
You can use both VS2015 Update3 and VS2017 (not preview 3) with this Repository samples. Make sure install [Azure Functions Core Tools](https://www.npmjs.com/package/azure-functions-core-tools) before begin. | ![](images/azure-functions-core-tools.png)
Use precompile function when you want to gain more faster execution and IDE compile, debug benefits. <br/>Unfortunately `.csx` is not yet match friendly for VS Debugging. It will not detect compile error and less intellisense at all.<br/> Therefore you will find .csx will cause compile error on Function App portal so often. If you feel it reduce your efficiency, then use precompile instead.<br/>[Azure App Service Team Blog - Publishing a .NET class library as a Function App](https://blogs.msdn.microsoft.com/appserviceteam/2017/03/16/publishing-a-net-class-library-as-a-function-app/) explain how to with Web Application project. <br/>If you are using VS2017 update 3 [.NET Web Development and Tools Blog - Visual Studio 2017 Tools for Azure Functions](https://blogs.msdn.microsoft.com/webdev/2017/05/10/azure-function-tools-for-visual-studio-2017/) offer how to with latest tooling.| `function.json` sample.<br/>![](images/PrecompileFunction.png)<br/>VS ProjectSettings > Web Sample<br/>![](images/PrecompileWebAppProjectSetting.png)
There are several way for local debug with post/get request, LinqPad, PowerShell, curl and others. I use [PostMan](https://www.getpostman.com/) Chrome extensions heavily when local debugging and remote debugging. | ![](images/LocalDebugWithPostMan.png)
Nuget Package reference for AzureFunctions functionality.<br/>[Microsoft.AspNet.WebApi.Core](https://www.nuget.org/packages/microsoft.aspnet.webapi.core/) for ```.CreateResponse()``` and other HttpRequestMessage Extension method.<br/>```Install-Package Microsoft.AspNet.WebApi.Client``` <br/>```Install-Package Microsoft.AspNet.WebApi.Core```<br/>[Microsoft.Azure.WebJobs.Host](https://www.nuget.org/packages/Microsoft.Azure.WebJobs) for TraceWriter Logger as like .csx ```using Microsoft.Azure.WebJobs.Host;``` at your code namespace.<br/>[Microsoft.Azure.WebJobs.Extensions](https://www.nuget.org/packages/Microsoft.Azure.WebJobs.Extensions) for Timer trigger<br/>```Install-Package Microsoft.Azure.WebJobs.Extensions``` | ![](images/HttpRequestMessageExtensions.png)<br/>![](images/TraceWriterForPrecompiled.png) <br/>![](images/PrecompileTrigger.png)
Precompiled function's dll will be locked by w3wp.exe when deployed function. This behaiviour is fixed but need to deploy into DEPLOYMENT_TEMP beforehand.<br/> See sample deploy in deploy.cmd | ![](images/PrecompileDeployToTemp.png)
Take care about nuget restore and KUDU SYNC volume. Dynamic pricing is limited to 500MB and will easily reach to the size with precompile.<br/>This repogitory needs 288MB for full Nuget package restore, it means total 500MB when build. <br/>Then Deploy will COPY to DEPLOYMENT_TEMP for Shadowcopy then COPY to production location. <br/>OK. you know what happens. I Recopment split codes for usage (Trigger, timer or other) or use Service Plan model. | ![](images/PrecompileOutOfVolume.png)<br/>![](images/PrecompileOutofVolumeKudo.png)

# License

[MIT](https://github.com/guitarrapc/AzureFunctionsIntroduction/blob/master/LICENSE)

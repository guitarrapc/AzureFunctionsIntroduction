using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsIntroduction.Features.Roslyn
{
    public class RoslynCompiler
    {
        private static readonly string[] DefaultImports = new[] {
            "System",
            "System.IO",
            "System.Linq",
            "System.Collections.Generic",
            "System.Text",
            "System.Text.RegularExpressions",
            "System.Net",
            "System.Net.Http",
            "System.Threading",
            "System.Threading.Tasks",
        };

        private static readonly Assembly[] DefaultReferences =
        {
            typeof(Enumerable).Assembly,
            typeof(List<string>).Assembly,
            typeof(HttpClient).Assembly,
        };

        public static async Task<string> EvaluateCSharpAsync(string code)
        {
            object result = null;
            try
            {
                result = await CSharpScript.EvaluateAsync(code ?? "コードが空ですよ？",
                    ScriptOptions.Default
                        .WithImports(DefaultImports)
                        .WithReferences(new[] {
                            "System",
                            "System.Core",
                            "System.Xml",
                            "System.Xml.Linq",
                        })
                        .WithReferences(DefaultReferences));
            }
            catch (Exception ex)
            {
                result = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}";
            }

            var resultText = result?.ToString() ?? "";

            return resultText;
        }
    }
}

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsIntroduction.Features
{
    public class RoslynComplier
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
            try
            {
                var result = await CSharpScript.EvaluateAsync(code ?? "Input any code, please?",
                    ScriptOptions.Default
                        .WithImports(DefaultImports)
                        .WithReferences(new[] {
                            "System",
                            "System.Core",
                            "System.Xml",
                            "System.Xml.Linq",
                        })
                        .WithReferences(DefaultReferences));
                return result?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}{Environment.NewLine}{ex.StackTrace}";
            }
        }
    }
}

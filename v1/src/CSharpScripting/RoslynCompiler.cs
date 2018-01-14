using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using MyExtensions;

namespace CSharpScripting
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
            "MyExtensions",
        };

        private static readonly Assembly[] DefaultReferences =
        {
            typeof(Enumerable).Assembly,
            typeof(List<string>).Assembly,
            typeof(System.Net.Http.HttpClient).Assembly,
            typeof(EnumerableExtensions).Assembly,
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
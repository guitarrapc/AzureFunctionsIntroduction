using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBuild
{
    class Program
    {
        static int Main(string[] args)
        {
            ///////////////////////////////////////////////////////////
            // MAKE SURE YOU MUST RESTORE NUGET PACKAGES BEFOREHAND. //
            ///////////////////////////////////////////////////////////

            // Arguments
            var parallelBuild = false;
            if (args.Length < 1)
            {
                Console.WriteLine($"Parallel execution argument missing. {nameof(parallelBuild)} : {parallelBuild}");
            }
            else if (bool.TryParse(args[0], out parallelBuild))
            {
                Console.WriteLine($"Parallel execution argument detected. {nameof(parallelBuild)} : {parallelBuild}");
            }

            // Run
            return RunAsync(parallelBuild).GetAwaiter().GetResult();

            ////////////////////////////////////////////////////////////
            // YOU MAY DEPLOY PACKAGE VIA MSDEPLOY, KUDO SYNC OR ANY. //
            ////////////////////////////////////////////////////////////
        }

        public static async Task<int> RunAsync(bool useParalellBuild)
        {
            // Base variables
            var outputPathBase = $@"{ Directory.GetCurrentDirectory()}\Tmp";
            var targetDirectory = $@"{ Directory.GetCurrentDirectory()}\src";

            // Create Build Output Directory
            if (Directory.Exists(outputPathBase))
            {
                Directory.Delete(outputPathBase, true);
            }
            Directory.CreateDirectory(outputPathBase);

            // Create host.json
            using (var file = File.Create(Path.Combine(outputPathBase, "host.json")))
            using (var writer = new StreamWriter(file, Encoding.UTF8))
            {
                writer.WriteLine("{}");
            }

            // Get Csproj Info
            var buildTargets = Directory.EnumerateFiles(targetDirectory, "*.csproj", SearchOption.AllDirectories).Select(csproj =>
            {
                var directory = Directory.GetParent(csproj);
                return new BuildInfo(csproj, directory);
            })
            .ToArray();

            // Build
            int[] exitCodes;
            if (useParalellBuild)
            {
                // Parallel Build        
                exitCodes = buildTargets.AsParallel().Select(x =>
                {
                    var outputPath = Path.Combine(outputPathBase, x.ProjectName);
                    var exitCode = new BuildAgent().Build(x.CsprojFullPath, outputPath, "Release", x.ParentInfo.FullName, TimeSpan.FromMinutes(1));
                    if (exitCode != 0)
                    {
                        throw new Exception($"ExitCode : {exitCode}. Build failed with {x.ProjectName} for {outputPath}.");
                    }
                    return exitCode;
                })
                .ToArray();
            }
            else
            {
                // Sequencial Build
                exitCodes = buildTargets.Select(x =>
                {
                    var outputPath = Path.Combine(outputPathBase, x.ProjectName);
                    var exitCode = new BuildAgent().Build(x.CsprojFullPath, outputPath, "Release", x.ParentInfo.FullName, TimeSpan.FromMinutes(1));
                    if (exitCode != 0)
                    {
                        throw new Exception($"ExitCode : {exitCode}. Build failed with {x.ProjectName} for {outputPath}.");
                    }
                    return exitCode;
                })
                .ToArray();
            }

            return exitCodes.Distinct().OrderByDescending(x => x).First();
        }
    }

    public class BuildAgent
    {
        private static readonly string msbuild = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe";

        public int Build(string projectFile, string outputDirectory, string buildConfiguration, string workingDirectory, TimeSpan timeout)
        {
            var startInfo = new ProcessStartInfo(msbuild)
            {
                Arguments = $"{projectFile} /p:OutDir={outputDirectory},Configuration={buildConfiguration}",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = workingDirectory,
            };

            using (var process = Process.Start(startInfo))
            {
                process.OutputDataReceived += (sender, e) => { if (e.Data != null) { Console.WriteLine(e.Data); } };
                process.ErrorDataReceived += (sender, e) => { if (e.Data != null) { Console.WriteLine(e.Data); } };
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (!process.WaitForExit((int)timeout.TotalMilliseconds))
                {
                    process.Kill();
                    Console.WriteLine("timeout while executing process.");
                }

                process.CancelOutputRead();
                process.CancelErrorRead();

                return process.ExitCode;
            }
        }
    }

    public struct BuildInfo
    {
        public string CsprojFullPath { get; }
        public string ProjectName { get; }
        public DirectoryInfo ParentInfo { get; }

        public BuildInfo(string csprojFullPath, DirectoryInfo directory)
        {
            CsprojFullPath = csprojFullPath;
            ProjectName = Path.GetFileNameWithoutExtension(csprojFullPath);
            ParentInfo = directory;
        }
    }
}
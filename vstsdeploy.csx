#r "System.Net.Http"

using System;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;

// EntryPoint
Program.Main(Args.ToArray());

// Main Logic
public class Program
{
    public static void Main(string[] args)
    {
        Run().GetAwaiter().GetResult();
    }

    public static async Task Run()
    {
        ///////////////////////////////////////////////////////////
        // MAKE SURE YOU MUST RESTORE NUGET PACKAGES BEFOREHAND. //
        ///////////////////////////////////////////////////////////
        
        // Base variables
        var outputPathBase = $@"{Environment.CurrentDirectory}\Tmp";
        var targetDirectory = $@"{Environment.CurrentDirectory}\src";
        var useParalellBuild = false;

        // Build Output Directory
        if (Directory.Exists(outputPathBase))
        {
            Directory.Delete(outputPathBase, true);
        }
        Directory.CreateDirectory(outputPathBase);
        
        // Get Csproj Info
        var buildTargets = Directory.EnumerateFiles(targetDirectory, "*.csproj", SearchOption.AllDirectories).Select(csproj =>
        {
            var directory = Directory.GetParent(csproj);
            return new BuildInfo(csproj, directory);
        })
        .ToArray();

        // Build
        if (useParalellBuild)
        {
            // Parallel Build        
            buildTargets.AsParallel().Select(x =>
            {
                var outputPath = Path.Combine(outputPathBase, x.ProjectName);
                return new BuildAgent().Build(x.CsprojFullPath, outputPath, "Release", x.ParentInfo.FullName, TimeSpan.FromMinutes(1));
            })
            .ToArray();
        }
        else
        {
            // Sequencial Build
            buildTargets.Select(x =>
            {
                var outputPath = Path.Combine(outputPathBase, x.ProjectName);
                return new BuildAgent().Build(x.CsprojFullPath, outputPath, "Release", x.ParentInfo.FullName, TimeSpan.FromMinutes(1));
            })
            .ToArray();
        }
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
            WindowStyle = ProcessWindowStyle.Hidden,
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
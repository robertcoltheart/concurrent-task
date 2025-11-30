#:package Bullseye@6.0.0
#:package SimpleExec@12.0.0

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Bullseye.Targets;
using static SimpleExec.Command;

var version = await GetGitVersion();

Target("clean", () =>
{
    Run("dotnet", "clean --configuration Release");

    if (Directory.Exists("artifacts"))
    {
        Directory.Delete("artifacts", true);
    }
});

Target("restore", dependsOn: ["clean"], () =>
{
    Run("dotnet", "restore");
});

Target("build", dependsOn: ["restore"], () =>
{
    Run("dotnet", "build " +
                  "--no-restore " +
                  "--configuration Release " +
                  $"--property Version={version.SemVer} " +
                  $"--property AssemblyVersion={version.AssemblySemVer} " +
                  $"--property FileVersion={version.AssemblySemFileVer} " +
                  $"--property InformationalVersion={version.InformationalVersion}");
});

Target("test", dependsOn: ["build"], () =>
{
    Run("dotnet", "test --configuration Release --no-restore --no-build");
});

Target("package", dependsOn: ["build", "test"], () =>
{
    Run("dotnet", $"pack --configuration Release --no-restore --no-build --output artifacts --property Version={version.SemVer}");
});

Target("publish", dependsOn: ["package"], () =>
{
    var apiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");

    Run("dotnet", $"nuget push {Path.Combine("artifacts", "*.nupkg")} --api-key {apiKey} --source https://api.nuget.org/v3/index.json");
});

Target("default", dependsOn: ["package"]);

await RunTargetsAndExitAsync(args);

async Task<GitVersion> GetGitVersion()
{
    Run("dotnet", "tool restore");

    var (value, _) = await ReadAsync("dotnet", "dotnet-gitversion");

    return JsonSerializer.Deserialize(value, SourceGenerationContext.Default.GitVersion)!;
}

public class GitVersion
{
    public string SemVer { get; set; } = string.Empty;

    public string AssemblySemVer { get; set; } = string.Empty;

    public string AssemblySemFileVer { get; set; } = string.Empty;

    public string InformationalVersion { get; set; } = string.Empty;

    public string PreReleaseTag { get; set; } = string.Empty;
}

[JsonSerializable(typeof(GitVersion))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}

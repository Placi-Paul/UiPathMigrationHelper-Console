using Cocona;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using UiPathMigrationHelper_Console.Logger;
using UiPathMigrationHelper_Console.Nuget;

internal class Program
{
    public static async Task Main(string[] args)
    {
        //CoconaApp.Run<Program>(args);

        await CoconaApp.CreateHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddScoped<ILogger, ConsoleLogger>();
            })
            .RunAsync<Program>(args);
    }

    [Command("list")]
    public async Task List(
        [Argument] string sourceUri,
        [FromService] ILogger logger
        )
    {
        var service = new NugetService(sourceUri, logger);
        var packages = await service.ListPackages(0, 10, true, SearchFilterType.IsLatestVersion);

        foreach (var package in packages)
        {
            var packageMetadata = await service.GetPackageMetadata(package.Identity);

            Console.WriteLine($"Package {package.Identity.Id} TargetFramework: {packageMetadata.DependencySets.First().TargetFramework.DotNetFrameworkName} with dependencies:");
            foreach (var dependencyPackage in packageMetadata.DependencySets.First().Packages)
            {
                Console.WriteLine($" {dependencyPackage.Id} {dependencyPackage.VersionRange}");
            }
        }
    }
}
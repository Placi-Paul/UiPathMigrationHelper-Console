using Cocona;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Common;
using UiPathMigrationHelper_Console.Logger;
using UiPathMigrationHelper_Console.Nuget;

internal class Program
{
    public static async Task Main(string[] args)
    {
        await CoconaApp.CreateHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddScoped<IServiceLogger, ConsoleLogger>();
            })
            .RunAsync<Program>(args);
    }

    [Command("list")]
    public async Task List(
        [FromService] IServiceLogger logger,
        [Option("source", ['u'] )] string sourceUri,
        [Option("level", ['l'] )] LogLevel minLogLevel = LogLevel.Warning,
        [Option("skip", ['s'] )] int skip = 0,
        [Option("take", ['t'] )] int take = 10
        )
    {
        if (!IsValidSource(sourceUri))
        {
            logger.LogError("Feed Url is not valid");
            return;
        }

        logger.SetMininumLogLevel(minLogLevel);

        var client = new NugetService(sourceUri, logger);

        var packages = await client.ListAllAsync(skip, take);

        PrintPackageAndDepedencies(packages);
    }

    [Command("search")]
    public async Task Search(
        [FromService] IServiceLogger logger,
        [Option("source", ['u'] )] string sourceUri,
        [Option("packageName", ['n'] )] string packageName,
        [Option("level", ['l'] )] LogLevel minLogLevel = LogLevel.Warning
        )
    {
        if (!IsValidSource(sourceUri))
        {
            logger.LogError("Feed Url is not valid");
            return;
        }

        logger.SetMininumLogLevel(minLogLevel);

        var client = new NugetService(sourceUri, logger);

        var packages = await client.SearchPackageAsync(searchTerm: packageName);

        PrintPackageAndDepedencies(packages);
    }

    private bool IsValidSource(string source)
    {
        return Uri.TryCreate(source, UriKind.Absolute, out Uri _);
    }

    private void PrintPackageAndDepedencies(IEnumerable<PackageGroup> packages)
    {
        foreach (var package in packages)
        {
            Console.WriteLine($"Package: {package.PackageSearchMetadata.Identity.Id}, Version: {package.PackageSearchMetadata.Identity.Version}");

            foreach (var dependecyGroup in package.Dependencies)
            {
                foreach (var dependency in dependecyGroup.Packages)
                {
                    Console.WriteLine($"  Dependency: {dependency.Id}, TargetFramework: {dependecyGroup.TargetFramework}");
                }
            }
            Console.WriteLine();
        }
    }
}
using Cocona;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
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
        [Option("feed", ['F'])] string sourceFeed,
        [Option("level", ['l'])] LogLevel minLogLevel = LogLevel.Warning,
        [Option("skip", ['s'])] int skip = 0,
        [Option("take", ['t'])] int take = 10
        )
    {
        ValidateSource(sourceFeed);

        logger.SetMininumLogLevel(minLogLevel);

        var client = new NugetService(sourceFeed, logger);

        var packages = await client.ListAllAsync(skip, take);

        PrintPackageAndDepedencies(packages);
    }

    [Command("search")]
    public async Task Search(
        [FromService] IServiceLogger logger,
        [Option("feed", ['F'])] string sourceFeed,
        [Option("packageName", ['n'])] string packageName,
        [Option("level", ['l'])] LogLevel minLogLevel = LogLevel.Warning
        )
    {
        ValidateSource(sourceFeed);

        logger.SetMininumLogLevel(minLogLevel);

        var client = new NugetService(sourceFeed, logger);

        var packages = await client.SearchPackageAsync(searchTerm: packageName);

        PrintPackageAndDepedencies(packages);
    }

    [Command("Check")]
    public async Task Check(
        [FromService] IServiceLogger logger,
        [Option("sourceFeed", ['F'] )]string sourceFeed,
        [Option("libraryFeed", ['L'] )]string libraryFeed,
        [Option("targetProject")] UiPathProjectType targetProject
        )
    {
        ValidateSource(sourceFeed);
        
        var packageService = new NugetService(sourceFeed, logger);
        var dependencyService = new NugetService(libraryFeed, logger);

        var packages = await packageService.ListAllAsync();

        foreach (var package in packages)
        {
            Console.WriteLine($"Package {package.PackageSearchMetadata.Identity.Id}. Current Framework: {package.Dependencies.First().TargetFramework.Framework.ToUiPathProject()}");

            foreach (var dependecyGroup in package.Dependencies)
            {
                foreach (var dependecy in dependecyGroup.Packages)
                {
                    var dependecyData = await dependencyService.GetMetadataAsync(new PackageIdentity(dependecy.Id, dependecy.VersionRange.MinVersion));

                    if (dependecyData.DependencySets.Any(ds => ds.TargetFramework.Framework == targetProject.ToNetFramework()))
                    {
                        Console.WriteLine($"Match found for {dependecy.Id}. Compatible with {targetProject}");
                    }
                    else
                    {
                        Console.WriteLine($"! Match NOT found for {dependecy.Id}.");
                    }
                }
            }
            Console.WriteLine();
        }
    }

    [Ignore]
    private void ValidateSource(string source)
    {
        if (Uri.TryCreate(source, UriKind.Absolute, out var _)) return;

        throw new CommandExitedException("Feed Url is not valid", 101);
    }

    [Ignore]
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
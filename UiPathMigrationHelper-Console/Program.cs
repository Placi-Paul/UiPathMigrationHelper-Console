using Cocona;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using UiPathMigrationHelper_Console.Extensions;
using UiPathMigrationHelper_Console.Nuget;
using UiPathMigrationHelper_Console.Parameters;
using UiPathMigrationHelper_Console.UiPath;
using UiPathMigrationHelper_Console.Validations;

internal class Program
{
    public static async Task Main(string[] args)
    {
        await CoconaLiteApp
            .CreateHostBuilder()
            .RunAsync<Program>(args);
    }

    [Command(Description = "List all packages and dependencies from a nuget feed")]
    public async Task List(
        [Option(shortName: 'f', Description = "Url package feed or local folder path with nupkg packages")][FeedIsValid] string feed,
        PaginationParameters paginationParameters
        )
    {
        var client = new NugetService(feed);

        var packages = await client.ListAllAsync(skip: paginationParameters.Skip, top: paginationParameters.Take);

        PrintHelpers.PrintPackageAndDepedencies(packages);
    }

    [Command(Description = "Search package or dependency based on identifier (package name and version)")]
    public async Task Search(
        [Option(shortName: 'f')][FeedIsValid] string feed,
        [Option(shortName: 'n')] string? packageName,
        [Option(shortName: 'd', Description = "Search only top level dependecies")] string? dependecyName,
        [Option(shortName: 'v', Description = "(Optional)This parameters works only when 'packageName' contains the fully qualified package id")] string? version,
        PaginationParameters paginationParameters
        )
    {
        bool packageProvided = !string.IsNullOrWhiteSpace(packageName);
        bool dependecyProvided = !string.IsNullOrWhiteSpace(dependecyName);

        if (!(packageProvided ^ dependecyProvided))
        {
            Console.WriteLine("Provide either a package (-n) or dependecy (-d) name.");
            return;
        }

        var client = new NugetService(feed);
        if (packageProvided)
        {
            await SearchForPackage(client, packageName!, version, paginationParameters);
        }

        if (dependecyProvided)
        {
            await SearchForDependency(client, dependecyName!, version, paginationParameters);
        }
    }

    [Command(Description = "Checks all packages and dependencies to provide full compatibility matrix.")]
    public async Task Check(
        [Option(shortName: 'f')][FeedIsValid] string feed,
        [Option(shortName: 'l', Description = "Library feed used to analyze dependecies,")] string libraryFeed,
        [Option(Description = "UiPath project type that all packages should be converted to.")] ProjectType target,
        PaginationParameters paginationParameters
        )
    {
        var packageService = new NugetService(feed);
        var dependencyService = new NugetService(libraryFeed);

        Package dependencyPackage;
        List<PackageDependency> dependenciesNotCompatible = [];

        var packages = await packageService.ListAllAsync(skip: paginationParameters.Skip, top: paginationParameters.Take);

        foreach (var package in packages)
        {
            foreach (var packageDependency in package.Dependencies.First().Packages)
            {
                var dependecyData = await dependencyService.GetMetadataAsync(new PackageIdentity(packageDependency.Id, packageDependency.VersionRange.MinVersion));

                if (dependecyData is null)
                {
                    dependenciesNotCompatible.Add(packageDependency);
                    continue;
                }

                dependencyPackage = new Package(dependecyData);

                if (!dependencyPackage.ProjectRange.IsCompatible(target))
                {
                    dependenciesNotCompatible.Add(packageDependency);
                }
            }

            if (!dependenciesNotCompatible.Any())
            {
                Console.WriteLine($"{package} can be migrated.");
                continue;
            }

            Console.WriteLine($"! {package} cannot be migrated to {target}");
            foreach (var dependency in dependenciesNotCompatible)
            {
                Console.WriteLine($"Dependency not compatible:{dependency.Id} Version:{dependency.VersionRange.MinVersion}");
            }
            Console.WriteLine();
        }
    }

    private async Task SearchForPackage(
        NugetService client,
        string packageName,
        string? version,
        PaginationParameters paginationParameters
        )
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            var packages = await client.SearchPackageIdAsync(searchTerm: packageName, skip: paginationParameters.Skip, top: paginationParameters.Take);

            IEnumerable<NuGetVersion> versions;

            foreach (var package in packages)
            {
                PrintHelpers.PrintPackageAndDepedencies([package]);
                versions = await client.ListAllVersions(package.Original.Identity.Id, true);

                Console.WriteLine($"Available versions for {package.Original.Identity.Id}: ");
                PrintHelpers.PrintAllVersions(versions);
                Console.WriteLine();
            }
        }
        else
        {
            var package = await client.SearchPackageIdVersionAsync(packageName, version);

            PrintHelpers.PrintPackageAndDepedencies([package]);
        }
    }
    private async Task SearchForDependency(
        NugetService client,
        string dependecyName,
        string? version,
        PaginationParameters paginationParameters
        )
    {
        var packages = await client.ListAllAsync(paginationParameters.Skip, paginationParameters.Take);
        IEnumerable<PackageDependency> dependecies;
        bool hasVersion = !string.IsNullOrWhiteSpace(version);

        foreach (var package in packages)
        {
            dependecies = package.Dependencies.SelectMany(dg => dg.Packages).Where(p => p.Id.Contains(dependecyName) && 
                (!hasVersion || p.VersionRange.MinVersion!.Equals(SemanticVersion.Parse(version!))));

            if (dependecies.Any())
            {
                Console.WriteLine($"{package}");
                PrintHelpers.PrintAllDependecies(dependecies);
                Console.WriteLine();
            }
        }
    }
}
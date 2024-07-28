using Cocona;
using NuGet.Packaging.Core;
using UiPathMigrationHelper_Console.Nuget;
using UiPathMigrationHelper_Console.Parameters;
using UiPathMigrationHelper_Console.UiPath;

internal class Program : CoconaConsoleAppBase
{
    public static async Task Main(string[] args)
    {
        await CoconaApp
            .CreateHostBuilder()
            .RunAsync<Program>(args);
    }

    [Command(Description = "List all packages and dependencies from a nuget feed")]
    public async Task List(
        [Option(shortName: 'f', Description = "Url package feed or local folder path with nupkg packages")] string feed,
        PaginationParameters paginationParameters
        )
    {
        ValidateSource(feed);

        var client = new NugetService(feed);

        var packages = await client.ListAllAsync(skip: paginationParameters.Skip,top: paginationParameters.Take);

        PrintPackageAndDepedencies(packages);
    }

    [Command(Description ="Search package based on identifier (package name)")]
    public async Task Search(
        [Option(shortName: 'f')] string feed,
        [Option(shortName: 'n')] string packageName,
        PaginationParameters paginationParameters
        )
    {
        ValidateSource(feed);

        var client = new NugetService(feed);

        var packages = await client.SearchPackageAsync(searchTerm: packageName, skip: paginationParameters.Skip, top: paginationParameters.Take);

        PrintPackageAndDepedencies(packages);
    }

    [Command(Description ="Checks all packages and dependencies to provide full compatibility matrix.")]
    public async Task Check(
        [Option(shortName:'f')] string feed,
        [Option(shortName:'l', Description ="Library feed used to analyze dependecies,")] string libraryFeed,
        [Option(Description ="UiPath project type that all packages should be converted to.")] ProjectType target,
        PaginationParameters paginationParameters
        )
    {
        ValidateSource(feed);

        var packageService = new NugetService(feed);
        var dependencyService = new NugetService(libraryFeed);
        Package dependencyPackage;

        var packages = await packageService.ListAllAsync(skip: paginationParameters.Skip, top: paginationParameters.Take);

        foreach (var package in packages)
        {
            Console.WriteLine(package);

            foreach (var packageDependency in package.Dependencies.First().Packages)
            {
                var dependecyData = await dependencyService.GetMetadataAsync(new PackageIdentity(packageDependency.Id, packageDependency.VersionRange.MinVersion));

                if (dependecyData is null)
                {
                    Console.WriteLine($"! Package NOT found for {packageDependency.Id} version{packageDependency.VersionRange.MinVersion}");
                    continue;
                }

                dependencyPackage = new Package(dependecyData);

                if (!dependencyPackage.ProjectRange.IsCompatible(target))
                {
                    Console.Write($"NOT compatible with {target}: ");
                }
                Console.WriteLine(dependencyPackage.ToString());
            }
            Console.WriteLine();
        }
    }

    private void ValidateSource(string source)
    {
        if (Uri.TryCreate(source, UriKind.Absolute, out var _)) return;

        throw new CommandExitedException("Feed Url is not valid", 101);
    }

    private void PrintPackageAndDepedencies(IEnumerable<Package> packages)
    {
        foreach (var package in packages)
        {
            Console.WriteLine(package);

            foreach (var dependecyGroup in package.Dependencies)
            {
                foreach (var dependency in dependecyGroup.Packages)
                {
                    Console.WriteLine($"  Dependency: {dependency.Id}, Version: {dependency.VersionRange}, TargetFramework: {dependecyGroup.TargetFramework}, Compatible with: {dependecyGroup.ToCompatibleUiPathProject()}");
                }
            }
            Console.WriteLine();
        }
    }
}
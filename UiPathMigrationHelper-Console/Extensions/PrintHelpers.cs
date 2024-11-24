using NuGet.Packaging.Core;
using NuGet.Versioning;
using UiPathMigrationHelper_Console.Nuget;
using UiPathMigrationHelper_Console.UiPath;

namespace UiPathMigrationHelper_Console.Extensions
{
    internal static class PrintHelpers
    {
        public static void PrintPackageAndDepedencies(IEnumerable<Package?> packages)
        {
            foreach (var package in packages)
            {
                if (package is null) continue;

                Console.WriteLine(package);

                foreach (var dependecyGroup in package!.Dependencies)
                {
                    foreach (var dependency in dependecyGroup.Packages)
                    {
                        Console.WriteLine($" Dependency: {dependency.Id}, Version: {dependency.VersionRange}, TargetFramework: {dependecyGroup.TargetFramework}, Compatible with: {dependecyGroup.ToCompatibleUiPathProject()}");
                    }
                }
                Console.WriteLine();
            }
        }

        public static void PrintAllVersions(IEnumerable<NuGetVersion> versions)
        {
            foreach (var version in versions)
            {
                Console.WriteLine($" {version}");
            }
        }

        public static void PrintAllDependecies(IEnumerable<PackageDependency> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                Console.WriteLine($" Dependency: {dependency.Id} Version: {dependency.VersionRange}");
            }
        }
    }
}

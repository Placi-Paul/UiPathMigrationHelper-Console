using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using UiPathMigrationHelper_Console.UiPath;

namespace UiPathMigrationHelper_Console.Nuget
{
    internal class Package
    {
        public IPackageSearchMetadata Original { get; private set; }
        public IEnumerable<PackageDependencyGroup> Dependencies { get; private set; }
        public ProjectRange ProjectRange { get; private set; }
        public Package(IPackageSearchMetadata package, bool isUiPathProject = true)
        {
            ArgumentNullException.ThrowIfNull(package);

            Original = package;
            Dependencies = package is not null && package.DependencySets.Any() ? package.DependencySets : [];
            ProjectRange = new ProjectRange(package!, isUiPathProject);
        }
        public override string ToString()
        {
            return $"{Original.Identity.Id}, Version: {Original.Identity.Version}, Compatible with: {ProjectRange.OriginalString}";
        }
    }
}

using NuGet.Packaging;
using NuGet.Protocol.Core.Types;

namespace UiPathMigrationHelper_Console.Nuget
{
    public class PackageGroup
    {
        public IPackageSearchMetadata PackageSearchMetadata { get; private set; }
        public IEnumerable<PackageDependencyGroup> Dependencies { get; private set; }
        public PackageGroup(IPackageSearchMetadata packageSearchMetadata)
        {
            PackageSearchMetadata = packageSearchMetadata;
            Dependencies = packageSearchMetadata.DependencySets.Any() ? packageSearchMetadata.DependencySets : Enumerable.Empty<PackageDependencyGroup>();
        }
    }
}

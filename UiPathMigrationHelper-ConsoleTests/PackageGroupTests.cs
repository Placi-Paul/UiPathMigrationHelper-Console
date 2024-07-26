using FluentAssertions;
using NSubstitute;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using UiPathMigrationHelper_Console.Nuget;

namespace UiPathMigrationHelper_ConsoleTests
{
    public class PackageGroupTests
    {
        [Fact]
        public void PackageGroupDependencies_ShouldBeEmpty_WhenDependecySetIsNull()
        {
            var sut = Substitute.For<IPackageSearchMetadata>();
            sut.DependencySets.Returns(Enumerable.Empty<PackageDependencyGroup>());

            var packageGroup = new PackageGroup(sut);

            packageGroup.Dependencies.Should().BeEquivalentTo(Enumerable.Empty<PackageDependencyGroup>());
        }
    }
}

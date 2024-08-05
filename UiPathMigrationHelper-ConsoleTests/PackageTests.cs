using FluentAssertions;
using NSubstitute;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using UiPathMigrationHelper_Console.Nuget;
using UiPathMigrationHelper_Console.UiPath;

namespace UiPathMigrationHelper_ConsoleTests
{
    public class PackageTests
    {
        [Fact]
        public void Package_ShouldThrowArgumentNullException_WhenProvidingNullIPackageSearchMetadata()
        {
            IPackageSearchMetadata nullPackageSearchMetadata = null;

            var sut = () => { new Package(nullPackageSearchMetadata); };

            sut.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Package_ShouldCreateEmptyArray_WhenDependenciesAreEmptyArray()
        {
            IPackageSearchMetadata packageSearch = Substitute.For<IPackageSearchMetadata>();
            packageSearch.DependencySets.Returns(Enumerable.Empty<PackageDependencyGroup>());

            var sut = new Package(packageSearch);

            sut.Dependencies.Should().BeEmpty();
        }

        [Fact]
        public void Package_ShouldPopulateCorrectly_WhenProvidedWithCorrectIPackageSearchMetadata()
        {
            var expectedPackageIdentity = new PackageIdentity("TestId", NuGetVersion.Parse("1.0.0"));
            var dependencyGroup = new PackageDependencyGroup(new NuGet.Frameworks.NuGetFramework(NetConstants.NETFramework), Enumerable.Empty<PackageDependency>());

            IPackageSearchMetadata packageSearch = Substitute.For<IPackageSearchMetadata>();
            packageSearch.DependencySets.Returns([dependencyGroup]);
            packageSearch.Identity.Returns(expectedPackageIdentity);

            var sut = new Package(packageSearch);

            sut.Original.Should().NotBeNull();
            sut.Original.Identity.Should().Be(expectedPackageIdentity);

            sut.Dependencies.Should().NotBeEmpty();
            sut.Dependencies.Should().BeEquivalentTo([dependencyGroup]);

            sut.ProjectRange.Should().NotBeNull();
            sut.ProjectRange.OriginalString.Should().Be(ProjectType.Legacy.ToString());
            sut.ProjectRange.IsLegacySupported.Should().Be(true);
        }
    }
}

using FluentAssertions;
using NSubstitute;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using UiPathMigrationHelper_Console.Nuget;
using UiPathMigrationHelper_Console.UiPath;
using NuGet.Frameworks;



namespace UiPathMigrationHelper_ConsoleTests
{
    public class ProjectRangeTests
    {
        [Fact]
        public void ProjectRange_ShouldThrowArgumentNull_WhenPackageIsNull()
        {
            IPackageSearchMetadata package = null;

            var sut = () => { new ProjectRange(package, false); };
            var sut2 = () => { new ProjectRange(package, true); };

            sut.Should().Throw<ArgumentNullException>();
            sut2.Should().Throw<ArgumentNullException>();
        }

        public record DetermineSingleProjectType(
            int recordId,
            string DotNetFramework,
            bool isPlatformSpecific,
            bool isUipathProject,
            bool isLegacy,
            bool isWindows,
            bool isCrossPlatform,
            string expectedOriginalString)
        { }

        public static TheoryData<DetermineSingleProjectType> Data
        {
            get
            {
                var data = new TheoryData<DetermineSingleProjectType>
                {
                    new (
                        recordId: 1,
                        DotNetFramework: NetConstants.NETFramework,
                        isPlatformSpecific: false,
                        isUipathProject: false,
                        isLegacy: true,
                        isWindows: false,
                        isCrossPlatform: false,
                        expectedOriginalString: "Legacy"
                    ),
                    new (
                        recordId: 2,
                        DotNetFramework: NetConstants.NETFramework,
                        isPlatformSpecific: false,
                        isUipathProject: true,
                        isLegacy: true,
                        isWindows: false,
                        isCrossPlatform: false,
                        expectedOriginalString: "Legacy"
                    ),
                    new (
                        recordId: 3,
                        DotNetFramework: NetConstants.NETCore,
                        isPlatformSpecific: true,
                        isUipathProject: false,
                        isLegacy: false,
                        isWindows: true,
                        isCrossPlatform: false,
                        expectedOriginalString: "Windows"
                    ),
                    new (
                        recordId: 4,
                        DotNetFramework: NetConstants.NETCore,
                        isPlatformSpecific: true,
                        isUipathProject: true,
                        isLegacy: false,
                        isWindows: true,
                        isCrossPlatform: false,
                        expectedOriginalString: "Windows"
                    ),
                    new (
                        recordId: 5,
                        DotNetFramework: NetConstants.NETCore,
                        isPlatformSpecific: false,
                        isUipathProject: false,
                        isLegacy: false,
                        isWindows: true,
                        isCrossPlatform: true,
                        expectedOriginalString: "Windows,CrossPlatform"
                    ),
                    new (
                        recordId: 6,
                        DotNetFramework: NetConstants.NETCore,
                        isPlatformSpecific: false,
                        isUipathProject: true,
                        isLegacy: false,
                        isWindows: false,
                        isCrossPlatform: true,
                        expectedOriginalString: "CrossPlatform"
                    ),
                    new (
                        recordId: 7,
                        DotNetFramework: NetConstants.NETStandard,
                        isPlatformSpecific: false,
                        isUipathProject: false,
                        isLegacy: true,
                        isWindows: true,
                        isCrossPlatform: true,
                        expectedOriginalString: "Legacy,Windows,CrossPlatform"
                    ),
                    new (
                        recordId: 8,
                        DotNetFramework: NetConstants.NETStandard,
                        isPlatformSpecific: false,
                        isUipathProject: true,
                        isLegacy: true,
                        isWindows: true,
                        isCrossPlatform: true,
                        expectedOriginalString: "Legacy,Windows,CrossPlatform"
                    )
                };
                return data;
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ProjectRange_ShouldDetermineSingleProjectType_WhenProvidingSingleDependencyGroup(
            DetermineSingleProjectType testDataInput
            )
        {
            var framework = new NuGetFramework(testDataInput.DotNetFramework, new Version(6, 0, 0), testDataInput.isPlatformSpecific ? "windows" : "", new Version(7, 0, 0));

            var dependencyGroup = new PackageDependencyGroup(framework, []);
            

            IPackageSearchMetadata packageSearch = Substitute.For<IPackageSearchMetadata>();
            packageSearch.DependencySets.Returns([dependencyGroup]);

            var sut = new ProjectRange(packageSearch, testDataInput.isUipathProject);

            sut.IsLegacySupported.Should().Be(testDataInput.isLegacy, testDataInput.ToString());
            sut.IsWindowsSupported.Should().Be(testDataInput.isWindows, testDataInput.ToString());
            sut.IsCrossPlatformSupported.Should().Be(testDataInput.isCrossPlatform, testDataInput.ToString());
            sut.OriginalString.Should().Be(testDataInput.expectedOriginalString, testDataInput.ToString());
        }


    }
}

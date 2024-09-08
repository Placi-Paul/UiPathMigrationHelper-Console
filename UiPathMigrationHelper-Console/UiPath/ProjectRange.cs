using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using UiPathMigrationHelper_Console.Nuget;

namespace UiPathMigrationHelper_Console.UiPath
{
    internal class ProjectRange
    {
        private string? _originalString;
        public bool IsLegacySupported { get; private set; }
        public bool IsWindowsSupported { get; private set; }
        public bool IsCrossPlatformSupported { get; private set; }
        public bool AllSupported => IsLegacySupported && IsWindowsSupported && IsCrossPlatformSupported;
        public string? OriginalString => _originalString;
        public ProjectRange()
        {

        }
        public ProjectRange(IPackageSearchMetadata package, bool isUiPathProject)
        {
            ArgumentNullException.ThrowIfNull(package, nameof(package));

            foreach (var dependencyGroup in package.DependencySets)
            {
                SetCompatibility(dependencyGroup, isUiPathProject);
            }

            SetOriginalString();
        }

        public ProjectRange(PackageDependencyGroup packageDependencyGroup, bool isUiPathProject)
        {
            SetCompatibility(packageDependencyGroup, isUiPathProject);
            SetOriginalString();
        }
        private void SetCompatibility(PackageDependencyGroup dependencyGroup, bool isUiPathProject)
        {
            if (dependencyGroup.TargetFramework.Framework == NetConstants.NETStandard)
            {
                IsLegacySupported = true;
                IsWindowsSupported = true;
                IsCrossPlatformSupported = true;
                return;
            }
            else if (dependencyGroup.TargetFramework.Framework == NetConstants.NETFramework)
            {
                IsLegacySupported = true;
            }
            else if (dependencyGroup.TargetFramework.Framework == NetConstants.NETCore)
            {
                if (dependencyGroup.TargetFramework.HasPlatform)
                {
                    IsWindowsSupported = true;
                }
                else
                {
                    IsWindowsSupported = true & !isUiPathProject; //cannot mark a single uipath project compatible with both, but libraries can be
                    IsCrossPlatformSupported = true;
                }
            }
            else
            {
                //throw new ArgumentException($"Framework {dependencyGroup?.TargetFramework?.Framework} not supported.", nameof(dependencyGroup.TargetFramework.Framework));
            }
        }
        private void SetOriginalString()
        {
            if (AllSupported)
            {
                _originalString = $"{ProjectType.Legacy},{ProjectType.Windows},{ProjectType.CrossPlatform}";
                return;
            }

            var projectTypeMappings = new Dictionary<ProjectType, bool>()
            {
                {ProjectType.Legacy, IsLegacySupported },
                {ProjectType.Windows, IsWindowsSupported },
                {ProjectType.CrossPlatform, IsCrossPlatformSupported }
            };

            _originalString = string.Join(",", projectTypeMappings.Where(kvp => kvp.Value).Select(kvp => kvp.Key));
        }
        public override bool Equals(object? obj)
        {
            return obj is ProjectRange range &&
                   IsLegacySupported == range.IsLegacySupported &&
                   IsWindowsSupported == range.IsWindowsSupported &&
                   IsCrossPlatformSupported == range.IsCrossPlatformSupported;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(IsLegacySupported, IsWindowsSupported, IsCrossPlatformSupported);
        }

        //Checks if source project is compatible with provided range
        // ex: source = Legacy, range = Legacy,Windows result true
        //  source = Legacy, range = Window,CrossPlatform result false
        //  source = Legacy, range = Window result false
        public bool IsCompatible(ProjectRange range)
        {
            return IsLegacySupported == range.IsLegacySupported ||
                   IsWindowsSupported == range.IsWindowsSupported ||
                   IsCrossPlatformSupported == range.IsCrossPlatformSupported;
        }

        public bool IsCompatible(ProjectType type)
        {
            return (type == ProjectType.Legacy && IsLegacySupported) ||
            (type == ProjectType.Windows && IsWindowsSupported) ||
            (type == ProjectType.CrossPlatform && IsCrossPlatformSupported);
        }
    }
}

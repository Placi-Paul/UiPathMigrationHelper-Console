using NuGet.Packaging;
using UiPathMigrationHelper_Console.Nuget;

namespace UiPathMigrationHelper_Console.UiPath
{
    public enum ProjectType
    {
        Legacy,
        Windows,
        CrossPlatform
    }

    public static class ProjectTypeExtensions
    {
        //duplicate code with project range, will have to refactor
        public static string ToCompatibleUiPathProject(this PackageDependencyGroup dependencyGroup)
        {
            if (dependencyGroup.TargetFramework.Framework == NetConstants.NETStandard) return $"{ProjectType.Legacy},{ProjectType.Windows},{ProjectType.CrossPlatform}";
            if (dependencyGroup.TargetFramework.Framework == NetConstants.NETFramework) return $"{ProjectType.Legacy}";

            if (dependencyGroup.TargetFramework.Framework == NetConstants.NETCore)
            {
                if (dependencyGroup.TargetFramework.HasPlatform)
                {
                    return $"{ProjectType.Windows}";
                }
                else
                {
                    return $"{ProjectType.Windows},{ProjectType.CrossPlatform}";
                }
            }
            else
            {
                //throw new ArgumentException("Framework not supported", nameof(dependencyGroup.TargetFramework.Framework));
                return "Unknown";
            }

        }
    }
}
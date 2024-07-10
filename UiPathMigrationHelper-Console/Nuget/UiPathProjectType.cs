using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiPathMigrationHelper_Console.Nuget
{
    public enum UiPathProjectType
    {
        Legacy,
        Windows
    }

    public static class UiPathProjectTypeExtensions
    {
        public static string ToNetFramework(this UiPathProjectType projectType)
        {
            return projectType switch
            {
                UiPathProjectType.Legacy => ".NETFramework",
                UiPathProjectType.Windows => ".NETCoreApp",
                _ => throw new NotImplementedException()
            };
        }

        public static UiPathProjectType ToUiPathProject(this string netFramework)
        {
            return netFramework switch
            {
                ".NETFramework" => UiPathProjectType.Legacy,
                ".NETCoreApp" => UiPathProjectType.Windows,
                _ => throw new NotImplementedException()
            };
        }
    }
}
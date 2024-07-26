using NuGet.Packaging.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiPathMigrationHelper_Console.Nuget
{
    public static class PackagesExtensions
    {
        public static string GetNameAndVersion(this PackageDependency packageDependency)
        {
            return $"{packageDependency.Id}, Version: {packageDependency.VersionRange.MinVersion}";
        }
        public static string GetNameVersionProjectType(this PackageGroup package)
        {
            return $"{package.PackageSearchMetadata.Identity.Id}, Version: {package.PackageSearchMetadata.Identity.Version}, Project Type: {package.Dependencies.First().TargetFramework.Framework.ToUiPathProject()}";
        }
    }
}

using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;

namespace UiPathMigrationHelper_Console.Nuget
{
    internal class NugetService
    {
        private readonly ILogger _logger;
        private SourceRepository _sourceRepository;
        public NugetService(string sourceUrl, ILogger logger)
        {
            List<Lazy<INuGetResourceProvider>> providers = [.. Repository.Provider.GetCoreV3()];
            PackageSource packageSource = new PackageSource(sourceUrl);
            _sourceRepository = new SourceRepository(packageSource, providers);
            _logger = logger;
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> ListPackages(int skip, int take, bool includePrereleases, SearchFilterType searchFilterType)
        {
            var search = await _sourceRepository.GetResourceAsync<PackageSearchResource>();
            var searchFilter = new SearchFilter(includePrereleases, searchFilterType);

            return await search.SearchAsync("", searchFilter, skip, take, _logger, CancellationToken.None);
        }

        public async Task<IPackageSearchMetadata> GetPackageMetadata(PackageIdentity packageIdentity)
        {
            PackageMetadataResource packageMetadataResource = await _sourceRepository.GetResourceAsync<PackageMetadataResource>();
            var context = new SourceCacheContext();

            return await packageMetadataResource.GetMetadataAsync(packageIdentity, context, _logger, CancellationToken.None);
        }
    }
}

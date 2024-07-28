using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace UiPathMigrationHelper_Console.Nuget
{
    internal class NugetService
    {
        private readonly ILogger _logger = NullLogger.Instance;
        private SourceRepository _sourceRepository;
        private readonly SourceCacheContext _sourceCacheContext = new SourceCacheContext();

        public NugetService(string sourceUrl)
        {
            //List<Lazy<INuGetResourceProvider>> providers = [.. Repository.Provider.GetCoreV3()];
            //PackageSource packageSource = new PackageSource(sourceUrl);
            //_sourceRepository = new SourceRepository(packageSource, providers);

            _sourceRepository = Repository.Factory.GetCoreV3(sourceUrl);
        }

        public async Task<ICollection<Package>> ListAllAsync(
            int skip = 0,
            int top = 100,
            bool includePrereleases = false,
            SearchFilterType searchFilterType = SearchFilterType.IsLatestVersion,
            string searchTerm = "")
        {
            var results = new List<Package>();
            IPackageSearchMetadata metadata;

            var searchResults = await SearchAsync(skip, top, includePrereleases, searchFilterType, searchTerm);

            foreach (var searchResult in searchResults)
            {
                metadata = await GetMetadataAsync(searchResult.Identity);
                results.Add(new Package(metadata));
            }

            return results;
        }

        public async Task<ICollection<Package>> SearchPackageAsync(
            string searchTerm,
            int skip = 0,
            int top = 100,
            bool includePrereleases = false,
            SearchFilterType searchFilterType = SearchFilterType.IsLatestVersion
            )
        {
            return await ListAllAsync(skip, top, includePrereleases, searchFilterType, searchTerm);
        }

        public async Task<IPackageSearchMetadata> GetMetadataAsync(PackageIdentity packageIdentity)
        {
            PackageMetadataResource packageMetadataResource = await _sourceRepository.GetResourceAsync<PackageMetadataResource>();

            return await packageMetadataResource.GetMetadataAsync(packageIdentity, _sourceCacheContext, _logger, CancellationToken.None);
        }

        private async Task<IEnumerable<IPackageSearchMetadata>> SearchAsync(
            int skip,
            int take,
            bool includePrereleases,
            SearchFilterType searchFilterType,
            string searchTerm = "")
        {
            var search = await _sourceRepository.GetResourceAsync<PackageSearchResource>();
            var searchFilter = new SearchFilter(includePrereleases, searchFilterType);

            return await search.SearchAsync(searchTerm, searchFilter, skip, take, _logger, CancellationToken.None);
        }
    }
}

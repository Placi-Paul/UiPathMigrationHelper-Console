using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using UiPathMigrationHelper_Console.Logger;

namespace UiPathMigrationHelper_Console.Nuget
{
    public class NugetService
    {
        private readonly ILogger _logger;
        private SourceRepository _sourceRepository;
        private readonly SourceCacheContext _sourceCacheContext = new SourceCacheContext();

        public NugetService(string sourceUrl, IServiceLogger logger)
        {
            //List<Lazy<INuGetResourceProvider>> providers = [.. Repository.Provider.GetCoreV3()];
            //PackageSource packageSource = new PackageSource(sourceUrl);
            //_sourceRepository = new SourceRepository(packageSource, providers);
            
            Initialize(sourceUrl);
            _logger = logger;
        }

        public void Initialize(string sourceUrl)
        {
            if (_sourceRepository is not null) return;
            _sourceRepository = Repository.Factory.GetCoreV3(sourceUrl);
        }

        public async Task<ICollection<PackageGroup>> ListAllAsync(
            int skip = 0,
            int top = 100,
            bool includePrereleases = false,
            SearchFilterType searchFilterType = SearchFilterType.IsLatestVersion,
            string searchTerm = "")
        {
            var results = new List<PackageGroup>();
            IPackageSearchMetadata metadata;

            var packages = await SearchAsync(skip, top, includePrereleases, searchFilterType, searchTerm);

            foreach (var package in packages)
            {
                metadata = await GetMetadataAsync(package.Identity);
                results.Add(new PackageGroup(metadata));
            }

            return results;
        }

        public async Task<ICollection<PackageGroup>> SearchPackageAsync(
            string searchTerm = "",
            int skip = 0,
            int top = 100,
            bool includePrereleases = false,
            SearchFilterType searchFilterType = SearchFilterType.IsLatestVersion
            )
        {
            return await ListAllAsync(skip, top, includePrereleases, searchFilterType, searchTerm);
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> SearchAsync(
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

        public async Task<IPackageSearchMetadata> GetMetadataAsync(PackageIdentity packageIdentity)
        {
            PackageMetadataResource packageMetadataResource = await _sourceRepository.GetResourceAsync<PackageMetadataResource>();

            return await packageMetadataResource.GetMetadataAsync(packageIdentity, _sourceCacheContext, _logger, CancellationToken.None);
        }
    }
}

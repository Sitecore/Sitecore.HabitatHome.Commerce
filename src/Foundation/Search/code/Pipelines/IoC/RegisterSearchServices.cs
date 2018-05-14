using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.XA.Foundation.Search.Interfaces;
using Sitecore.XA.Foundation.Search.Services;
using Sitecore.XA.Foundation.Search.Wrappers;

namespace Sitecore.Foundation.Search.Pipelines.IoC
{
    public class RegisterSearchServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISortingService, SortingService>();
            serviceCollection.AddSingleton<ISearchService, Sitecore.Foundation.Search.Services.SearchService>();
            serviceCollection.AddSingleton<IScopeService, ScopeService>();
            serviceCollection.AddSingleton<IFacetService, FacetService>();
            serviceCollection.AddSingleton<IContentSearchManager, ContentSearchManager>();
            serviceCollection.AddSingleton<ILinqHelper, LinqHelper>();
            serviceCollection.AddSingleton<IIndexResolver, IndexResolver>();
            serviceCollection.AddSingleton<ISearchContextService, SearchContextService>();
        }
    }
}
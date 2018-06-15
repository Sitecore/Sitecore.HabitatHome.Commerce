using System.Linq;          
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.HabitatHome.Feature.Catalog.Repositories;

namespace Sitecore.HabitatHome.Feature.Catalog.ServiceCollection
{
    public class RegisterCatalogServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {                                                                           
            Replace<Sitecore.XA.Feature.Navigation.Repositories.Breadcrumb.IBreadcrumbRepository, BreadcrumbRepository>(serviceCollection, ServiceLifetime.Transient);
        }

        public IServiceCollection Replace<TService, TImplementation>(
            IServiceCollection services,
            ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {

            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(TService));  
            services.Remove(descriptorToRemove);

            var descriptorToAdd = new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime);      
            services.Add(descriptorToAdd);

            return services;
        }
    }
}
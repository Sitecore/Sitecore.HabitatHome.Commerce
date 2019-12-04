using Microsoft.Extensions.DependencyInjection;
using Sitecore.Demo.Foundation.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Sitecore.HabitatHome.Commerce.Website.Infrastructure
{

    public class MvcControllerServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMvcControllers("Sitecore.HabitatHome.Feature.*");
            serviceCollection.AddClassesWithServiceAttribute("Sitecore.HabitatHome.Feature.*");
            serviceCollection.AddClassesWithServiceAttribute("Sitecore.HabitatHome.Foundation.*");
        }
    }
}
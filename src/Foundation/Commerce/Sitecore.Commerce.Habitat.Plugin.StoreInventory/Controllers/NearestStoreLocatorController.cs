
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;
using System;
using System.Threading.Tasks;
using Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines.Arguments;
using Plugin.Demo.HabitatHome.StoreInventorySet.Commands;


namespace Plugin.Demo.HabitatHome.StoreInventorySet.Controllers
{
    [EnableQuery]
    [Route("api/NearestStoreLocator")]
    public class NearestStoreLocatorController : CommerceController
    {
        public NearestStoreLocatorController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
             : base(serviceProvider, globalEnvironment)
        {
        }

        [HttpGet]
        [Route("(Id={id})")]
        [EnableQuery]
        public async Task<IActionResult> Get(string id)
        {
            NearestStoreLocatorController nearestStoreLocatorController = this;
            if (!nearestStoreLocatorController.ModelState.IsValid || string.IsNullOrEmpty(id))
                return (IActionResult)nearestStoreLocatorController.NotFound();
            //id = id.EnsurePrefix(CommerceEntity.IdPrefix<InventoryInformation>());

            var input = id.Split('|');

            var args = new GetNearestStoreDetailsByLocationArgument() { Latitude = Convert.ToDouble(input[0]), Longitude = Convert.ToDouble(input[1]) };

            var result = await nearestStoreLocatorController.Command<GetNearestStoreDetailsByLocationCommand>().Process(nearestStoreLocatorController.CurrentContext, args);
            return result != null ? (IActionResult)new ObjectResult((object)result) : (IActionResult)nearestStoreLocatorController.NotFound();
        }

    }
}

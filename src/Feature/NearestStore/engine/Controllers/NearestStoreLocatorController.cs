using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Sitecore.Commerce.Core;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Commands;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Controllers
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
            {
                return nearestStoreLocatorController.NotFound();
            }

            var input = id.Split('|');

            var args = new GetNearestStoreDetailsByLocationArgument() { Latitude = Convert.ToDouble(input[0]), Longitude = Convert.ToDouble(input[1]) };

            var result = await nearestStoreLocatorController.Command<GetNearestStoreDetailsByLocationCommand>().Process(nearestStoreLocatorController.CurrentContext, args);
            return result != null ? new ObjectResult(result) : (IActionResult)nearestStoreLocatorController.NotFound();
        }

    }
}

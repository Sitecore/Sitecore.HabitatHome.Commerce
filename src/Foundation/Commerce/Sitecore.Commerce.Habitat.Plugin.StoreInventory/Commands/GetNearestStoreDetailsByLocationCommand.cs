using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Inventory;
using Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines;
using Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines.Arguments;
using Plugin.Demo.HabitatHome.StoreInventorySet.Entities;

namespace Plugin.Demo.HabitatHome.StoreInventorySet.Commands
{
    public class GetNearestStoreDetailsByLocationCommand : CommerceCommand
    {

        private readonly IGetNearestStoreDetailsByLocationPipeline _getNearestStoreDetailsByLocationPipeline
;
        public GetNearestStoreDetailsByLocationCommand(IGetNearestStoreDetailsByLocationPipeline getNearestStoreDetailsByLocationPipeline, IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
            this._getNearestStoreDetailsByLocationPipeline = getNearestStoreDetailsByLocationPipeline;
           
        }

        public async Task<List<NearestStoreLocation>> Process(CommerceContext commerceContext, GetNearestStoreDetailsByLocationArgument inputArgumentList)
        {
            GetNearestStoreDetailsByLocationCommand getNearestStoreDetailsByLocationCommand = this;
            CommercePipelineExecutionContextOptions pipelineContextOptions = commerceContext.GetPipelineContextOptions();

            //InventorySet result = (InventorySet)null;            
            List<NearestStoreLocation> sets = new List<NearestStoreLocation>();
            using (CommandActivity.Start(commerceContext, (CommerceCommand)getNearestStoreDetailsByLocationCommand))
            {
                sets = await getNearestStoreDetailsByLocationCommand._getNearestStoreDetailsByLocationPipeline.Run(inputArgumentList, pipelineContextOptions);                     
            }

            return sets;
        }
    }
}

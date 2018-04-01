using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.Commerce.Plugin.Inventory;
using Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines.Arguments;

namespace Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines
{
    
    [PipelineDisplayName("StoreInventory.pipeline.CreateStoreInventorySetPipeline")]
    public interface IAssociateStoreInventoryToSellableItem : IPipeline<SellableItemInventorySetsArgument, bool, CommercePipelineExecutionContext>, IPipelineBlock<SellableItemInventorySetsArgument, bool, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
    }
}

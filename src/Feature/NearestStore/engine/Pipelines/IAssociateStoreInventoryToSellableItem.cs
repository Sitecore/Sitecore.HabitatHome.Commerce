using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines
{
    
    [PipelineDisplayName("StoreInventory.pipeline.CreateStoreInventorySetPipeline")]
    public interface IAssociateStoreInventoryToSellableItem : IPipeline<SellableItemInventorySetsArgument, bool, CommercePipelineExecutionContext>, IPipelineBlock<SellableItemInventorySetsArgument, bool, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
    }
}

using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines
{
    [PipelineDisplayName("StoreInventory.pipeline.CreateStoreInventorySetPipeline")]
    public interface ICreateStoreInventorySetPipeline: IPipeline<CreateStoreInventorySetArgument, InventorySet, CommercePipelineExecutionContext>, IPipelineBlock<CreateStoreInventorySetArgument, InventorySet, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
    }
}

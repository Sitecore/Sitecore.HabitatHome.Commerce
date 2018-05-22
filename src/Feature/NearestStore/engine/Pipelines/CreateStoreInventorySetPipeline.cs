using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines
{
    public class CreateStoreInventorySetPipeline  :  CommercePipeline<CreateStoreInventorySetArgument, InventorySet>, ICreateStoreInventorySetPipeline, IPipeline<CreateStoreInventorySetArgument, InventorySet, CommercePipelineExecutionContext>, IPipelineBlock<CreateStoreInventorySetArgument, InventorySet, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
        public CreateStoreInventorySetPipeline(IPipelineConfiguration<ICreateStoreInventorySetPipeline> configuration, ILoggerFactory loggerFactory) : base((IPipelineConfiguration)configuration, loggerFactory)
        {
        }
    }
}

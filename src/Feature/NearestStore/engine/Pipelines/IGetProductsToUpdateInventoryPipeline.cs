using System.Collections.Generic;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines
{
    [PipelineDisplayName("StoreInventory.pipeline.GetProductsToUpdateInventoryPipeline")]
    public interface IGetProductsToUpdateInventoryPipeline : IPipeline<string, List<string>, CommercePipelineExecutionContext>, IPipelineBlock<string, List<string>, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
    }
}

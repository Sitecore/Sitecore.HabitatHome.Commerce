using System.Collections.Generic;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Entities;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines
{
    [PipelineDisplayName("StoreInventory.pipeline.GetNearestStoreDetailsByLocationPipeline")]
    public interface IGetNearestStoreDetailsByLocationPipeline : IPipeline<GetNearestStoreDetailsByLocationArgument, List<NearestStoreLocation>, CommercePipelineExecutionContext>, IPipelineBlock<GetNearestStoreDetailsByLocationArgument, List<NearestStoreLocation>, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
    }
}

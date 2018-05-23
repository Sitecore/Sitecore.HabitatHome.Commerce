using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Entities;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines
{
    public class GetNearestStoreDetailsByLocationPipeline : CommercePipeline<GetNearestStoreDetailsByLocationArgument, List<NearestStoreLocation>>, IGetNearestStoreDetailsByLocationPipeline, IPipeline<GetNearestStoreDetailsByLocationArgument, List<NearestStoreLocation>, CommercePipelineExecutionContext>, IPipelineBlock<GetNearestStoreDetailsByLocationArgument, List<NearestStoreLocation>, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
        public GetNearestStoreDetailsByLocationPipeline(IPipelineConfiguration<IGetNearestStoreDetailsByLocationPipeline> configuration, ILoggerFactory loggerFactory) : base((IPipelineConfiguration)configuration, loggerFactory)
        {
        }
    }
}

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
using Plugin.Demo.HabitatHome.StoreInventorySet.Entities;

namespace Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines
{
    public class GetNearestStoreDetailsByLocationPipeline : CommercePipeline<GetNearestStoreDetailsByLocationArgument, List<NearestStoreLocation>>, IGetNearestStoreDetailsByLocationPipeline, IPipeline<GetNearestStoreDetailsByLocationArgument, List<NearestStoreLocation>, CommercePipelineExecutionContext>, IPipelineBlock<GetNearestStoreDetailsByLocationArgument, List<NearestStoreLocation>, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
        public GetNearestStoreDetailsByLocationPipeline(IPipelineConfiguration<IGetNearestStoreDetailsByLocationPipeline> configuration, ILoggerFactory loggerFactory) : base((IPipelineConfiguration)configuration, loggerFactory)
        {
        }
    }
}

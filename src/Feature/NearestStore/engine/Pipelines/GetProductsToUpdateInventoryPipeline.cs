using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines
{

    public class GetProductsToUpdateInventoryPipeline : CommercePipeline<string, List<string>>, IGetProductsToUpdateInventoryPipeline, IPipeline<string, List<string>, CommercePipelineExecutionContext>, IPipelineBlock<string, List<string>, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
        public GetProductsToUpdateInventoryPipeline(IPipelineConfiguration<IGetProductsToUpdateInventoryPipeline> configuration, ILoggerFactory loggerFactory) : base((IPipelineConfiguration)configuration, loggerFactory)
        {
        }
    }
}

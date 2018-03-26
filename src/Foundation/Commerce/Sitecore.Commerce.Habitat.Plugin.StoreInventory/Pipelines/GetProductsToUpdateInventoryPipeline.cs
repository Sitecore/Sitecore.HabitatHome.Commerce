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

    public class GetProductsToUpdateInventoryPipeline : CommercePipeline<string, List<string>>, IGetProductsToUpdateInventoryPipeline, IPipeline<string, List<string>, CommercePipelineExecutionContext>, IPipelineBlock<string, List<string>, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
        public GetProductsToUpdateInventoryPipeline(IPipelineConfiguration<IGetProductsToUpdateInventoryPipeline> configuration, ILoggerFactory loggerFactory) : base((IPipelineConfiguration)configuration, loggerFactory)
        {
        }
    }
}

using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.Orders.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.Orders.Engine.Pipelines
{
   
    public class CreateOfflineOrderPipeline : CommercePipeline<OfflineStoreOrderArgument, Order>, ICreateOfflineOrderPipeline, IPipeline<OfflineStoreOrderArgument, Order, CommercePipelineExecutionContext>, IPipelineBlock<OfflineStoreOrderArgument, Order, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
        public CreateOfflineOrderPipeline(IPipelineConfiguration<ICreateOfflineOrderPipeline> configuration, ILoggerFactory loggerFactory)
          : base(configuration, loggerFactory)
        {
        }
    }
}

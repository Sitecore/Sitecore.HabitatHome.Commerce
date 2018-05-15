using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.Orders.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.Orders.Engine.Pipelines
{
    [PipelineDisplayName("Orders.pipeline.createofflineorder")]
    public interface ICreateOfflineOrderPipeline : IPipeline<OfflineStoreOrderArgument, Order, CommercePipelineExecutionContext>, IPipelineBlock<OfflineStoreOrderArgument, Order, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
    }
    
}

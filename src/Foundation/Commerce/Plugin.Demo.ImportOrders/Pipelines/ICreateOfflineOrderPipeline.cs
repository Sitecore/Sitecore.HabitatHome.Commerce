using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Commerce.Plugin.Customers;
using Plugin.Demo.ImportOrders.Pipelines.Arguments;

namespace Plugin.Demo.ImportOrders.Pipelines
{
    [PipelineDisplayName("Orders.pipeline.createofflineorder")]
    public interface ICreateOfflineOrderPipeline : IPipeline<OfflineStoreOrderArgument, Order, CommercePipelineExecutionContext>, IPipelineBlock<OfflineStoreOrderArgument, Order, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
    }
    
}

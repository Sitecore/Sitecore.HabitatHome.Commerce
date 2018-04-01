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
using Microsoft.Extensions.Logging;

namespace Plugin.Demo.ImportOrders.Pipelines
{
   
    public class CreateOfflineOrderPipeline : CommercePipeline<OfflineStoreOrderArgument, Order>, ICreateOfflineOrderPipeline, IPipeline<OfflineStoreOrderArgument, Order, CommercePipelineExecutionContext>, IPipelineBlock<OfflineStoreOrderArgument, Order, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
        public CreateOfflineOrderPipeline(IPipelineConfiguration<ICreateOfflineOrderPipeline> configuration, ILoggerFactory loggerFactory)
          : base((IPipelineConfiguration)configuration, loggerFactory)
        {
        }
    }
}

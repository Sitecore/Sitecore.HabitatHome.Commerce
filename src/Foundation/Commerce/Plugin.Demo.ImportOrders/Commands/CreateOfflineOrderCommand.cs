using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;
using Plugin.Demo.ImportOrders.Pipelines;
using Sitecore.Commerce.Plugin.Orders;
using Plugin.Demo.ImportOrders.Pipelines.Arguments;

namespace Plugin.Demo.ImportOrders.Commands
{
    public class CreateOfflineOrderCommand : CommerceCommand
    {
        private readonly ICreateOfflineOrderPipeline _createOfflineOrderPipeline;
        
        public CreateOfflineOrderCommand(ICreateOfflineOrderPipeline createOfflineOrderPipeline, IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
            
            this._createOfflineOrderPipeline = createOfflineOrderPipeline;
        }

        public virtual async Task<Order> Process(CommerceContext commerceContext, OfflineStoreOrderArgument arg)
        {
            CreateOfflineOrderCommand createOrderCommand = this;
            Order result = (Order)null;            
            Order order;
            using (CommandActivity.Start(commerceContext, (CommerceCommand)createOrderCommand))
            {
               
                Func<Task> func = await createOrderCommand.PerformTransaction(commerceContext, (Func<Task>)(async () =>
                {                                    
                   
                    Order order2 = await createOrderCommand._createOfflineOrderPipeline.Run(arg, commerceContext.GetPipelineContextOptions());                    
                    result = order2;
                }));
                order = result;
            }
            return order;
        }
    }
}

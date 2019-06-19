using System;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.HabitatHome.Feature.Orders.Engine.Pipelines;
using Sitecore.HabitatHome.Feature.Orders.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.Orders.Engine.Commands
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
            Order result = null;            
            Order order;

            using (CommandActivity.Start(commerceContext, createOrderCommand))
            {
                await createOrderCommand.PerformTransaction(commerceContext, async () =>
                {                                           
                    Order order2 = await createOrderCommand._createOfflineOrderPipeline.Run(arg, commerceContext.PipelineContextOptions).ConfigureAwait(false);                    
                    result = order2;
                });
                order = result;
            }

            return order;
        }
    }
}

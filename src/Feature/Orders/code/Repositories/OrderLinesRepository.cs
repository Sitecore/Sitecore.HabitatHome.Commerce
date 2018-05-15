using Sitecore.Commerce.Entities.Orders;
using Sitecore.Commerce.Services.Orders;
using Sitecore.Commerce.XA.Feature.Account.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.Orders.Models;

namespace Sitecore.HabitatHome.Feature.Orders.Repositories
{
    public class OrderLinesRepository : BaseAccountRepository, IOrderLinesRepository
    {
        public OrderLinesRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, IOrderManager orderManager)
          : base(modelProvider, storefrontContext)
        {
            Assert.IsNotNull(orderManager, nameof(orderManager));
            this.OrderManager = orderManager;
        }

        protected IOrderManager OrderManager { get; set; }

        public OrderLinesViewModel GetOrderLinesRenderingModel(IVisitorContext visitorContext, string orderId)
        {
            OrderLinesViewModel model = ModelProvider.GetModel<OrderLinesViewModel>();
            Init(model);
            if (string.IsNullOrEmpty(orderId))
            {
                return OrderLinesMockData.InitializeMockData(model);
            }

            ManagerResponse<GetVisitorOrderResult, Order> orderDetails = OrderManager.GetOrderDetails(StorefrontContext.CurrentStorefront, visitorContext, orderId);
            if (!orderDetails.ServiceProviderResult.Success || orderDetails.Result == null)
            {
                string systemMessage = StorefrontContext.GetSystemMessage("Could not retrieve order details!", true);
                model.ErrorMessage = systemMessage;
                return model;
            }
            model.Initialize(orderDetails.Result);
            return model;
        }
    }

}
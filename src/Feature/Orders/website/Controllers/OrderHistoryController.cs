using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Commerce.XA.Feature.Account.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Diagnostics;

namespace Sitecore.HabitatHome.Feature.Orders.Controllers
{
    public class OrderHistoryController : BaseCommerceStandardController
    {
        public OrderHistoryController(IStorefrontContext storefrontContext, IVisitorContext visitorContext, IOrderHistoryRepository orderHistoryRepository, IContext context)
             : base(storefrontContext, context)
        {
            Assert.IsNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.IsNotNull((object)visitorContext, nameof(visitorContext));
            Assert.IsNotNull((object)orderHistoryRepository, nameof(orderHistoryRepository));  
            this.OrderHistoryRepository = orderHistoryRepository;
            this.VisitorContext = visitorContext;
        }

        protected IVisitorContext VisitorContext { get; set; }

        protected IOrderHistoryRepository OrderHistoryRepository { get; set; }

        [HttpGet]
        [Authorize]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult OrderHistory()
        {
            return (ActionResult)this.View("~/Views/Orders/OrderHistory.cshtml", (object)this.OrderHistoryRepository.GetOrderHistoryRenderingModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult GetOrderHistory()
        {
            return this.Json((object)this.OrderHistoryRepository.GetVisitorOrders(this.VisitorContext));
        }
    }
}
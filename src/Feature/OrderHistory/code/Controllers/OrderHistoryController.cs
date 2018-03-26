using Sitecore.Commerce.XA.Feature.Account.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Diagnostics;
using System.Web.Mvc;
using System.Web.UI;

namespace Sitecore.Feature.OrderHistory.Controllers
{
    public class OrderHistoryController : BaseCommerceStandardController
    {
        public OrderHistoryController(IStorefrontContext storefrontContext, IVisitorContext visitorContext, IOrderHistoryRepository orderHistoryRepository)
      : base(storefrontContext)
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
            return (ActionResult)this.View("~/Views/OrderHistory/OrderHistory.cshtml", (object)this.OrderHistoryRepository.GetOrderHistoryRenderingModel());
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
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Diagnostics;
using Sitecore.Feature.OrderLines.Models;
using Sitecore.Feature.OrderLines.Repositories;
using System.Web.Mvc;
using System.Web.UI;

namespace Sitecore.Feature.OrderLines.Controllers
{
    public class OrderLinesController : BaseCommerceStandardController
    {
        public OrderLinesController(IStorefrontContext storefrontContext, IVisitorContext visitorContext, IOrderLinesRepository orderLinesRepository)
      : base(storefrontContext)
        {
            Assert.IsNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.IsNotNull((object)visitorContext, nameof(visitorContext));
            Assert.IsNotNull((object)orderLinesRepository, nameof(orderLinesRepository));
            this.OrderLinesRepository = orderLinesRepository;
            this.VisitorContext = visitorContext;
        }

        protected IVisitorContext VisitorContext { get; set; }

        protected IOrderLinesRepository OrderLinesRepository { get; set; }

        [HttpGet]
        [Authorize]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult OrderLines([Bind(Prefix = "id")] string orderId = "")
        {          

            return (ActionResult)this.View("~/Views/OrderLines/OrderLines.cshtml", (object)this.OrderLinesRepository.GetOrderLinesRenderingModel(this.VisitorContext, orderId));

        }
    }
}
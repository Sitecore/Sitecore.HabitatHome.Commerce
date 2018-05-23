using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.Orders.Repositories;

namespace Sitecore.HabitatHome.Feature.Orders.Controllers
{
    public class OrderReviewController : BaseCommerceStandardController
    {
        public OrderReviewController(IStorefrontContext storefrontContext, IReviewRepository reviewRepository, IVisitorContext visitorContext) : base(storefrontContext)
        {
            Assert.ArgumentNotNull((object)reviewRepository, nameof(reviewRepository));            
            this.ReviewRepository = reviewRepository;
            this.VisitorContext = visitorContext;
        }
        public IReviewRepository ReviewRepository { get; protected set; }
        public IVisitorContext VisitorContext { get; protected set; }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Review()
        {
            return (ActionResult)this.View("~/Views/Orders/OrderReview.cshtml", (object)this.ReviewRepository.GetReviewRenderingModel(this.Rendering));
        }

        [ValidateJsonAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetReviewData()
        {
            return this.Json((object)this.ReviewRepository.GetReviewData(this.VisitorContext), JsonRequestBehavior.AllowGet);
        }
    }
}
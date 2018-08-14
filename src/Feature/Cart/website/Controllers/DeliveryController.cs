using Sitecore.Commerce.XA.Feature.Cart.Repositories;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.Cart.Managers;
using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Commerce.XA.Foundation.Common.Context;

namespace Sitecore.HabitatHome.Feature.Cart.Controllers
{
    public class DeliveryController : BaseCommerceStandardController
    {             
        private readonly IDeliveryRepository _deliveryRepository;                   
        private readonly IVisitorContext _visitorContext;
        private readonly ISearchManager _searchManager;

        public DeliveryController(IContext context, IStorefrontContext storefrontContext, IDeliveryRepository deliveryRepository, IVisitorContext visitorContext, ISearchManager searchManager)
          : base(storefrontContext, context)
        {            
            Assert.ArgumentNotNull(deliveryRepository, nameof(deliveryRepository));
            Assert.ArgumentNotNull(storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));            
            _visitorContext = visitorContext;
            _deliveryRepository = deliveryRepository;
            _searchManager = searchManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Delivery()
        {
            return View("~/Views/Commerce/Checkout/Delivery.cshtml", _deliveryRepository.GetDeliveryRenderingModel(this.Rendering));
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetCurrentCart(string cartID)
        {
            ShoppingCartLinesManager cartManager = new ShoppingCartLinesManager(StorefrontContext, _searchManager);
            string shopName = StorefrontContext.CurrentStorefront.ShopName;
            string cartId = $"Default{_visitorContext.UserId}" + shopName;

            dynamic cartModel = cartManager.GetCurrentCartLines(cartId);
            JsonResult baseJsonResult = this.Json(cartModel);
            return this.Json(baseJsonResult);
        }
    }
}

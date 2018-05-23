
using Sitecore.Commerce.XA.Feature.Cart.Models;
using Sitecore.Commerce.XA.Feature.Cart.Models.InputModels;
using Sitecore.Commerce.XA.Feature.Cart.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.Cart.Managers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.UI;

namespace Sitecore.HabitatHome.Feature.Cart.Controllers
{
    public class DeliveryController : BaseCommerceStandardController
    {
        public DeliveryController(IStorefrontContext storefrontContext, IDeliveryRepository deliveryRepository, IVisitorContext visitorContext, IShoppingCartLinesRepository shoppingCartLinesRepository, ISearchManager searchManager)
          : base(storefrontContext)
        {            
            Assert.ArgumentNotNull((object)deliveryRepository, nameof(deliveryRepository));
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)deliveryRepository, nameof(deliveryRepository));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));            
            this.VisitorContext = visitorContext;
            this.DeliveryRepository = deliveryRepository;
            this.ShoppingCartLinesRepository = shoppingCartLinesRepository;
            this.SearchManager = searchManager;
        }

        public IShoppingCartLinesRepository ShoppingCartLinesRepository { get; protected set; }


        public IDeliveryRepository DeliveryRepository { get; protected set; }
        
        public IVisitorContext VisitorContext { get; protected set; }
        public ISearchManager SearchManager { get; set; }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Delivery()
        {
            return (ActionResult)this.View("~/Views/Commerce/Checkout/Delivery.cshtml", (object)this.DeliveryRepository.GetDeliveryRenderingModel(this.Rendering));
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetCurrentCart(string cartID)
        {
            JsonResult baseJsonResult;
            ShoppingCartLinesManager cartManager = new ShoppingCartLinesManager(this.StorefrontContext, this.SearchManager);
            string shopName = this.StorefrontContext.CurrentStorefront.ShopName;
            string cartId = $"Default{this.VisitorContext.UserId}" + shopName;

            dynamic cartModel = cartManager.GetCurrentCartLines(cartId);
            baseJsonResult = this.Json(cartModel);
            return this.Json((object)baseJsonResult);
        }
    }
}

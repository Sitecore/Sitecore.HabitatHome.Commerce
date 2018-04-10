using Sitecore.Commerce.XA.Feature.Cart.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Diagnostics;
using Sitecore.Feature.ShoppingCartLines.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Sitecore.Feature.ShoppingCartLines.Controllers
{
    public class ShoppingCartLinesController : BaseCommerceStandardController
    {
        public ShoppingCartLinesController(IStorefrontContext storefrontContext, IVisitorContext visitorContext, IShoppingCartLinesRepository shoppingCartLinesRepository)
      : base(storefrontContext)
        {
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));
            Assert.ArgumentNotNull((object)shoppingCartLinesRepository, nameof(shoppingCartLinesRepository));
            this.ShoppingCartLinesRepository = shoppingCartLinesRepository;
            this.VisitorContext = visitorContext;
        }

        public IShoppingCartLinesRepository ShoppingCartLinesRepository { get; protected set; }
        public IVisitorContext VisitorContext { get; protected set; }

        [HttpGet]
        public ActionResult ShoppingCartLines()
        {
            return (ActionResult)this.View("~/Views/ShoppingCartLines/ShoppingCartLines.cshtml", (object)this.ShoppingCartLinesRepository.GetShoppingCartLinesModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetCurrentCartLines(string cartID)
        {
            JsonResult baseJsonResult;
            ShoppingCartLinesManager cartManager = new ShoppingCartLinesManager();
            string shopName = this.StorefrontContext.CurrentStorefront.ShopName;
            string cartId = $"Default{this.VisitorContext.UserId}" + shopName;

            dynamic cartModel = cartManager.GetCurrentCartLines(cartId);
            baseJsonResult = this.Json(cartModel);
            return this.Json((object)baseJsonResult);
        }
    }
}
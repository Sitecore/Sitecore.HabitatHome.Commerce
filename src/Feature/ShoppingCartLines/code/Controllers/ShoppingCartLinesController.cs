using Sitecore.Commerce.XA.Feature.Cart.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sitecore.Feature.ShoppingCartLines.Controllers
{
    public class ShoppingCartLinesController : BaseCommerceStandardController
    {
        public ShoppingCartLinesController(IStorefrontContext storefrontContext, IShoppingCartLinesRepository shoppingCartLinesRepository)
      : base(storefrontContext)
        {           
            Assert.ArgumentNotNull((object)shoppingCartLinesRepository, nameof(shoppingCartLinesRepository));
            this.ShoppingCartLinesRepository = shoppingCartLinesRepository;           
        }

        public IShoppingCartLinesRepository ShoppingCartLinesRepository { get; protected set; }

        [HttpGet]
        public ActionResult ShoppingCartLines()
        {
            return (ActionResult)this.View("~/Views/ShoppingCartLines/ShoppingCartLines.cshtml", (object)this.ShoppingCartLinesRepository.GetShoppingCartLinesModel());
        }


    }
}
using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.Cart.Managers;
using Sitecore.HabitatHome.Feature.Cart.Repositories;

namespace Sitecore.HabitatHome.Feature.Cart.Controllers
{
    public class ShoppingCartLinesController : BaseCommerceStandardController
    {
        public ShoppingCartLinesController(IStorefrontContext storefrontContext, IVisitorContext visitorContext, ISearchManager searchManager, IShoppingCartLinesRepository shoppingCartLinesRepository, IContext context)
            : base(storefrontContext, context)
        {
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));
            Assert.ArgumentNotNull((object)searchManager, nameof(searchManager));
            Assert.ArgumentNotNull((object)shoppingCartLinesRepository, nameof(shoppingCartLinesRepository));
            this.ShoppingCartLinesRepository = shoppingCartLinesRepository;
            this.VisitorContext = visitorContext;
            this.SearchManager = searchManager;
        }
        public ISearchManager SearchManager { get; set; }
        public IShoppingCartLinesRepository ShoppingCartLinesRepository { get; protected set; }
        public IVisitorContext VisitorContext { get; protected set; }

        [HttpGet]
        public ActionResult ShoppingCartLines()
        {
            return (ActionResult)this.View("~/Views/Cart/ShoppingCartLines.cshtml", (object)this.ShoppingCartLinesRepository.GetShoppingCartLinesModel());
        }
        public JsonResult GetShoppingCartLines()
        {            
            return this.Json((object)this.ShoppingCartLinesRepository.GetCurrentShoppingCart(this.StorefrontContext, this.VisitorContext));
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetCurrentCartLines(string cartID)
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
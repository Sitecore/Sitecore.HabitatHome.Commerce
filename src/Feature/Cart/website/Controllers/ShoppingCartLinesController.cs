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
        private readonly ISearchManager _searchManager;
        private readonly IShoppingCartLinesRepository _shoppingCartLinesRepository;
        private readonly IVisitorContext _visitorContext;

        public ShoppingCartLinesController(IStorefrontContext storefrontContext, IVisitorContext visitorContext, ISearchManager searchManager, IShoppingCartLinesRepository shoppingCartLinesRepository, IContext context)
            : base(storefrontContext, context)
        {
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));
            Assert.ArgumentNotNull(searchManager, nameof(searchManager));
            Assert.ArgumentNotNull(shoppingCartLinesRepository, nameof(shoppingCartLinesRepository));
            _shoppingCartLinesRepository = shoppingCartLinesRepository;
            _visitorContext = visitorContext;
            _searchManager = searchManager;
        }

        [HttpGet]
        public ActionResult ShoppingCartLines()
        {
            return View("~/Views/Cart/ShoppingCartLines.cshtml", _shoppingCartLinesRepository.GetShoppingCartLinesModel());
        }
        public JsonResult GetShoppingCartLines()
        {            
            return Json(_shoppingCartLinesRepository.GetCurrentShoppingCart(StorefrontContext, _visitorContext));
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetCurrentCartLines()
        {
            ShoppingCartLinesManager cartManager = new ShoppingCartLinesManager(StorefrontContext, _searchManager);
            string shopName = StorefrontContext.CurrentStorefront.ShopName;
            string cartId = $"Default{_visitorContext.UserId}" + shopName;

            dynamic cartModel = cartManager.GetCurrentCartLines(cartId);
            JsonResult baseJsonResult = this.Json(cartModel);
            return Json(baseJsonResult);
        }              
    }
}
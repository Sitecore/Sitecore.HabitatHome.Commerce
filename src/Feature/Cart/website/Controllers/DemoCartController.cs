using Sitecore.Commerce.XA.Feature.Cart.Controllers;
using Sitecore.Commerce.XA.Feature.Cart.Models;
using Sitecore.Commerce.XA.Feature.Cart.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using System.Web.Mvc;

namespace Sitecore.HabitatHome.Feature.Cart.Controllers
{
    public class DemoCartController : CartController
    {
        public DemoCartController(IStorefrontContext storefrontContext, IModelProvider modelProvider, IAddToCartRepository addToCartRepository, IMinicartRepository minicartRepository, IPromotionCodesRepository promotionCodesRepository, IShoppingCartLinesRepository shoppingCartLinesRepository, IShoppingCartTotalRepository shoppingCartTotalRepository, IVisitorContext visitorContext)
            : base(storefrontContext, modelProvider, addToCartRepository, minicartRepository, promotionCodesRepository, shoppingCartLinesRepository, shoppingCartTotalRepository, visitorContext)
        {

        }
                                                 
        public ActionResult AddToDemoCart()
        {
            AddToCartRenderingModel addToCartModel = AddToCartRepository.GetAddToCartModel();

            // CatalogName appears to always be empty when the model is returned from the repository
            // overriding with the current storefront catalog from context
            addToCartModel.CatalogName = this.StorefrontContext.CurrentStorefront.Catalog;

            return base.View("~/Views/Commerce/Cart/AddToCart.cshtml", addToCartModel);
        }

        /// <summary>
        /// Additional HttpPost method so Minicart can be returned on a post      
        /// </summary>
        /// <returns></returns>          
        public ActionResult DemoMinicart()
        {
            MinicartRenderingModel minicartModel = this.MinicartRepository.GetMinicartModel(base.StorefrontContext, this.VisitorContext);
            return View("~/Views/Commerce/Cart/MiniCart.cshtml", minicartModel);
        }
    }
}
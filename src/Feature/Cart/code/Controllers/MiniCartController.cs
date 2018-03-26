using System.Web.Mvc;
using Sitecore.Commerce.XA.Feature.Cart.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Feature.Cart.Models;

namespace Sitecore.Feature.Cart.Controllers
{
    public class MiniCartController : Commerce.XA.Feature.Cart.Controllers.CartController
    {
        public MiniCartController(IStorefrontContext storefrontContext, IModelProvider modelProvider, IAddToCartRepository addToCartRepository, IMinicartRepository minicartRepository, IPromotionCodesRepository promotionCodesRepository, IShoppingCartLinesRepository shoppingCartLinesRepository, IShoppingCartTotalRepository shoppingCartTotalRepository, Commerce.XA.Foundation.Connect.IVisitorContext visitorContext) : base(storefrontContext, modelProvider, addToCartRepository, minicartRepository, promotionCodesRepository, shoppingCartLinesRepository, shoppingCartTotalRepository, visitorContext)
        {
        }

        [HttpPost]
        public ActionResult Minicart()
        {
            MinicartRenderingModel minicartModel = this.MinicartRepository.GetMinicartModel(base.StorefrontContext, this.VisitorContext);
            return View("~/Views/Commerce/MiniCart/Minicart.cshtml", minicartModel);
        }

    }
}
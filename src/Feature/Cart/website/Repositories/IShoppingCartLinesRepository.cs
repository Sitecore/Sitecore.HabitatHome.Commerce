using Sitecore.Commerce.XA.Feature.Cart.Models;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.HabitatHome.Feature.Cart.Models.JsonResults;

namespace Sitecore.HabitatHome.Feature.Cart.Repositories
{
    public interface IShoppingCartLinesRepository
    {
        ShoppingCartLinesRenderingModel GetShoppingCartLinesModel();
        ShoppingCartJsonResult GetCurrentShoppingCart(IStorefrontContext storefrontContext, IVisitorContext visitorContext);
    }
}
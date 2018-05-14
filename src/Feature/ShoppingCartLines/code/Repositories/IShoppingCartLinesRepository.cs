using Sitecore.Commerce.XA.Feature.Cart.Models;
using Sitecore.Commerce.XA.Feature.Cart.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Connect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.ShoppingCartLines.Repositories
{
    public interface IShoppingCartLinesRepository
    {
        ShoppingCartLinesRenderingModel GetShoppingCartLinesModel();
        Sitecore.Feature.ShoppingCartLines.Models.JsonResults.ShoppingCartJsonResult GetCurrentShoppingCart(IStorefrontContext storefrontContext, IVisitorContext visitorContext);
    }
}
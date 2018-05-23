using System;
using System.Collections.Generic;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.HabitatHome.Feature.WishLists.Models;
using Sitecore.HabitatHome.Feature.WishLists.Models.JsonResults;

namespace Sitecore.HabitatHome.Feature.WishLists.Repositories
{
    public interface IWishListLinesRepository
    {
        WishListLinesRenderingModel GetWishListLinesModel();
        WishListJsonResult GetWishList(IStorefrontContext storefrontContext, IVisitorContext visitorContext);
        WishListJsonResult AddWishListLine(IStorefrontContext storefrontContext, IVisitorContext visitorContext, string catalogName, string productId, string variantId, Decimal quantity);
        WishListJsonResult RemoveWishListLines(IStorefrontContext storefrontContext, IVisitorContext visitorContext, List<string> wishListLineIds);
    }
}
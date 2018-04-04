
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Feature.WishListLines.Models;
using Sitecore.Feature.WishListLines.Models.JsonResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Commerce.Services;

namespace Sitecore.Feature.WishListLines.Repositories
{
    public interface IWishListLinesRepository
    {
        WishListLinesRenderingModel GetWishListLinesModel();
        WishListJsonResult GetWishList(IStorefrontContext storefrontContext, IVisitorContext visitorContext);
        WishListJsonResult AddWishListLines(IStorefrontContext storefrontContext, IVisitorContext visitorContext, IEnumerable<WishListLine> wishListLines);
        WishListJsonResult RemoveWishListLines(IStorefrontContext storefrontContext, IVisitorContext visitorContext, IEnumerable<string> wishListLineIds);
    }
}
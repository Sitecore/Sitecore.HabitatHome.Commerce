
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Feature.WishLists.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.WishLists.Repositories
{
    public interface IWishListLinesRepository
    {
        WishListLinesRenderingModel GetWishListLinesModel();

        //BaseJsonResult GetWishList(IStorefrontContext storefrontContext, IVisitorContext visitorContext, string wishListId);

        //BaseJsonResult GetWishLists(IStorefrontContext storefrontContext, IVisitorContext visitorContext);

        //BaseJsonResult CreateWishList(IStorefrontContext storefrontContext, IVisitorContext visitorContext);

        //BaseJsonResult AddLinesToWishList(IStorefrontContext storefrontContext, IVisitorContext visitorContext, IEnumerable<string> wishListLineIds);

        //BaseJsonResult RemoveWishListLines(IStorefrontContext storefrontContext, IVisitorContext visitorContext, IEnumerable<string> wishListLineIds);

        //BaseJsonResult UpdateWishListLines(IStorefrontContext storefrontContext, IVisitorContext visitorContext, IEnumerable<string> wishListLineIds);
        
        ////BaseJsonResult UpdateLineItemQuantity(IStorefrontContext storefrontContext, IVisitorContext visitorContext, string lineNumber, Decimal quantity);

        
    }
}
using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Commerce.Services.WishLists;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Arguments;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Foundation.Habitat.Commerce.Managers
{
    public interface IWishListManager
    {
        ManagerResponse<CreateWishListResult, WishList> CreateWishList(CommerceStorefront storefront, IVisitorContext visitorContext);

        ManagerResponse<GetWishListResult, WishList> GetWishList(IVisitorContext visitorContext, IStorefrontContext storefrontContext, string wishListId);

        ManagerResponse<GetWishListsResult, IEnumerable<WishListHeader>> GetWishLists(IVisitorContext visitorContext, IStorefrontContext storefrontContext);

        ManagerResponse<AddLinesToWishListResult, WishList> AddLinesToWishList(CommerceStorefront storefront, IVisitorContext visitorContext, WishList wishList, IEnumerable<WishListLine> wishListLines);

        ManagerResponse<RemoveWishListLinesResult, WishList> RemoveWishListLines(CommerceStorefront storefront, IVisitorContext visitorContext, WishList wishList, IEnumerable<string> wishListLineIds);

        ManagerResponse<UpdateWishListLinesResult, WishList> UpdateWishListLines(CommerceStorefront storefront, WishList wishList, List<CartLineUpdateArgument> wishListLineUpdateArguments);

        ManagerResponse<DeleteWishListResult, WishList> DeleteWishList(WishList wishList);


    }
}
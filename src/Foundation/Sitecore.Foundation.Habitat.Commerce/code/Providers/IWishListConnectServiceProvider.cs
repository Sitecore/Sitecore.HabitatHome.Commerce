using Sitecore.Commerce.Services.WishLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Foundation.Commerce.WishLists.Providers
{
    public interface IWishListConnectServiceProvider
    {
        WishListServiceProvider GetWishListServiceProvider();
    }
}
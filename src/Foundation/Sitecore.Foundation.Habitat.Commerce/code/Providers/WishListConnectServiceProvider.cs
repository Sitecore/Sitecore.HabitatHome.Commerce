using Sitecore.Commerce.Services.WishLists;
using Sitecore.Commerce.XA.Foundation.Connect.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Foundation.Habitat.Commerce.Providers
{
    public class WishListConnectServiceProvider : ConnectServiceProvider, IWishListConnectServiceProvider
    {
        public virtual WishListServiceProvider GetWishListServiceProvider()
        {
            return this.GetConnectServiceProvider<WishListServiceProvider>("wishListServiceProvider");
        }
    }
}
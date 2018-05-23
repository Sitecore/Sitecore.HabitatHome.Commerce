using Sitecore.Commerce.Services.WishLists;
using Sitecore.Commerce.XA.Foundation.Connect.Providers;

namespace Sitecore.HabitatHome.Foundation.WishLists.Providers
{
    public class WishListConnectServiceProvider : ConnectServiceProvider, IWishListConnectServiceProvider
    {
        public virtual WishListServiceProvider GetWishListServiceProvider()
        {
            return this.GetConnectServiceProvider<WishListServiceProvider>("wishListServiceProvider");
        }
    }
}
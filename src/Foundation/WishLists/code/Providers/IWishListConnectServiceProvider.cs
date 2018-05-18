using Sitecore.Commerce.Services.WishLists;

namespace Sitecore.HabitatHome.Foundation.WishLists.Providers
{
    public interface IWishListConnectServiceProvider
    {
        WishListServiceProvider GetWishListServiceProvider();
    }
}
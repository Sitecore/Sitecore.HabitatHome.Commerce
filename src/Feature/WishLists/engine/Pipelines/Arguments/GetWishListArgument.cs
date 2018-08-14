using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;

namespace Sitecore.HabitatHome.Feature.Wishlists.Engine.Pipelines.Arguments
{
    public class GetWishListArgument : PipelineArgument
    {
        public GetWishListArgument(string shopName, string wishlistid, string shopperId)
        {
            Condition.Requires(wishlistid).IsNotNullOrEmpty("The wishlist id can not be null or empty");

            this.ShopName = shopName;
            this.WishListId = wishlistid;
            this.ShopperId = shopperId;
        }

        public string ShopName { get; set; }

        public string ShopperId { get; set; }

        public string WishListName { get; set; }

        public string WishListId { get; set; }
    }
}

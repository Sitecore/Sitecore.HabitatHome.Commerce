using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;

namespace Plugin.Demo.HabitatHome.Wishlists.Pipelines.Arguments
{
    public class GetWishListArgument : PipelineArgument
    {
        public GetWishListArgument(string shopName, string wishlistid, string shopperId)
        {
            Condition.Requires<string>(WishListId).IsNotNullOrEmpty("The cart id can not be null or empty");
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

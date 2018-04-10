using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Commerce.Services.WishLists;
using Sitecore.Commerce.Engine.Connect.Pipelines.Arguments;

namespace Sitecore.Feature.WishLists.Pipelines.Arguments
{
    public class TranslateCartToWishListEntityResult : TranslateODataToEntityResult<WishList>
    {
        public TranslateCartToWishListEntityResult()
        {

        }
    }
}
using System;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.WishLists;
using Sitecore.Diagnostics;

namespace Sitecore.HabitatHome.Feature.WishLists.Pipelines
{
    public class GetWishList : WishListProcessor
    {
        public override void Process(ServicePipelineArgs args)
        {
            ValidateArguments(args, out GetWishListRequest request, out GetWishListResult result);
                                     
            try
            {
                Assert.IsNotNull(request.UserId, "request.UserIds");                
                Assert.IsNotNull(request.Shop, "request.Shop");
                string prefix = "WishListDefault";
                string message = "wishListId";
                Assert.IsNotNullOrEmpty(prefix, message);                
                Assert.IsNotNullOrEmpty(request.UserId, "userId");
                string userId = request.UserId.Replace("{", string.Empty).Replace("}", string.Empty);                
                string name = request.Shop.Name;
                string cartId = prefix + userId + name;
                Sitecore.Commerce.Plugin.Carts.Cart cart = GetWishList(userId, request.Shop.Name, cartId, "", args.Request.CurrencyCode);
                              
                if (cart != null)
                {
                    // Translate cart to wishlist
                    result.WishList = TranslateCartToWishListEntity(cart, result);
                }
                else
                {
                    result.Success = false;
                }
            }
            catch (ArgumentException ex)
            {
                result.Success = false;
                result.SystemMessages.Add(CreateSystemMessage(ex));
            }
            catch (AggregateException ex)
            {
                result.Success = false;
                result.SystemMessages.Add(CreateSystemMessage(ex));
            }
            base.Process(args);
        }

               
    }    
}
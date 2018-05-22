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
            GetWishListRequest request;
            GetWishListResult result;
            ValidateArguments<GetWishListRequest, GetWishListResult>(args, out request, out result);


            try
            {
                Assert.IsNotNull((object)request.UserId, "request.UserIds");                
                Assert.IsNotNull((object)request.Shop, "request.Shop");
                string prefix = "WishListDefault";
                string message = "wishListId";
                Assert.IsNotNullOrEmpty(prefix, message);                
                Assert.IsNotNullOrEmpty(request.UserId, "userId");
                string userId = request.UserId.Replace("{", string.Empty).Replace("}", string.Empty);                
                string name = request.Shop.Name;
                string cartId = prefix + userId + name;
                Sitecore.Commerce.Plugin.Carts.Cart cart = this.GetWishList(userId, request.Shop.Name, cartId, "", args.Request.CurrencyCode);
                              
                if (cart != null)
                {
                    // Translate cart to wishlist
                    result.WishList = TranslateCartToWishListEntity(cart, result);
                }

                else
                    result.Success = false;
            }
            catch (ArgumentException ex)
            {
                result.Success = false;
                result.SystemMessages.Add(CreateSystemMessage((Exception)ex));
            }
            catch (AggregateException ex)
            {
                result.Success = false;
                result.SystemMessages.Add(CreateSystemMessage((Exception)ex));
            }
            base.Process(args);
        }

               
    }    
}
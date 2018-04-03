using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services;
using Sitecore.Diagnostics;
using System;
using Sitecore.Commerce.Engine.Connect.Pipelines;
using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Commerce.Services.WishLists;
using Sitecore.Commerce.Engine.Connect.Pipelines.Arguments;
using Sitecore.Feature.WishLists.Pipelines.Arguments;
using System.Linq;
using System.Collections.Generic;

namespace Sitecore.Feature.WishLists.Pipelines
{   

    public class RemoveWishListLine : WishListProcessor
    {

        public override void Process(ServicePipelineArgs args)
        {
            RemoveWishListLinesRequest request;
            RemoveWishListLinesResult result;

            ValidateArguments<RemoveWishListLinesRequest, RemoveWishListLinesResult>(args, out request, out result);

            try
            {
                Assert.IsNotNull((object)request.WishList, "request.WishList");
                Assert.IsNotNullOrEmpty(request.WishList.UserId, "request.WishList.UserId");
                Assert.IsNotNull((object)request.LineIds, "request.Lines");                                          
                
                foreach (string lineId in request.LineIds)                {
                  
                    if (!string.IsNullOrEmpty(lineId))
                    {
                        var command = this.RemoveWishListLine(request.WishList.UserId, request.WishList.ShopName, request.WishList.ExternalId, lineId, request.WishList.CustomerId, args.Request.CurrencyCode);
                        result.HandleCommandMessages(command);
                        if (!result.Success)
                            break;
                    }
                }
                Sitecore.Commerce.Plugin.Carts.Cart cart = this.GetWishList(request.WishList.UserId, request.WishList.ShopName, request.WishList.ExternalId, "", args.Request.CurrencyCode);
                if (cart != null)
                    result.WishList = TranslateCartToWishListEntity(cart, (ServiceProviderResult)result);
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
using System;
using Sitecore.Commerce.Engine.Connect.Pipelines;
using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Commerce.Pipelines;    
using Sitecore.Commerce.Services.WishLists;
using Sitecore.Diagnostics;

namespace Sitecore.HabitatHome.Feature.WishLists.Pipelines
{                                                  
    public class RemoveWishListLine : WishListProcessor
    {                                             
        public override void Process(ServicePipelineArgs args)
        {
            ValidateArguments<RemoveWishListLinesRequest, RemoveWishListLinesResult>(args, out var request, out var result);

            try
            {
                Assert.IsNotNull(request.WishList, "request.WishList");
                Assert.IsNotNullOrEmpty(request.WishList.UserId, "request.WishList.UserId");
                Assert.IsNotNull(request.LineIds, "request.Lines");                                          
                
                foreach (string lineId in request.LineIds)                {
                  
                    if (!string.IsNullOrEmpty(lineId))
                    {
                        var command = RemoveWishListLine(request.WishList.UserId, request.WishList.ShopName, request.WishList.ExternalId, lineId, request.WishList.CustomerId, args.Request.CurrencyCode);
                        result.HandleCommandMessages(command);
                        if (!result.Success)
                        {
                            break;
                        }
                    }
                }

                Sitecore.Commerce.Plugin.Carts.Cart cart = GetWishList(request.WishList.UserId, request.WishList.ShopName, request.WishList.ExternalId, "", args.Request.CurrencyCode);
                if (cart != null)
                {
                    result.WishList = TranslateCartToWishListEntity(cart, result);
                    result.RemovedLines = new System.Collections.ObjectModel.ReadOnlyCollection<WishListLine>(SetListLines(result.WishList));
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
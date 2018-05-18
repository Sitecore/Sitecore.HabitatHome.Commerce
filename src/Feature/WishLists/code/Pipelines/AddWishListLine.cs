﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Engine.Connect.Pipelines;
using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.WishLists;
using Sitecore.Diagnostics;

namespace Sitecore.HabitatHome.Feature.WishLists.Pipelines
{
    public class AddWishListLine : WishListProcessor
    {

        public override void Process(ServicePipelineArgs args)
        {
            AddLinesToWishListRequest request;
            AddLinesToWishListResult result;
            

            ValidateArguments<AddLinesToWishListRequest, Commerce.Services.WishLists.AddLinesToWishListResult>(args, out request, out result);            
            try
            {
                Assert.IsNotNull((object)request.WishList, "request.WishList");
                Assert.IsNotNullOrEmpty(request.WishList.UserId, "request.WishList.UserId");
                Assert.IsNotNull((object)request.Lines, "request.Lines");

                List<WishListLine> list = request.Lines.ToList<WishListLine>();
                list.RemoveAll((Predicate<WishListLine>)(l =>
                {
                    if (l != null)
                        return l.Product == null;
                    return true;
                }));
                request.Lines = (IEnumerable<WishListLine>)list;
                foreach (WishListLine line in request.Lines)
                {
                    string lineItemId = line.Product?.ProductId;
                    if (!string.IsNullOrEmpty(lineItemId))
                    {
                        var command = this.AddWishListLine(request.WishList.UserId, request.WishList.ShopName, request.WishList.ExternalId, lineItemId, line.Quantity, request.WishList.CustomerId, args.Request.CurrencyCode);
                       result.HandleCommandMessages(command);
                        if (!result.Success)
                            break;
                    }
                }
                Sitecore.Commerce.Plugin.Carts.Cart cart = this.GetWishList(request.WishList.UserId, request.WishList.ShopName, request.WishList.ExternalId, "", args.Request.CurrencyCode);
                if (cart != null)
                {
                    result.WishList = TranslateCartToWishListEntity(cart, (ServiceProviderResult)result);
                    result.AddedLines = new System.Collections.ObjectModel.ReadOnlyCollection<WishListLine>(SetListLines(result.WishList));                    
                }
                
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

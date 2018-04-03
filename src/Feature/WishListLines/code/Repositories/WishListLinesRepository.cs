using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.WishLists;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Repositories;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Arguments;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Diagnostics;
using Sitecore.Feature.WishListLines.Models;
using Sitecore.Feature.WishListLines.Models.JsonResults;
using Sitecore.Foundation.Habitat.Commerce.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.WishListLines.Repositories
{
    public class WishListLinesRepository : BaseCommerceModelRepository, IWishListLinesRepository
    {
        public WishListLinesRepository(IModelProvider modelProvider, IWishListManager wishListManager, ISiteContext siteContext)
        {
            Assert.ArgumentNotNull((object)modelProvider, nameof(modelProvider));
            Assert.ArgumentNotNull((object)wishListManager, nameof(wishListManager));
            Assert.ArgumentNotNull((object)siteContext, nameof(siteContext));
            this.ModelProvider = modelProvider;
            this.WishListManager = wishListManager;
            this.SiteContext = siteContext;
        }
        public IModelProvider ModelProvider { get; protected set; }

        public IWishListManager WishListManager { get; protected set; }

        public ISiteContext SiteContext { get; protected set; }

        public virtual WishListLinesRenderingModel GetWishListLinesModel()
        {
            WishListLinesRenderingModel model = this.ModelProvider.GetModel<WishListLinesRenderingModel>();
            this.Init((BaseCommerceRenderingModel)model);
            model.Initialize();
            return model;
        }
        public virtual WishListJsonResult CreateWishList(IStorefrontContext storefrontContext, IVisitorContext visitorContext)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));
            WishListJsonResult model = this.ModelProvider.GetModel<WishListJsonResult>();
            try
            {
                ManagerResponse<CreateWishListResult, WishList> newWishList = this.WishListManager.CreateWishList(storefrontContext, visitorContext);
                if (!newWishList.ServiceProviderResult.Success || newWishList.Result == null)
                {
                    model.Success = false;
                    string systemMessage = "Could not create wish list";
                    newWishList.ServiceProviderResult.SystemMessages.Add(new SystemMessage()
                    {
                        Message = systemMessage
                    });
                    model.SetErrors((ServiceProviderResult)newWishList.ServiceProviderResult);
                    return model;
                }
                else
                {
                    model.Success = true;
                    model.Initialize(newWishList.Result);
                }
            }
            catch (Exception ex)
            {
                model.SetErrors(nameof(CreateWishList), ex);
            }
            return model;
        }
        //TODO: GET CART FROM NEW PIPELINE AND USE CART ID. IF NO WISHLIST, RETURN NEW WISHLIST WITH ID POPULATED (GETWISHLIST PIPELINE)
        //add ADD LINES TO WISHLIST METHOD


        public virtual WishListJsonResult RemoveWishListLines(IStorefrontContext storefrontContext, IVisitorContext visitorContext, string wishListId, IEnumerable<string> wishListLineIds)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));
            Assert.ArgumentNotNull((object)wishListId, nameof(wishListId));
            Assert.ArgumentNotNull((object)wishListLineIds, nameof(wishListLineIds));
            WishListJsonResult model = this.ModelProvider.GetModel<WishListJsonResult>();
            CommerceStorefront currentStorefront = storefrontContext.CurrentStorefront;
            ManagerResponse<GetWishListResult, WishList> currentWishList = this.WishListManager.GetWishList(visitorContext, storefrontContext, wishListId);
            if (!currentWishList.ServiceProviderResult.Success || currentWishList.Result == null)
            {
                string systemMessage = "Wish List not found.";
                currentWishList.ServiceProviderResult.SystemMessages.Add(new SystemMessage()
                {
                    Message = systemMessage
                });
                model.SetErrors((ServiceProviderResult)currentWishList.ServiceProviderResult);
                return model;
            }
            ManagerResponse<RemoveWishListLinesResult, WishList> managerResponse = this.WishListManager.RemoveWishListLines(currentStorefront, visitorContext, currentWishList.Result, wishListLineIds);
            if (!managerResponse.ServiceProviderResult.Success)
            {
                model.SetErrors((ServiceProviderResult)managerResponse.ServiceProviderResult);
                return model;
            }
            model.Initialize(managerResponse.Result);
            model.Success = true;
            return model;
        }

        public virtual WishListJsonResult UpdateWishListLines(IStorefrontContext storefrontContext, IVisitorContext visitorContext, string wishListId, string lineNumber, Decimal quantity)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));
            Assert.ArgumentNotNull((object)wishListId, nameof(wishListId));
            Assert.ArgumentNotNull((object)lineNumber, nameof(lineNumber));
            Assert.IsTrue(quantity > Decimal.Zero, "quantity > 0");
            WishListJsonResult model = this.ModelProvider.GetModel<WishListJsonResult>();
            CommerceStorefront currentStorefront = storefrontContext.CurrentStorefront;
            ManagerResponse<GetWishListResult, WishList> currentWishList = this.WishListManager.GetWishList(visitorContext, storefrontContext, wishListId);
            if (!currentWishList.ServiceProviderResult.Success || currentWishList.Result == null)
            {
                string systemMessage = "Wish List not found.";
                currentWishList.ServiceProviderResult.SystemMessages.Add(new SystemMessage()
                {
                    Message = systemMessage
                });
                model.SetErrors((ServiceProviderResult)currentWishList.ServiceProviderResult);
                return model;
            }
            CartLineUpdateArgument lineUpdateArgument = new CartLineUpdateArgument()
            {
                ExternalLineId = lineNumber,
                LineArguments = {
                  Quantity = quantity
                }
            };
            ManagerResponse<UpdateWishListLinesResult, WishList> managerResponse = this.WishListManager.UpdateWishListLines(currentStorefront, currentWishList.Result, new List<CartLineUpdateArgument>()
              {
                lineUpdateArgument
              });
            if (!managerResponse.ServiceProviderResult.Success)
            {
                model.SetErrors((ServiceProviderResult)managerResponse.ServiceProviderResult);
                return model;
            }
            model.Initialize(managerResponse.Result);
            model.Success = true;
            return model;
        }

    }
}
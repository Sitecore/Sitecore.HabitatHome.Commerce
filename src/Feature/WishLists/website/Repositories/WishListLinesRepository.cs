using System;
using System.Collections.Generic;
using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.WishLists;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Repositories;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Arguments;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.WishLists.Models;
using Sitecore.HabitatHome.Feature.WishLists.Models.JsonResults;
using Sitecore.HabitatHome.Foundation.WishLists.Managers;

namespace Sitecore.HabitatHome.Feature.WishLists.Repositories
{
    public class WishListLinesRepository : BaseCommerceModelRepository, IWishListLinesRepository
    {
        private readonly IModelProvider _modelProvider;
        private readonly IWishListManager _wishListManager;     

        public WishListLinesRepository(IModelProvider modelProvider, IWishListManager wishListManager)
        {
            Assert.ArgumentNotNull(modelProvider, nameof(modelProvider));
            Assert.ArgumentNotNull(wishListManager, nameof(wishListManager));       
            _modelProvider = modelProvider;
            _wishListManager = wishListManager;     
        }

        public virtual WishListLinesRenderingModel GetWishListLinesModel()
        {
            WishListLinesRenderingModel model = _modelProvider.GetModel<WishListLinesRenderingModel>();
            Init(model);
            model.Initialize();
            return model;
        }

        public WishListJsonResult GetWishList(IStorefrontContext storefrontContext, IVisitorContext visitorContext)
        {
            Assert.ArgumentNotNull(storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));
            WishListJsonResult model = _modelProvider.GetModel<WishListJsonResult>();
            ManagerResponse<GetWishListResult, WishList> currentWishList = _wishListManager.GetWishList(visitorContext, storefrontContext);

            if (!currentWishList.ServiceProviderResult.Success || currentWishList.Result == null)
            {
                string systemMessage = "Wish List not found.";
                currentWishList.ServiceProviderResult.SystemMessages.Add(new SystemMessage
                {
                    Message = systemMessage
                });
                model.SetErrors(currentWishList.ServiceProviderResult);
                return model;
            }

            if (!currentWishList.ServiceProviderResult.Success)
            {
                model.SetErrors(currentWishList.ServiceProviderResult);
                return model;
            }

            model.Initialize(currentWishList.Result);
            model.Success = true;
            return model;
        }

        public virtual WishListJsonResult AddWishListLine(IStorefrontContext storefrontContext, IVisitorContext visitorContext, string catalogName, string productId, string variantId, Decimal quantity)
        {
            Assert.ArgumentNotNull(storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));
            WishListJsonResult model = _modelProvider.GetModel<WishListJsonResult>();
            CommerceStorefront currentStorefront = storefrontContext.CurrentStorefront;
            ManagerResponse<GetWishListResult, WishList> currentWishList = _wishListManager.GetWishList(visitorContext, storefrontContext);
            if (!currentWishList.ServiceProviderResult.Success || currentWishList.Result == null)
            {
                string systemMessage = "Wish List not found.";
                currentWishList.ServiceProviderResult.SystemMessages.Add(new SystemMessage
                {
                    Message = systemMessage
                });
                model.SetErrors(currentWishList.ServiceProviderResult);
                return model;
            }

            List<WishListLine> wishListLines = new List<WishListLine>();
            var wishlistLine = new WishListLine { Quantity = quantity, Product = new Commerce.Entities.Carts.CartProduct() { ProductId = catalogName + "|" + productId + "|" + variantId, } };
            wishListLines.Add(wishlistLine);

            ManagerResponse<AddLinesToWishListResult, WishList> managerResponse = _wishListManager.AddLinesToWishList(currentStorefront, visitorContext, currentWishList.Result, wishListLines);
            if (!managerResponse.ServiceProviderResult.Success)
            {
                model.SetErrors(managerResponse.ServiceProviderResult);
                return model;
            }

            model.Initialize(managerResponse.Result);
            model.Success = true;
            return model;
        }

        public virtual WishListJsonResult RemoveWishListLines(IStorefrontContext storefrontContext, IVisitorContext visitorContext, List<string> wishListLineIds)
        {
            Assert.ArgumentNotNull(storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));
            Assert.ArgumentNotNull(wishListLineIds, nameof(wishListLineIds));
            WishListJsonResult model = _modelProvider.GetModel<WishListJsonResult>();
            CommerceStorefront currentStorefront = storefrontContext.CurrentStorefront;
            ManagerResponse<GetWishListResult, WishList> currentWishList = _wishListManager.GetWishList(visitorContext, storefrontContext);
            if (!currentWishList.ServiceProviderResult.Success || currentWishList.Result == null)
            {
                string systemMessage = "Wish List not found.";
                currentWishList.ServiceProviderResult.SystemMessages.Add(new SystemMessage
                {
                    Message = systemMessage
                });

                model.SetErrors(currentWishList.ServiceProviderResult);
                return model;
            }
            ManagerResponse<RemoveWishListLinesResult, WishList> managerResponse = _wishListManager.RemoveWishListLines(currentStorefront, visitorContext, currentWishList.Result, wishListLineIds);
            if (!managerResponse.ServiceProviderResult.Success)
            {
                model.SetErrors(managerResponse.ServiceProviderResult);
                return model;
            }
            model.Initialize(managerResponse.Result);
            model.Success = true;
            return model;
        }           
    }
}
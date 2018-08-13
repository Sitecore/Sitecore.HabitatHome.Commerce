using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Entities.WishLists;    
using Sitecore.Commerce.Services.WishLists;        
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Utils;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Arguments;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Foundation.WishLists.Providers;

namespace Sitecore.HabitatHome.Foundation.WishLists.Managers
{
    public class WishListManager : IWishListManager
    {                                                              
        private readonly WishListServiceProvider _wishListServiceProvider;

        public WishListManager(IWishListConnectServiceProvider connectServiceProvider)      
        {
            Assert.ArgumentNotNull(connectServiceProvider, nameof(connectServiceProvider));  
            _wishListServiceProvider = connectServiceProvider.GetWishListServiceProvider();            
        }

        public ManagerResponse<CreateWishListResult, WishList> CreateWishList(IStorefrontContext storefrontContext, IVisitorContext visitorContext)
        {
            Assert.ArgumentNotNull(storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));
            string userId = visitorContext.UserId;
            string shopName = storefrontContext.CurrentStorefront.ShopName;
            string wishListName = "WishList" + Guid.NewGuid() + shopName;

            CreateWishListResult wishListResult = _wishListServiceProvider.CreateWishList(new CreateWishListRequest(userId, wishListName, shopName));
            Helpers.LogSystemMessages(wishListResult.SystemMessages, wishListResult);
            CreateWishListResult serviceProviderResult = wishListResult;
            return new ManagerResponse<CreateWishListResult, WishList>(serviceProviderResult, serviceProviderResult.WishList);
        }

        public ManagerResponse<GetWishListResult, WishList> GetWishList(IVisitorContext visitorContext, IStorefrontContext storefrontContext)
        {
            Assert.ArgumentNotNull(storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));
            string userId = visitorContext.UserId;
            string shopName = storefrontContext.CurrentStorefront.ShopName;
            string wishListId = "wishListId";
            
            GetWishListResult wishListResult = _wishListServiceProvider.GetWishList(new GetWishListRequest(userId, wishListId, shopName));
            Helpers.LogSystemMessages(wishListResult.SystemMessages, wishListResult);
            GetWishListResult serviceProviderResult = wishListResult;
            return new ManagerResponse<GetWishListResult, WishList>(serviceProviderResult, serviceProviderResult.WishList);
        }


        public ManagerResponse<GetWishListsResult, IEnumerable<WishListHeader>> GetWishLists(IVisitorContext visitorContext, IStorefrontContext storefrontContext)
        {
            Assert.ArgumentNotNull(storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));
            string userId = visitorContext.UserId;
            string shopName = storefrontContext.CurrentStorefront.ShopName;

            GetWishListsResult wishListResult = _wishListServiceProvider.GetWishLists(new GetWishListsRequest(userId, shopName));
            Helpers.LogSystemMessages(wishListResult.SystemMessages, wishListResult);
            GetWishListsResult serviceProviderResult = wishListResult;
            return new ManagerResponse<GetWishListsResult, IEnumerable<WishListHeader>>(serviceProviderResult, serviceProviderResult.WishLists);
        }
        
        public ManagerResponse<AddLinesToWishListResult, WishList> AddLinesToWishList(CommerceStorefront storefront, IVisitorContext visitorContext, WishList wishList, IEnumerable<WishListLine> wishListLines)
        {
            Assert.ArgumentNotNull(storefront, nameof(storefront));
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));
            Assert.ArgumentNotNull(wishList, nameof(wishList));
            Assert.ArgumentNotNull(wishListLines, nameof(wishListLines));

            
            AddLinesToWishListResult wishListResult = _wishListServiceProvider.AddLinesToWishList(new AddLinesToWishListRequest(wishList, wishListLines));
            Helpers.LogSystemMessages(wishListResult.SystemMessages, wishListResult);
            AddLinesToWishListResult serviceProviderResult = wishListResult;
            return new ManagerResponse<AddLinesToWishListResult, WishList>(serviceProviderResult, serviceProviderResult.WishList);
        }

        public ManagerResponse<RemoveWishListLinesResult, WishList> RemoveWishListLines(CommerceStorefront storefront, IVisitorContext visitorContext, WishList wishList, IEnumerable<string> wishListLineIds)
        {
            Assert.ArgumentNotNull(storefront, nameof(storefront));
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));
            Assert.ArgumentNotNull(wishList, nameof(wishList));
            Assert.ArgumentNotNull(wishListLineIds, "cartLinesIds");        
            RemoveWishListLinesResult serviceProviderResult = _wishListServiceProvider.RemoveWishListLines(new RemoveWishListLinesRequest(wishList, wishListLineIds));

            return new ManagerResponse<RemoveWishListLinesResult, WishList>(serviceProviderResult, serviceProviderResult.WishList);
        }
        public ManagerResponse<UpdateWishListLinesResult, WishList> UpdateWishListLines(CommerceStorefront storefront, WishList wishList, List<CartLineUpdateArgument> wishListLineUpdateArguments)
        {
            Assert.ArgumentNotNull(wishList, nameof(wishList));
            Assert.ArgumentNotNull(storefront, nameof(storefront));

            List<WishListLine> lineList = new List<WishListLine>();

            foreach (CartLineUpdateArgument lineUpdateArgument in wishListLineUpdateArguments)
            {
                CartLineUpdateArgument inputModel = lineUpdateArgument;
                Assert.ArgumentNotNullOrEmpty(inputModel.ExternalLineId, "inputModel.ExternalLineId");
                int quantity = (int)inputModel.LineArguments.Quantity;
                WishListLine wishListLine = wishList.Lines.FirstOrDefault(l => l.ExternalId == inputModel.ExternalLineId);
                if (wishListLine != null)
                {
                    wishListLine.Quantity = quantity;
                    lineList.Add(wishListLine);
                }
            }

            UpdateWishListLinesResult wishListResult = _wishListServiceProvider.UpdateWishListLines(new UpdateWishListLinesRequest(wishList, lineList));
            Helpers.LogSystemMessages(wishListResult.SystemMessages, wishListResult);
            UpdateWishListLinesResult serviceProviderResult = wishListResult;
            return new ManagerResponse<UpdateWishListLinesResult, WishList>(serviceProviderResult, serviceProviderResult.WishList);
        }

        public ManagerResponse<DeleteWishListResult, WishList> DeleteWishList(WishList wishList)
        {
            Assert.ArgumentNotNull(wishList, nameof(wishList));
            DeleteWishListResult wishListResult = _wishListServiceProvider.DeleteWishList(new DeleteWishListRequest(wishList));
            Helpers.LogSystemMessages(wishListResult.SystemMessages, wishListResult);
            DeleteWishListResult serviceProviderResult = wishListResult;
            return new ManagerResponse<DeleteWishListResult, WishList>(serviceProviderResult, serviceProviderResult.WishList);
        }             
    }
}
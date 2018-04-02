using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.WishLists;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Arguments;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Foundation.Habitat.Commerce.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Foundation.Habitat.Commerce.Managers
{
    public class WishListManager : IWishListManager
    {
        
        public WishListManager(IWishListConnectServiceProvider connectServiceProvider, IStorefrontContext storefrontContext, ISearchManager searchManager)      
        {
            Assert.ArgumentNotNull((object)connectServiceProvider, nameof(connectServiceProvider));
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)searchManager, nameof(searchManager));
            this.StorefrontContext = storefrontContext;
            this.SearchManager = searchManager;
            this.WishListServiceProvider = connectServiceProvider.GetWishListServiceProvider();            
        }
        public IStorefrontContext StorefrontContext { get; set; }

        public ISearchManager SearchManager { get; set; }

        public WishListServiceProvider WishListServiceProvider { get; set; }

        public ManagerResponse<CreateWishListResult, WishList> CreateWishList(IStorefrontContext storefrontContext, IVisitorContext visitorContext)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));
            string userId = visitorContext.UserId;
            string shopName = storefrontContext.CurrentStorefront.ShopName;
            string wishListName = "WishList" + Guid.NewGuid().ToString() + shopName;
            
            CreateWishListResult wishListResult = this.WishListServiceProvider.CreateWishList(new CreateWishListRequest(userId, wishListName, shopName));
            Helpers.LogSystemMessages((IEnumerable<SystemMessage>)wishListResult.SystemMessages, (object)wishListResult);
            CreateWishListResult serviceProviderResult = wishListResult;
            return new ManagerResponse<CreateWishListResult, WishList>(serviceProviderResult, serviceProviderResult.WishList);
        }

        public ManagerResponse<GetWishListResult, WishList> GetWishList(IVisitorContext visitorContext, IStorefrontContext storefrontContext, string wishListId)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));
            string userId = visitorContext.UserId;
            string shopName = storefrontContext.CurrentStorefront.ShopName;
            
            GetWishListResult wishListResult = this.WishListServiceProvider.GetWishList(new GetWishListRequest(userId, wishListId, shopName));
            Helpers.LogSystemMessages((IEnumerable<SystemMessage>)wishListResult.SystemMessages, (object)wishListResult);
            GetWishListResult serviceProviderResult = wishListResult;
            return new ManagerResponse<GetWishListResult, WishList>(serviceProviderResult, serviceProviderResult.WishList);
        }


        public ManagerResponse<GetWishListsResult, IEnumerable<WishListHeader>> GetWishLists(IVisitorContext visitorContext, IStorefrontContext storefrontContext)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));
            string userId = visitorContext.UserId;
            string shopName = storefrontContext.CurrentStorefront.ShopName;

            GetWishListsResult wishListResult = this.WishListServiceProvider.GetWishLists(new GetWishListsRequest(userId, shopName));
            Helpers.LogSystemMessages((IEnumerable<SystemMessage>)wishListResult.SystemMessages, (object)wishListResult);
            GetWishListsResult serviceProviderResult = wishListResult;
            return new ManagerResponse<GetWishListsResult, IEnumerable<WishListHeader>>(serviceProviderResult, serviceProviderResult.WishLists);
        }
        
        public ManagerResponse<AddLinesToWishListResult, WishList> AddLinesToWishList(CommerceStorefront storefront, IVisitorContext visitorContext, WishList wishList, IEnumerable<WishListLine> wishListLines)
        {
            Assert.ArgumentNotNull((object)storefront, nameof(storefront));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));
            Assert.ArgumentNotNull((object)wishList, nameof(wishList));
            Assert.ArgumentNotNull((object)wishListLines, nameof(wishListLines));

            
            AddLinesToWishListResult wishListResult = this.WishListServiceProvider.AddLinesToWishList(new AddLinesToWishListRequest(wishList, (IEnumerable<WishListLine>)wishListLines));
            Helpers.LogSystemMessages((IEnumerable<SystemMessage>)wishListResult.SystemMessages, (object)wishListResult);
            AddLinesToWishListResult serviceProviderResult = wishListResult;
            return new ManagerResponse<AddLinesToWishListResult, WishList>(serviceProviderResult, serviceProviderResult.WishList);
        }

        public ManagerResponse<RemoveWishListLinesResult, WishList> RemoveWishListLines(CommerceStorefront storefront, IVisitorContext visitorContext, WishList wishList, IEnumerable<string> wishListLineIds)
        {
            Assert.ArgumentNotNull((object)storefront, nameof(storefront));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));
            Assert.ArgumentNotNull((object)wishList, nameof(wishList));
            Assert.ArgumentNotNull((object)wishListLineIds, "cartLinesIds");
            List<WishListLine> cartLineList = new List<WishListLine>();

            RemoveWishListLinesResult serviceProviderResult = this.WishListServiceProvider.RemoveWishListLines(new RemoveWishListLinesRequest(wishList, wishListLineIds));

            return new ManagerResponse<RemoveWishListLinesResult, WishList>(serviceProviderResult, serviceProviderResult.WishList);
        }
        public ManagerResponse<UpdateWishListLinesResult, WishList> UpdateWishListLines(CommerceStorefront storefront, WishList wishList, List<CartLineUpdateArgument> wishListLineUpdateArguments)
        {
            Assert.ArgumentNotNull((object)wishList, nameof(wishList));
            Assert.ArgumentNotNull((object)storefront, nameof(storefront));
            List<WishListLine> lineList = new List<WishListLine>();
            foreach (CartLineUpdateArgument lineUpdateArgument in wishListLineUpdateArguments)
            {
                CartLineUpdateArgument inputModel = lineUpdateArgument;
                Assert.ArgumentNotNullOrEmpty(inputModel.ExternalLineId, "inputModel.ExternalLineId");
                int quantity = (int)inputModel.LineArguments.Quantity;
                WishListLine wishListLine = wishList.Lines.FirstOrDefault<WishListLine>((Func<WishListLine, bool>)(l => l.ExternalId == inputModel.ExternalLineId));
                if (wishListLine != null)
                {
                    wishListLine.Quantity = (Decimal)quantity;
                    lineList.Add(wishListLine);
                }
            }
            UpdateWishListLinesResult wishListResult = this.WishListServiceProvider.UpdateWishListLines(new UpdateWishListLinesRequest(wishList, lineList));
            Helpers.LogSystemMessages((IEnumerable<SystemMessage>)wishListResult.SystemMessages, (object)wishListResult);
            UpdateWishListLinesResult serviceProviderResult = wishListResult;
            return new ManagerResponse<UpdateWishListLinesResult, WishList>(serviceProviderResult, serviceProviderResult.WishList);
        }

        public ManagerResponse<DeleteWishListResult, WishList> DeleteWishList(WishList wishList)
        {
            Assert.ArgumentNotNull((object)wishList, nameof(wishList));
            DeleteWishListResult wishListResult = this.WishListServiceProvider.DeleteWishList(new DeleteWishListRequest(wishList));
            Helpers.LogSystemMessages((IEnumerable<SystemMessage>)wishListResult.SystemMessages, (object)wishListResult);
            DeleteWishListResult serviceProviderResult = wishListResult;
            return new ManagerResponse<DeleteWishListResult, WishList>(serviceProviderResult, serviceProviderResult.WishList);
        }



    }
}
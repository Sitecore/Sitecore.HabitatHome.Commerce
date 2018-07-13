using System;
using System.Collections.Generic;
using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.WishLists;           
using Sitecore.Commerce.XA.Foundation.Common.Context;
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

        public WishListJsonResult GetWishList(IStorefrontContext storefrontContext, IVisitorContext visitorContext)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));
            WishListJsonResult model = this.ModelProvider.GetModel<WishListJsonResult>();
            CommerceStorefront currentStorefront = storefrontContext.CurrentStorefront;
            ManagerResponse<GetWishListResult, WishList> currentWishList = this.WishListManager.GetWishList(visitorContext, storefrontContext);

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

            if (!currentWishList.ServiceProviderResult.Success)
            {
                model.SetErrors((ServiceProviderResult)currentWishList.ServiceProviderResult);
                return model;
            }
            model.Initialize(currentWishList.Result);
            model.Success = true;
            return model;
        }



        public virtual WishListJsonResult AddWishListLine(IStorefrontContext storefrontContext, IVisitorContext visitorContext, string catalogName, string productId, string variantId, Decimal quantity)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));
            WishListJsonResult model = this.ModelProvider.GetModel<WishListJsonResult>();
            CommerceStorefront currentStorefront = storefrontContext.CurrentStorefront;
            ManagerResponse<GetWishListResult, WishList> currentWishList = this.WishListManager.GetWishList(visitorContext, storefrontContext);
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

            List<WishListLine> wishListLines = new List<WishListLine>();
            var wishlistLine = new WishListLine() { Quantity = quantity, Product = new Commerce.Entities.Carts.CartProduct() { ProductId = catalogName + "|" + productId + "|" + variantId, } };
            wishListLines.Add(wishlistLine);

            ManagerResponse<AddLinesToWishListResult, WishList> managerResponse = this.WishListManager.AddLinesToWishList(currentStorefront, visitorContext, currentWishList.Result, wishListLines);
            if (!managerResponse.ServiceProviderResult.Success)
            {
                model.SetErrors((ServiceProviderResult)managerResponse.ServiceProviderResult);
                return model;
            }
            model.Initialize(managerResponse.Result);
            model.Success = true;
            return model;
        }



        //public virtual WishListJsonResult AddWishListLine(IStorefrontContext storefrontContext, IVisitorContext visitorContext, string catalogName, string productId, string variantId, Decimal quantity)
        //{
        //    Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
        //    Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));                        
        //    WishListJsonResult model = this.ModelProvider.GetModel<WishListJsonResult>();
        //    CommerceStorefront currentStorefront = storefrontContext.CurrentStorefront;
        //    ManagerResponse<GetWishListsResult, IEnumerable<WishListHeader>> allWishLists = this.WishListManager.GetWishLists(visitorContext, storefrontContext);
        //    WishListHeader currentWishListHeader = allWishLists.Result != null ? allWishLists.Result.ToList()[0] : null;
        //    WishList currentWishList = new WishList();
        //    if(currentWishListHeader == null)
        //    {
        //        ManagerResponse<CreateWishListResult, WishList> createWishListResponse = this.WishListManager.CreateWishList(storefrontContext, visitorContext);
        //        if (!createWishListResponse.ServiceProviderResult.Success)
        //        {
        //            model.SetErrors((ServiceProviderResult)createWishListResponse.ServiceProviderResult);
        //            return model;
        //        }
        //        currentWishList = createWishListResponse.Result;
        //    }
        //    else
        //    {
        //        ManagerResponse<GetWishListResult, WishList> getWishListResponse = this.WishListManager.GetWishList(visitorContext, storefrontContext);
        //        if (!getWishListResponse.ServiceProviderResult.Success)
        //        {
        //            model.SetErrors((ServiceProviderResult)getWishListResponse.ServiceProviderResult);
        //            return model;
        //        }
        //        currentWishList = getWishListResponse.Result;
        //    }           

        //    List<WishListLine> wishListLines = new List<WishListLine>();
        //    var wishlistLine = new WishListLine() { Quantity = quantity, Product = new Commerce.Entities.Carts.CartProduct() { ProductId = catalogName + "|" + productId + "|" + variantId, } };
        //    wishListLines.Add(wishlistLine);

        //    ManagerResponse<AddLinesToWishListResult, WishList> managerResponse = this.WishListManager.AddLinesToWishList(currentStorefront, visitorContext, currentWishList, wishListLines);
        //    if (!managerResponse.ServiceProviderResult.Success)
        //    {
        //        model.SetErrors((ServiceProviderResult)managerResponse.ServiceProviderResult);
        //        return model;
        //    }
        //    model.Initialize(managerResponse.Result);
        //    model.Success = true;
        //    return model;
        //}



        public virtual WishListJsonResult RemoveWishListLines(IStorefrontContext storefrontContext, IVisitorContext visitorContext, List<string> wishListLineIds)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));            
            Assert.ArgumentNotNull((object)wishListLineIds, nameof(wishListLineIds));
            WishListJsonResult model = this.ModelProvider.GetModel<WishListJsonResult>();
            CommerceStorefront currentStorefront = storefrontContext.CurrentStorefront;
            ManagerResponse<GetWishListResult, WishList> currentWishList = this.WishListManager.GetWishList(visitorContext, storefrontContext);
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


        // Not implemented
        public virtual WishListJsonResult UpdateWishListLines(IStorefrontContext storefrontContext, IVisitorContext visitorContext, string lineNumber, Decimal quantity)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));            
            Assert.ArgumentNotNull((object)lineNumber, nameof(lineNumber));
            Assert.IsTrue(quantity > Decimal.Zero, "quantity > 0");
            WishListJsonResult model = this.ModelProvider.GetModel<WishListJsonResult>();
            CommerceStorefront currentStorefront = storefrontContext.CurrentStorefront;
            ManagerResponse<GetWishListResult, WishList> currentWishList = this.WishListManager.GetWishList(visitorContext, storefrontContext);
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
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.WishLists.Repositories
{
    public class WishListLinesRepository : IWishListLinesRepository
    {
        //public WishListLinesRepository(IModelProvider modelProvider, ICartManager cartManager, ISiteContext siteContext)
        //{
        //    Assert.ArgumentNotNull((object)modelProvider, nameof(modelProvider));
        //    Assert.ArgumentNotNull((object)cartManager, nameof(cartManager));
        //    Assert.ArgumentNotNull((object)siteContext, nameof(siteContext));
        //    this.ModelProvider = modelProvider;
        //    this.CartManager = cartManager;
        //    this.SiteContext = siteContext;
        //}

        //public IModelProvider ModelProvider { get; protected set; }

        //public ICartManager CartManager { get; protected set; }

        //public ISiteContext SiteContext { get; protected set; }

        //protected virtual BaseJsonResult RemoveLineItemsFromCart(IStorefrontContext storefrontContext, IVisitorContext visitorContext, IEnumerable<string> lineNumbers)
        //{
        //    Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
        //    Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));
        //    Assert.ArgumentNotNull((object)lineNumbers, nameof(lineNumbers));
        //    BaseJsonResult model = this.ModelProvider.GetModel<BaseJsonResult>();
        //    CommerceStorefront currentStorefront = storefrontContext.CurrentStorefront;
        //    ManagerResponse<CartResult, Sitecore.Commerce.Entities.Carts.Cart> currentCart = this.CartManager.GetCurrentCart(visitorContext, storefrontContext, false);
        //    if (!currentCart.ServiceProviderResult.Success || currentCart.Result == null)
        //    {
        //        string systemMessage = storefrontContext.GetSystemMessage("Cart Not Found Error", true);
        //        currentCart.ServiceProviderResult.SystemMessages.Add(new SystemMessage()
        //        {
        //            Message = systemMessage
        //        });
        //        model.SetErrors((ServiceProviderResult)currentCart.ServiceProviderResult);
        //        return model;
        //    }
        //    ManagerResponse<CartResult, Sitecore.Commerce.Entities.Carts.Cart> managerResponse = this.CartManager.RemoveLineItemsFromCart(currentStorefront, visitorContext, currentCart.Result, lineNumbers);
        //    if (!managerResponse.ServiceProviderResult.Success)
        //    {
        //        model.SetErrors((ServiceProviderResult)managerResponse.ServiceProviderResult);
        //        return model;
        //    }
        //    model.Success = true;
        //    return model;
        //}

        //protected virtual CartJsonResult GetCurrentCart(IStorefrontContext storefrontContext, IVisitorContext visitorContext)
        //{
        //    CartJsonResult model = this.ModelProvider.GetModel<CartJsonResult>();
        //    try
        //    {
        //        ManagerResponse<CartResult, Sitecore.Commerce.Entities.Carts.Cart> currentCart = this.CartManager.GetCurrentCart(visitorContext, storefrontContext, true);
        //        if (!currentCart.ServiceProviderResult.Success || currentCart.Result == null)
        //        {
        //            model.Success = false;
        //            string systemMessage = storefrontContext.GetSystemMessage("Cart Not Found Error", true);
        //            currentCart.ServiceProviderResult.SystemMessages.Add(new SystemMessage()
        //            {
        //                Message = systemMessage
        //            });
        //            model.SetErrors((ServiceProviderResult)currentCart.ServiceProviderResult);
        //        }
        //        model.Initialize(currentCart.Result);
        //    }
        //    catch (Exception ex)
        //    {
        //        model.SetErrors(nameof(GetCurrentCart), ex);
        //    }
        //    return model;
        //}
    }
}
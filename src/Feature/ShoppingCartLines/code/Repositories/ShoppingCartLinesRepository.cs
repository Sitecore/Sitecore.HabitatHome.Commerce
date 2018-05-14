using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Commerce.XA.Feature.Cart.Models;
using Sitecore.Commerce.XA.Feature.Cart.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Common.Repositories;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Feature.ShoppingCartLines.Managers;
using Sitecore.Feature.ShoppingCartLines.Models.JsonResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.ShoppingCartLines.Repositories
{
    public class ShoppingCartLinesRepository : BaseCartRepository, IShoppingCartLinesRepository
    {
        public ShoppingCartLinesRepository(IModelProvider modelProvider, ICartManager cartManager, ISiteContext siteContext, IStorefrontContext storefrontContext, ISearchManager searchManager)
      : base(modelProvider, cartManager, siteContext)
        {
            this.SearchManager = searchManager;
            this.StorefrontContext = storefrontContext;
        }
        public ISearchManager SearchManager { get; set; }
        public IStorefrontContext StorefrontContext { get; set; }
        public virtual ShoppingCartLinesRenderingModel GetShoppingCartLinesModel()
        {
            ShoppingCartLinesRenderingModel model = this.ModelProvider.GetModel<ShoppingCartLinesRenderingModel>();
            this.Init((BaseCommerceRenderingModel)model);
            model.Initialize();
            return model;
        }
        public ShoppingCartJsonResult GetCurrentShoppingCart(IStorefrontContext storefrontContext, IVisitorContext visitorContext)
        {
            Sitecore.Feature.ShoppingCartLines.Models.JsonResults.ShoppingCartJsonResult model = this.ModelProvider.GetModel<Sitecore.Feature.ShoppingCartLines.Models.JsonResults.ShoppingCartJsonResult>();
            try
            {
                ManagerResponse<CartResult, Sitecore.Commerce.Entities.Carts.Cart> currentCart = this.CartManager.GetCurrentCart(visitorContext, storefrontContext, true);
                if (!currentCart.ServiceProviderResult.Success || currentCart.Result == null)
                {
                    model.Success = false;
                    string systemMessage = storefrontContext.GetSystemMessage("Cart Not Found Error", true);
                    currentCart.ServiceProviderResult.SystemMessages.Add(new SystemMessage()
                    {
                        Message = systemMessage
                    });
                    model.SetErrors((ServiceProviderResult)currentCart.ServiceProviderResult);
                }

                ShoppingCartLinesManager cartManager = new ShoppingCartLinesManager(this.StorefrontContext, this.SearchManager);
                string shopName = storefrontContext.CurrentStorefront.ShopName;
                string cartId = $"Default{visitorContext.UserId}" + shopName;

                dynamic cartLineList = cartManager.GetCurrentCartLines(cartId);

                model.Initialize(currentCart.Result, cartLineList);
            }
            catch (Exception ex)
            {
                model.SetErrors(nameof(GetCurrentCart), ex);
            }
            return model;
        }
    }
}
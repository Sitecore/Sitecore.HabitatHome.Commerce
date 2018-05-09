using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Orders;
using Sitecore.Commerce.Entities.Shipping;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Commerce.Services.Orders;
using Sitecore.Commerce.Services.Shipping;
using Sitecore.Commerce.XA.Feature.Cart;
using Sitecore.Commerce.XA.Feature.Cart.Models;
using Sitecore.Commerce.XA.Feature.Cart.Models.InputModels;
using Sitecore.Commerce.XA.Feature.Cart.Models.JsonResults;
using Sitecore.Commerce.XA.Feature.Cart.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.ExtensionMethods;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Providers;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Diagnostics;
using Sitecore.Feature.OrderReview.Models.JsonResults;
using Sitecore.XA.Foundation.SitecoreExtensions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.OrderReview.Repositories
{
    public class ReviewRepository : BaseCheckoutRepository, Sitecore.Feature.OrderReview.Repositories.IReviewRepository
    {
        public ReviewRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, ICartManager cartManager, IOrderManager orderManager, IAccountManager accountManager, IShippingManager shippingManager, ICheckoutStepProvider checkoutStepProvider)
          : base(modelProvider, storefrontContext, cartManager, orderManager, accountManager, checkoutStepProvider)
        {
            Assert.ArgumentNotNull((object)shippingManager, nameof(shippingManager));
            this.ShippingManager = shippingManager;
        }

        public IShippingManager ShippingManager { get; protected set; }

        public virtual ReviewRenderingModel GetReviewRenderingModel(IRendering rendering)
        {
            ReviewRenderingModel model = this.ModelProvider.GetModel<ReviewRenderingModel>();
            this.Init((BaseCommerceRenderingModel)model);
            model.Initialize(rendering);
            if (rendering.DataSourceItem != null)
                return model;
            model.ErrorMessage = "[Confirm] Please set the rendering data source appropriately.";
            return model;
        }

        public virtual OrderReviewDataJsonResult GetReviewData(IVisitorContext visitorContext)
        {
            OrderReviewDataJsonResult model = this.ModelProvider.GetModel<OrderReviewDataJsonResult>();
            if (!Context.PageMode.IsExperienceEditor)
            {
                try
                {
                    ManagerResponse<CartResult, Sitecore.Commerce.Entities.Carts.Cart> currentCart = this.CartManager.GetCurrentCart(visitorContext, this.StorefrontContext, true);
                    if (!currentCart.ServiceProviderResult.Success)
                    {
                        model.SetErrors((ServiceProviderResult)currentCart.ServiceProviderResult);
                        return model;
                    }
                    Sitecore.Commerce.Entities.Carts.Cart result = currentCart.Result;
                    if (result.Lines != null)
                    {
                        if (result.Lines.Any<CartLine>())
                        {
                            model.Initialize(result, visitorContext);
                            this.AddUserInfo(visitorContext, (BaseCheckoutDataJsonResult)model);
                            if (model.Success)
                            {
                                this.CheckForDigitalProductInCart(model, result);
                                model.CurrencyCode = result.CurrencyCode;
                                model.BenefitsData = new BenefitsDataJsonResult(result);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex, (object)this);
                    model.SetErrors(nameof(GetReviewData), ex);
                    return model;
                }
            }
            return model;
        }

        protected virtual void CheckForDigitalProductInCart(ReviewDataJsonResult jsonResult, Sitecore.Commerce.Entities.Carts.Cart cart)
        {
            ManagerResponse<GetShippingOptionsResult, List<ShippingOption>> shippingPreferences = this.ShippingManager.GetShippingPreferences(cart);
            if (!shippingPreferences.ServiceProviderResult.Success)
            {
                jsonResult.SetErrors((ServiceProviderResult)shippingPreferences.ServiceProviderResult);
            }
            else
            {
                foreach (LineShippingOption lineShippingOption in shippingPreferences.ServiceProviderResult.LineShippingPreferences.ToList<LineShippingOption>())
                {
                    List<ShippingOption> shippingOptions = lineShippingOption.ShippingOptions;
                    Func<ShippingOption, bool> func = (Func<ShippingOption, bool>)(o => o.ShippingOptionType.Value == CartFeatureConstants.DataTemplates.FixedValues.ElectronicDeliveryShippingOptionValue);
                    Func<ShippingOption, bool> predicate = func;
                    if (shippingOptions.Where<ShippingOption>(predicate).Count<ShippingOption>() >= 1)
                    {
                        jsonResult.HasDigitalProduct = true;
                        break;
                    }
                }
            }
        }
    }
}
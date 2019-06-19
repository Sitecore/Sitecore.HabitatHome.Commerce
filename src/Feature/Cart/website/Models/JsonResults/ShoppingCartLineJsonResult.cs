using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.XA.Feature.Cart.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Providers;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.HabitatHome.Feature.Cart.Models.JsonResults
{
    public class ShoppingCartLineJsonResult : CartLineJsonResult
    {
        public bool IsKit { get; set; }
        public bool IsBundle { get; set; }
        public string Comments { get; set; }
        public List<dynamic> RelatedKitProducts { get; set; }
        public List<dynamic> RelatedBundleProducts { get; set; }

        public ShoppingCartLineJsonResult(IStorefrontContext storefrontContext, IModelProvider modelProvider, ISearchManager searchManager, IContext context, ICurrencyFormatter currencyFormatter)
            : base(storefrontContext, modelProvider, searchManager, context, currencyFormatter)
        {
            Assert.ArgumentNotNull(modelProvider, nameof(modelProvider));
            this.ModelProvider = modelProvider;
        }

        public override void Initialize(CartLine cartLine, ShippingInfo shippingInfo, Sitecore.Commerce.Entities.Party party)
        {
            base.Initialize(cartLine, shippingInfo, party);
        }

        public void SetProductType(dynamic expandedCartLine)
        {
            this.IsKit = expandedCartLine.IsKit;
            this.IsBundle = expandedCartLine.IsBundle;
            this.Comments = expandedCartLine.Comments;
            this.RelatedKitProducts = new List<dynamic>();
            this.RelatedBundleProducts = new List<dynamic>();
            if (this.IsKit && !String.IsNullOrEmpty(this.Comments))
            {
                var relatedProductsList = this.Comments.Split('|');
                foreach(string product in relatedProductsList.Where(s=>!String.IsNullOrEmpty(s)))
                {
                    dynamic newRelatedProduct = new System.Dynamic.ExpandoObject();
                    newRelatedProduct.ProductId = product.Split(',')[0];
                    newRelatedProduct.DisplayName = product.Split(',')[1];
                    this.RelatedKitProducts.Add(newRelatedProduct);
                }
            }
            if (this.IsBundle && !String.IsNullOrEmpty(this.Comments))
            {
                var relatedProductsList = this.Comments.Split('|');
                foreach (string product in relatedProductsList.Where(s => !String.IsNullOrEmpty(s)))
                {
                    dynamic newRelatedProduct = new System.Dynamic.ExpandoObject();
                    newRelatedProduct.ProductId = product.Split(',')[0];
                    newRelatedProduct.DisplayName = product.Split(',')[1];
                    newRelatedProduct.ProductPrice = product.Split(',')[2];
                    this.RelatedBundleProducts.Add(newRelatedProduct);
                }
            }
        }
    }
}
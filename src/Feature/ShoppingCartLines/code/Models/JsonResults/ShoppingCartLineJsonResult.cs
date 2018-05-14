using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.ExtensionMethods;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Feature.ShoppingCartLines.Models.JsonResults
{
    public class ShoppingCartLineJsonResult : Sitecore.Commerce.XA.Foundation.CommerceEngine.Models.JsonResults.CartLineJsonResult
    {
        public bool IsKit { get; set; }
        public bool IsBundle { get; set; }
        public string Comments { get; set; }
        public List<dynamic> RelatedKitProducts { get; set; }
        public List<dynamic> RelatedBundleProducts { get; set; }
        public ShoppingCartLineJsonResult(IStorefrontContext storefrontContext, IModelProvider modelProvider, ISearchManager searchManager)
      : base(storefrontContext, modelProvider, searchManager)
        {
            Assert.ArgumentNotNull((object)modelProvider, nameof(modelProvider));
            this.ModelProvider = modelProvider;
        }
        public override void Initialize(CartLine cartLine)
        {
            base.Initialize(cartLine);
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
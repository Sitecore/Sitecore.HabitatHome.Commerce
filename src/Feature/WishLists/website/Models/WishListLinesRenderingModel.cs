using System;
using System.Collections.Generic;               
using Sitecore.Commerce.XA.Foundation.Common.Context;  
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Presentation;

namespace Sitecore.HabitatHome.Feature.WishLists.Models
{
    public class WishListLinesRenderingModel : BaseCommerceRenderingModel
    {
        public WishListLinesRenderingModel(IStorefrontContext storefrontContext)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            this.StorefrontContext = storefrontContext;
        }

        public IStorefrontContext StorefrontContext { get; set; }

        public string ProductTotalTooltip { get; set; }

        public string QuantityTooltip { get; set; }

        public string UnitPriceTooltip { get; set; }

        public string ProductDetailsTooltip { get; set; }

        public virtual Dictionary<string, string> VariantLabels { get; private set; }

        public void Initialize()
        {
            Rendering rendering = RenderingContext.CurrentOrNull.ValueOrDefault<RenderingContext, Rendering>((Func<RenderingContext, Rendering>)(context => context.Rendering));
            if (rendering != null)
            {
                Item obj = rendering.Item;
                this.ProductDetailsTooltip = obj["Product Details Tooltip"];
                this.UnitPriceTooltip = obj["Unit Price Tooltip"];
                this.QuantityTooltip = obj["Quantity Tooltip"];
                this.ProductTotalTooltip = obj["Total Tooltip"];
            }
            this.VariantLabels = new Dictionary<string, string>()
      {
        {
          "Color",
          this.StorefrontContext.GetVariantSpecificationLabels("Color", true)
        },
        {
          "Size",
          this.StorefrontContext.GetVariantSpecificationLabels("Size", true)
        },
        {
          "Style",
          this.StorefrontContext.GetVariantSpecificationLabels("Style", true)
        }
      };

        }
    }
}
using Sitecore.Commerce.Entities.WishLists;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.ExtensionMethods;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Diagnostics;
using Sitecore.Links;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Sitecore.Feature.WishLists.Models.JsonResults
{
    public class WishListLineJsonResult : BaseJsonResult
    {
        public WishListLineJsonResult(IStorefrontContext storefrontContext, IModelProvider modelProvider, ISearchManager searchManager)
      : base(storefrontContext)
        {
            Assert.ArgumentNotNull((object)modelProvider, nameof(modelProvider));
            Assert.ArgumentNotNull((object)searchManager, nameof(searchManager));
            this.ModelProvider = modelProvider;
            this.SearchManager = searchManager;
        }

        [ScriptIgnore]
        public IModelProvider ModelProvider { get; set; }

        [ScriptIgnore]
        public ISearchManager SearchManager { get; set; }

        public string ExternalWishListLineId { get; set; }
        
        public string Image { get; set; }

        public string DisplayName { get; set; }

        public string ProductUrl { get; set; }

        public string ColorInformation { get; set; }

        public string SizeInformation { get; set; }

        public string StyleInformation { get; set; }

        public string GiftCardAmountInformation { get; set; }

        public string Quantity { get; set; }

        public string LinePrice { get; set; }

        public string LineTotal { get; set; }

        public List<string> DiscountOfferNames { get; protected set; }


        public string ProductId { get; set; }

        public virtual void Initialize(WishListLine listLine)
        {
            this.ExternalWishListLineId = listLine.ExternalId;
            this.ProductId = listLine.Product.ProductId;
            this.Quantity = listLine.Quantity.ToString((IFormatProvider)Context.Language.CultureInfo);
            this.LinePrice = listLine.Product.Price.Amount.ToCurrency();
            this.LineTotal = listLine.Total.Amount.ToCurrency();
            this.SetLink(); 
        }

        public virtual void SetLink()
        {
            this.ProductUrl = this.ProductId.Equals(this.StorefrontContext.CurrentStorefront.GiftCardProductId, StringComparison.OrdinalIgnoreCase) ? this.StorefrontContext.CurrentStorefront.GiftCardPageLink : LinkManager.GetDynamicUrl(this.SearchManager.GetProduct(this.ProductId, this.StorefrontContext.CurrentStorefront.Catalog));
        }
    }
}
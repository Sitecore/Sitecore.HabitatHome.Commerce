using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Sitecore.Commerce.Entities.WishLists;      
using Sitecore.Commerce.XA.Foundation.Common.Context;             
using Sitecore.Commerce.XA.Foundation.Common.ExtensionMethods;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Resources.Media;

namespace Sitecore.HabitatHome.Feature.WishLists.Models.JsonResults
{
    public class WishListLineJsonResult : BaseJsonResult
    {
        public WishListLineJsonResult(IStorefrontContext storefrontContext, IModelProvider modelProvider, ISearchManager searchManager, IContext context)
            : base(context, storefrontContext)
        {
            Assert.ArgumentNotNull(modelProvider, nameof(modelProvider));
            Assert.ArgumentNotNull(searchManager, nameof(searchManager));
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
            this.Quantity = listLine.Quantity.ToString(Context.Language.CultureInfo);
            this.LinePrice = listLine.Product.Price.Amount.ToCurrency();
            this.LineTotal = this.LinePrice;
            this.DisplayName = listLine.Product.ProductName;            
            var prodId = this.ProductId.Split('|')[0];            
            var imageId = listLine.Product.GetPropertyValue("Image").ToString();
            this.SetImageUrl(imageId);
            this.SetLink(prodId);     
        }

        public virtual void SetLink(string productId)
        {
            if (productId.Equals(this.StorefrontContext.CurrentStorefront.GiftCardProductId,
                StringComparison.OrdinalIgnoreCase))
            {
                this.ProductUrl = this.StorefrontContext.CurrentStorefront.GiftCardPageLink;
            }
            else
            {
                var product = this.SearchManager.GetProduct(productId, this.StorefrontContext.CurrentStorefront.Catalog);
                this.ProductUrl = product != null ? LinkManager.GetDynamicUrl(product) : string.Empty;
            } 
        }
        public virtual void SetImageUrl(string imageId)
        {
            MediaItem imageItem = Sitecore.Context.Database.GetItem(imageId);           
            this.Image = imageItem != null ? MediaManager.GetMediaUrl(imageItem) :" ";
        }
    }
}
//using Sitecore.Commerce.XA.Feature.Catalog.Models.ProductLists;
using Sitecore.Commerce.XA.Foundation.CommerceEngine.Models;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect.Entities;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.ProductKit.Models.JsonResults;
using Sitecore.Links;
using Sitecore.Resources.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Feature.ProductKit.Managers
{
    public class RelatedProductsManager : IRelatedProductsManager
    {
        public RelatedProductsManager(IModelProvider modelProvider, IStorefrontContext storefrontContext, ISearchManager searchManager)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)modelProvider, nameof(modelProvider));
            Assert.ArgumentNotNull((object)searchManager, nameof(searchManager));
            this.SearchManager = searchManager;
            this.StorefrontContext = storefrontContext;
            this.ModelProvider = modelProvider;
        }
        public ISearchManager SearchManager { get; set; }
        public IStorefrontContext StorefrontContext { get; set; }
        public IModelProvider ModelProvider { get; set; }
        public IEnumerable<RelatedProductJsonResult> GetRelatedProducts(string productID)
        {
            List<RelatedProductJsonResult> relatedProducts = new List<RelatedProductJsonResult>();
            //Dictionary<string, List<ProductEntity>> productList = new RelatedProducts(this.ModelProvider, this.StorefrontContext).GetRelatedProductsLists(Sitecore.Context.Item, "RelatedProduct", 10, 0);

            CommerceStorefront currentStorefront = this.StorefrontContext.CurrentStorefront;
            string catalog = currentStorefront.Catalog;
            Item product = this.SearchManager.GetProduct(productID, catalog);

            var relatedProductsListFld = product.Fields["RelatedProduct"];
            if (relatedProductsListFld != null)
            {
                string productsFldVal = relatedProductsListFld.Value;
                List<string> productList;
                if (productsFldVal == null)
                    productList = (List<string>)null;
                else
                    productList = ((IEnumerable<string>)productsFldVal.Split('|')).ToList<string>();
                List<string> source = productList;
                if (source != null && source.Any<string>())
                {
                    source.Remove("");
                    foreach (string id in source)
                    {
                        Item relatedProduct = Context.Database.GetItem(new ID(id));                      
                       
                        RelatedProductJsonResult result = new RelatedProductJsonResult();
                        result.ProductName = relatedProduct.DisplayName;
                        MultilistField imagesFld = (MultilistField)relatedProduct.Fields["Images"];
                        if (imagesFld != null && !String.IsNullOrEmpty(imagesFld.Value))
                        {                             
                            MediaItem imageItem = Sitecore.Context.Database.GetItem(imagesFld.TargetIDs[0]);
                            result.Image = imageItem != null ? MediaManager.GetMediaUrl(imageItem) : " ";
                        }
                        result.Description = relatedProduct["Description"];
                        result.Quantity = 1;

                        result.ProductUrl = id.Equals(currentStorefront.GiftCardProductId, StringComparison.OrdinalIgnoreCase) ? currentStorefront.GiftCardPageLink : LinkManager.GetDynamicUrl(relatedProduct);

                        relatedProducts.Add(result);
                    }
                }
            }
            return relatedProducts;
        }
    }
}
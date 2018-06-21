using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Foundation.Catalog.Managers
{
    public class ProductRelatedContentManager : IProductRelatedContentManager
    {
        public ProductRelatedContentManager(IStorefrontContext storefrontContext, ISearchManager searchManager)
        {            
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)searchManager, nameof(searchManager));
            this.StorefrontContext = storefrontContext;
            this.SearchManager = searchManager;
        }
        public IStorefrontContext StorefrontContext { get; set; }

        public ISearchManager SearchManager { get; set; }
        public IEnumerable<Item> GetAssociatedProducts(string productID, string associatedProductsField)
        {
            List<Item> associatedProducts = new List<Item>();
            CommerceStorefront currentStorefront = this.StorefrontContext.CurrentStorefront;
            string catalog = currentStorefront.Catalog;
            Item product = this.SearchManager.GetProduct(productID, catalog);

            var associatedProductsListFld = product.Fields[associatedProductsField];
            if (associatedProductsListFld != null)
            {
                string productsFldVal = associatedProductsListFld.Value;
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
                        associatedProducts.Add(relatedProduct);
                    }
                }
            }
            return associatedProducts;
        }


        public IEnumerable<Item> GetProductContent(string productID)
        {
            List<Item> productRelatedContent = new List<Item>();
            string productContentParentPath = Configuration.Settings.GetSetting("Feature.ProductRelatedContent.ProductContent");
            if(!String.IsNullOrEmpty(productContentParentPath))
            {
                var productContentParent = Sitecore.Configuration.Factory.GetDatabase("master").GetItem(productContentParentPath);
                if (productContentParent != null)
                {
                    Item contentRepoParent = productContentParent.Axes.GetDescendants().Where(repo => repo.Name == productID).FirstOrDefault();
                    if(contentRepoParent != null)
                        productRelatedContent = contentRepoParent.Children.Where(i => i.TemplateName.ToLower() == "product related content item").ToList();
                }
            }
            return productRelatedContent;
        }
    }
}
//using Sitecore.Commerce.XA.Feature.Catalog.Models.ProductLists;
using Sitecore.Commerce.XA.Foundation.CommerceEngine.Models;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Entities;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Providers;
using Sitecore.Commerce.XA.Foundation.Common.Search;
using Sitecore.Commerce.XA.Foundation.Connect.Entities;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.ProductBundle.Managers;
using Sitecore.HabitatHome.Feature.ProductBundle.Models.JsonResults;
using Sitecore.Links;
using Sitecore.Resources.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Feature.ProductBundle.Managers
{
    public class RelatedProductsManager : IRelatedProductsManager
    {
        public RelatedProductsManager(IModelProvider modelProvider, IStorefrontContext storefrontContext, ISearchManager searchManager, IVariantDefinitionProvider variantDefinitionProvider, ISiteContext siteContext)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)modelProvider, nameof(modelProvider));
            Assert.ArgumentNotNull((object)searchManager, nameof(searchManager));
            this.SearchManager = searchManager;
            this.StorefrontContext = storefrontContext;
            this.ModelProvider = modelProvider;
            this.VariantDefinitionProvider = variantDefinitionProvider;
        }
        public ISearchManager SearchManager { get; set; }
        public IStorefrontContext StorefrontContext { get; set; }
        public IModelProvider ModelProvider { get; set; }
        public IVariantDefinitionProvider VariantDefinitionProvider { get; set; }


        public IEnumerable<RelatedProductJsonResult> GetRelatedProducts(string productId)
        {
            List<RelatedProductJsonResult> relatedProducts = new List<RelatedProductJsonResult>();            
            CommerceStorefront currentStorefront = this.StorefrontContext.CurrentStorefront;
            string catalog = currentStorefront.Catalog;            
            Item product = this.SearchManager.GetProduct(productId, catalog);           

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
                        Item cxtRelatedProduct = Context.Database.GetItem(new ID(id));
                        Assert.IsNotNull((object)cxtRelatedProduct, string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Unable to locate the product with id: {0}", (object)id));
                        var relProductId = cxtRelatedProduct["ProductId"];
                        Item relatedProduct = this.SearchManager.GetProduct(relProductId, catalog);

                        RelatedProductJsonResult result = new RelatedProductJsonResult(); 


                        result.ProductId = relProductId;
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
                        
                        string str1 = relatedProduct[Sitecore.Commerce.XA.Foundation.Common.Constants.ItemFieldNames.VariationProperties];

                        if (!string.IsNullOrWhiteSpace(str1))
                        {
                            string str2 = str1;
                            char[] chArray = new char[1] { '|' };
                            foreach (string labelKey in str2.Split(chArray))
                            {
                                VariantDefinitionEntity model = this.ModelProvider.GetModel<VariantDefinitionEntity>();
                                model.PropertyName = labelKey;
                                model.DisplayName = this.StorefrontContext.GetVariantSpecificationLabels(labelKey, true);
                                result.VariantDefinitionList.Add(model);                            }
                        }
                        if (relatedProduct.HasChildren)
                        {                            
                            if(result.VariantDefinitionList.Count() > 0)
                            {
                                foreach(var definition in result.VariantDefinitionList)
                                {
                                    List<RelatedProductVariantJsonResult> options = result.GetDistinctVariantPropertyValues(relatedProduct.Children.ToList(), definition.PropertyName);
                                    if(options.Count()>0)
                                    {
                                        result.VariantOptions.Add(new VariantOptionJsonResult
                                        {
                                            Label = definition.DisplayName,
                                            Options = options
                                        });
                                    }                                    
                                }                               
                            }                           
                        }
                        relatedProducts.Add(result);
                    }
                }
            }
            return relatedProducts;
        }
    }
}
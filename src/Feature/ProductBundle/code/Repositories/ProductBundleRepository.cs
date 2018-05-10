using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Search;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Commerce.XA.Foundation.Catalog.Managers;
using Sitecore.Feature.ProductBundle.Models.JsonResults;
using Sitecore.Feature.ProductBundle.Managers;
using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Common.Providers;

namespace Sitecore.Feature.ProductBundle.Repositories
{
    public class ProductBundleRepository : BaseCatalogRepository, IProductBundleRepository
    {
        public ProductBundleRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, ISiteContext siteContext, ISearchInformation searchInformation, ISearchManager searchManager, ICatalogManager catalogManager, ICatalogUrlManager catalogUrlManager, IRelatedProductsManager relatedProductsManager, IVariantDefinitionProvider variantDefinitionProvider) : base(modelProvider, storefrontContext, siteContext, searchInformation, searchManager, catalogManager, catalogUrlManager)
        {
            this.RelatedProductsManager = relatedProductsManager;
        }
        public IRelatedProductsManager RelatedProductsManager { get; protected set; }

        public virtual CatalogItemRenderingModel GetProductBundleRenderingModel(IVisitorContext visitorContext)
        {
            return this.GetProduct(visitorContext);
        }
        public virtual IEnumerable<RelatedProductJsonResult> GetRelatedProducts(IModelProvider modelProvider, IStorefrontContext storefrontContext, string productId)
        {

            return this.RelatedProductsManager.GetRelatedProducts(productId);
        }
    }
}
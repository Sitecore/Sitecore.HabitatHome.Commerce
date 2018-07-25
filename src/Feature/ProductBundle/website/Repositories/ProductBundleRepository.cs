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
using Sitecore.HabitatHome.Feature.ProductBundle.Models.JsonResults;
using Sitecore.HabitatHome.Feature.ProductBundle.Managers;
using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Common.Providers;
using Sitecore.Commerce.XA.Foundation.Common.Context;

namespace Sitecore.HabitatHome.Feature.ProductBundle.Repositories
{
    public class ProductBundleRepository : BaseCatalogRepository, IProductBundleRepository
    {
        public ProductBundleRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, ISiteContext siteContext, ISearchInformation searchInformation, ISearchManager searchManager, ICatalogManager catalogManager, ICatalogUrlManager catalogUrlManager, IRelatedProductsManager relatedProductsManager, IVariantDefinitionProvider variantDefinitionProvider, IContext context) 
            : base(modelProvider, storefrontContext, siteContext, searchInformation, searchManager, catalogManager, catalogUrlManager, context)
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
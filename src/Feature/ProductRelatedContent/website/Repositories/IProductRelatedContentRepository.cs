using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.HabitatHome.Feature.ProductRelatedContent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Feature.ProductRelatedContent.Repositories
{
    public interface IProductRelatedContentRepository
    {
        CatalogItemRenderingModel GetProductRelatedContentRenderingModel(IVisitorContext visitorContext);
        IEnumerable<RelatedProductJsonResult> GetRelatedProducts(IModelProvider modelProvider, IStorefrontContext storefrontContext, IVisitorContext visitorContext, string productId);
        IEnumerable<RelatedProductJsonResult> GetCrossSellProducts(IModelProvider modelProvider, IStorefrontContext storefrontContext, IVisitorContext visitorContext, string productId);
        IEnumerable<RelatedProductJsonResult> GetUpSellProducts(IModelProvider modelProvider, IStorefrontContext storefrontContext, IVisitorContext visitorContext, string productId);
        IEnumerable<ProductDocumentJsonResult> GetProductDocuments(IModelProvider modelProvider, IStorefrontContext storefrontContext, IVisitorContext visitorContext, string productId);
    }
}
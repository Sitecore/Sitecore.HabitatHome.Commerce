using Sitecore.Commerce.Entities.Products;
using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.HabitatHome.Feature.ProductBundle.Models.JsonResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.HabitatHome.Feature.ProductBundle.Repositories
{
    public interface IProductBundleRepository
    {
        IEnumerable<RelatedProductJsonResult> GetRelatedProducts(IModelProvider modelProvider, IStorefrontContext storefrontContext, string productID);
        CatalogItemRenderingModel GetProductBundleRenderingModel(IVisitorContext visitorContext);
    }
}
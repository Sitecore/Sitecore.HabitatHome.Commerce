using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.ProductBundle.Repositories;     
using System.Web.Mvc;

namespace Sitecore.HabitatHome.Feature.ProductBundle.Controllers
{
    public class ProductBundleController : BaseCommerceStandardController
    {
        public ProductBundleController(IStorefrontContext storefrontContext, IModelProvider modelProvider, IVisitorContext visitorContext, IProductBundleRepository productBundleRepository, IContext context)
            : base(storefrontContext, context)
        {
            Assert.ArgumentNotNull(storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull(modelProvider, nameof(modelProvider));
            Assert.ArgumentNotNull(productBundleRepository, nameof(productBundleRepository));

            _productBundleRepository = productBundleRepository;
            _visitorContext = visitorContext;
        }
        public IModelProvider ModelProvider { get; set; }
        private readonly IProductBundleRepository _productBundleRepository;
        private readonly IVisitorContext _visitorContext;

        public ActionResult ProductBundle()
        {
            return View("~/Views/ProductBundle/ProductBundle.cshtml", _productBundleRepository.GetProductBundleRenderingModel(_visitorContext));
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetRelatedProducts(string pid)
        {
            dynamic relatedProducts = _productBundleRepository.GetRelatedProducts(this.ModelProvider, this.StorefrontContext, pid);
            JsonResult baseJsonResult = this.Json(relatedProducts);

            return this.Json(baseJsonResult);
        }
    }
}
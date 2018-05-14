using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Diagnostics;
using Sitecore.Feature.ProductBundle.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sitecore.Feature.ProductBundle.Controllers
{
    public class ProductBundleController : BaseCommerceStandardController
    {
        public ProductBundleController(IStorefrontContext storefrontContext, IModelProvider modelProvider, IVisitorContext visitorContext, IProductBundleRepository productBundleRepository)
      : base(storefrontContext)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)modelProvider, nameof(modelProvider));
            Assert.ArgumentNotNull((object)productBundleRepository, nameof(productBundleRepository));
            this.ProductBundleRepository = productBundleRepository;
            _visitorContext = visitorContext;
        }
        public IModelProvider ModelProvider { get; set; }
        public IStorefrontContext StoreFrontContext { get; set; }
        private IProductBundleRepository ProductBundleRepository { get; set; }
        private readonly IVisitorContext _visitorContext;

        public ActionResult ProductBundle()
        {
            return (ActionResult)this.View("~/Views/ProductBundle/ProductBundle.cshtml", this.ProductBundleRepository.GetProductBundleRenderingModel(_visitorContext));
        }

        [AllowAnonymous]
        [HttpPost]
        //[OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetRelatedProducts(string pid)
        {
            JsonResult baseJsonResult;
            try
            {
                dynamic relatedProducts = this.ProductBundleRepository.GetRelatedProducts(this.ModelProvider, this.StorefrontContext, pid);
                baseJsonResult = this.Json(relatedProducts);
            }
            catch (Exception ex)
            {
                throw;
            }
            return this.Json((object)baseJsonResult);
        }
    }
}
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Feature.ProductKit.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.UI;

namespace Sitecore.Feature.ProductKit.Controllers
{
    public class ProductKitController : BaseCommerceStandardController
    {
        public ProductKitController(IStorefrontContext storefrontContext, IModelProvider modelProvider, IVisitorContext visitorContext, IProductKitRepository productKitRepository)
      : base(storefrontContext)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)modelProvider, nameof(modelProvider));
            Assert.ArgumentNotNull((object)productKitRepository, nameof(productKitRepository));
            this.ProductKitRepository = productKitRepository;
            _visitorContext = visitorContext;
        }
        public IModelProvider ModelProvider { get; set; }
        public IStorefrontContext StoreFrontContext { get; set; }
        private IProductKitRepository ProductKitRepository { get; set; }
        private readonly IVisitorContext _visitorContext;

        public ActionResult ProductKit()
        {
            return (ActionResult)this.View("~/Views/ProductKit/ProductKit.cshtml", this.ProductKitRepository.GetProductKitRenderingModel(_visitorContext));
        }

        [AllowAnonymous]
        [HttpPost]
        //[OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetRelatedProducts(string pid)
        {
            JsonResult baseJsonResult;            
            try
            {
                dynamic relatedProducts = this.ProductKitRepository.GetRelatedProducts(this.ModelProvider, this.StorefrontContext, pid);
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
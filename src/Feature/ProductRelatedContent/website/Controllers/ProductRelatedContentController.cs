using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.ProductRelatedContent.Repositories;
using Sitecore.Commerce.XA.Foundation.Common.Context;

namespace Sitecore.HabitatHome.Feature.ProductRelatedContent.Controllers
{
    public class ProductRelatedContentController : BaseCommerceStandardController
    {
        public ProductRelatedContentController(IContext context, IStorefrontContext storefrontContext, IVisitorContext visitorContext, IProductRelatedContentRepository productRelatedContentRepository) 
            : base(storefrontContext, context)
        {
            Assert.ArgumentNotNull((object)storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));            
            _visitorContext = visitorContext;
            _productRelatedContentRepository = productRelatedContentRepository;
        }
        private readonly IProductRelatedContentRepository _productRelatedContentRepository;
        private readonly IVisitorContext _visitorContext;
        public IModelProvider ModelProvider { get; set; }
        public ActionResult ProductRelatedContent()
        {            
            return View("~/Views/ProductRelatedContent/ProductRelatedContent.cshtml", _productRelatedContentRepository.GetProductRelatedContentRenderingModel(_visitorContext));
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetRelatedProducts(string pid)
        {
            dynamic relatedProducts = _productRelatedContentRepository.GetRelatedProducts(this.ModelProvider, this.StorefrontContext, _visitorContext, pid);
            JsonResult baseJsonResult = this.Json(relatedProducts);
            return this.Json(baseJsonResult);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetCrossSellProducts(string pid)
        {
            dynamic relatedProducts = _productRelatedContentRepository.GetCrossSellProducts(this.ModelProvider, this.StorefrontContext, _visitorContext, pid);
            JsonResult baseJsonResult = this.Json(relatedProducts);
            return this.Json(baseJsonResult);
        }

        public JsonResult GetUpSellProducts(string pid)
        {
            dynamic relatedProducts = _productRelatedContentRepository.GetUpSellProducts(this.ModelProvider, this.StorefrontContext, _visitorContext, pid);
            JsonResult baseJsonResult = this.Json(relatedProducts);
            return this.Json(baseJsonResult);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetProductDocuments(string pid)
        {
            dynamic productDocuments = _productRelatedContentRepository.GetProductDocuments(this.ModelProvider, this.StorefrontContext, _visitorContext, pid);
            JsonResult baseJsonResult = this.Json(productDocuments);
            return this.Json(baseJsonResult);
        }
    }
}
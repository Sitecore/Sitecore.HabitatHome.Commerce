using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Commerce.XA.Feature.Cart.Repositories;   
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.WishLists.Repositories;

namespace Sitecore.HabitatHome.Feature.WishLists.Controllers
{
    public class WishListLinesController : BaseCommerceStandardController
    {
        
        protected IWishListLinesRepository WishListLinesRepository { get; set; }
        public IVisitorContext VisitorContext { get; protected set; }
        public IModelProvider ModelProvider { get; protected set; }
        public IAddToCartRepository AddToCartRepository { get; protected set; }

        public WishListLinesController(IStorefrontContext storefrontContext, IModelProvider modelProvider, IVisitorContext visitorContext, IAddToCartRepository addToCartRepository, IWishListLinesRepository wishListLinesRepository, IContext context) 
            : base(storefrontContext, context)
        {

            Assert.ArgumentNotNull((object)modelProvider, nameof(modelProvider));
            Assert.ArgumentNotNull((object)visitorContext, nameof(visitorContext));

            this.ModelProvider = modelProvider;
            this.VisitorContext = visitorContext;
            this.WishListLinesRepository = wishListLinesRepository;
            this.AddToCartRepository = addToCartRepository;
        }

        [HttpGet]
        public ActionResult WishListLines()
        {
            return (ActionResult)this.View("~/Views/Wishlists/WishListLines.cshtml", (object)this.WishListLinesRepository.GetWishListLinesModel()); ;
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetWishList()
        {
            BaseJsonResult baseJsonResult;
            try
            {
                string catalogName = this.StorefrontContext.CurrentStorefront.Catalog;
                baseJsonResult = this.WishListLinesRepository.GetWishList(this.StorefrontContext, this.VisitorContext);
            }
            catch (Exception ex)
            {
                baseJsonResult = this.ModelProvider.GetModel<BaseJsonResult>();
                baseJsonResult.SetErrors(nameof(AddWishListLine), ex);
            }
            return this.Json(baseJsonResult);
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult AddWishListLine(string productId, string variantId, Decimal quantity)
        {
            BaseJsonResult baseJsonResult;
            try
            {
                string catalogName = this.StorefrontContext.CurrentStorefront.Catalog;
                baseJsonResult = this.WishListLinesRepository.AddWishListLine(this.StorefrontContext, this.VisitorContext, catalogName, productId, variantId, quantity);
            }
            catch (Exception ex)
            {
                baseJsonResult = this.ModelProvider.GetModel<BaseJsonResult>();
                baseJsonResult.SetErrors(nameof(AddWishListLine), ex);
            }
            return this.Json(baseJsonResult);
        }


        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult RemoveWishListLines(string lineIds)
        {
            BaseJsonResult baseJsonResult;
            try
            {
                List<string> ids = new List<string>();
                ids.Add(lineIds);
                baseJsonResult = this.WishListLinesRepository.RemoveWishListLines(this.StorefrontContext, this.VisitorContext, ids);
            }
            catch (Exception ex)
            {
                baseJsonResult = this.ModelProvider.GetModel<BaseJsonResult>();
                baseJsonResult.SetErrors(nameof(AddWishListLine), ex);
            }
            return this.Json(baseJsonResult);
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult AddWishListLineToCart(string productId, string variantId, Decimal quantity)
        {
            BaseJsonResult baseJsonResult;
            try
            {
                string catalogName = this.StorefrontContext.CurrentStorefront.Catalog;
                baseJsonResult = this.AddToCartRepository.AddLineItemsToCart(this.StorefrontContext, this.VisitorContext, catalogName, productId, variantId, quantity);
            }
            catch (Exception ex)
            {
                baseJsonResult = this.ModelProvider.GetModel<BaseJsonResult>();
                baseJsonResult.SetErrors(nameof(AddWishListLine), ex);
            }
            return this.Json(baseJsonResult);
        } 
    }
}
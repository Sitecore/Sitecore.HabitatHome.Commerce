using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Commerce.XA.Feature.Cart.Repositories;   
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
        private readonly IWishListLinesRepository _wishListLinesRepository;
        private readonly IVisitorContext _visitorContext;
        private readonly IModelProvider _modelProvider;
        private readonly IAddToCartRepository _addToCartRepository;

        public WishListLinesController(IStorefrontContext storefrontContext, IModelProvider modelProvider, IVisitorContext visitorContext, IAddToCartRepository addToCartRepository, IWishListLinesRepository wishListLinesRepository, IContext context) 
            : base(storefrontContext, context)
        {                                
            Assert.ArgumentNotNull(modelProvider, nameof(modelProvider));
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));

            _modelProvider = modelProvider;
            _visitorContext = visitorContext;
            _wishListLinesRepository = wishListLinesRepository;
            _addToCartRepository = addToCartRepository;
        }

        [HttpGet]
        public ActionResult WishListLines()
        {
            return View("~/Views/Wishlists/WishListLines.cshtml", _wishListLinesRepository.GetWishListLinesModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult GetWishList()
        {
            BaseJsonResult baseJsonResult;
            try
            {                                                                               
                baseJsonResult = _wishListLinesRepository.GetWishList(StorefrontContext, _visitorContext);
            }
            catch (Exception ex)
            {
                baseJsonResult = _modelProvider.GetModel<BaseJsonResult>();
                baseJsonResult.SetErrors(nameof(AddWishListLine), ex);
            }
            return Json(baseJsonResult);
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult AddWishListLine(string productId, string variantId, Decimal quantity)
        {
            BaseJsonResult baseJsonResult;
            try
            {
                string catalogName = StorefrontContext.CurrentStorefront.Catalog;
                baseJsonResult = _wishListLinesRepository.AddWishListLine(StorefrontContext, _visitorContext, catalogName, productId, variantId, quantity);
            }
            catch (Exception ex)
            {
                baseJsonResult = _modelProvider.GetModel<BaseJsonResult>();
                baseJsonResult.SetErrors(nameof(AddWishListLine), ex);
            }
            return Json(baseJsonResult);
        }
                    
        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult RemoveWishListLines(string lineIds)
        {
            BaseJsonResult baseJsonResult;
            try
            {
                List<string> ids = new List<string> {lineIds};
                baseJsonResult = _wishListLinesRepository.RemoveWishListLines(StorefrontContext, _visitorContext, ids);
            }
            catch (Exception ex)
            {
                baseJsonResult = _modelProvider.GetModel<BaseJsonResult>();
                baseJsonResult.SetErrors(nameof(AddWishListLine), ex);
            }
            return Json(baseJsonResult);
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult AddWishListLineToCart(string productId, string variantId, Decimal quantity)
        {
            BaseJsonResult baseJsonResult;
            try
            {
                string catalogName = StorefrontContext.CurrentStorefront.Catalog;
                baseJsonResult = _addToCartRepository.AddLineItemsToCart(StorefrontContext, _visitorContext, catalogName, productId, variantId, quantity);
            }
            catch (Exception ex)
            {
                baseJsonResult = _modelProvider.GetModel<BaseJsonResult>();
                baseJsonResult.SetErrors(nameof(AddWishListLine), ex);
            }
            return Json(baseJsonResult);
        } 
    }
}
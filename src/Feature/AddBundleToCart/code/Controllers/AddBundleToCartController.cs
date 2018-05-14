using Sitecore.Commerce.XA.Feature.Cart.Controllers;
using Sitecore.Commerce.XA.Feature.Cart.Models;
using Sitecore.Commerce.XA.Feature.Cart.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Diagnostics;
using Sitecore.Feature.ProductBundle.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Sitecore.Feature.AddBundleToCart.Controllers
{
    public class AddBundleToCartController : CartController
    {
        public AddBundleToCartController(IStorefrontContext storefrontContext, IModelProvider modelProvider, IAddToCartRepository addToCartRepository, IMinicartRepository minicartRepository, IPromotionCodesRepository promotionCodesRepository, IShoppingCartLinesRepository shoppingCartLinesRepository, IShoppingCartTotalRepository shoppingCartTotalRepository, IVisitorContext visitorContext, ISiteContext siteContext, IProductBundleRepository productBundleRepository, ISearchManager searchManager)
            : base(storefrontContext, modelProvider, addToCartRepository, minicartRepository, promotionCodesRepository, shoppingCartLinesRepository, shoppingCartTotalRepository, visitorContext)
        {
            Assert.ArgumentNotNull((object) productBundleRepository, nameof(productBundleRepository));
            this.ProductBundleRepository = productBundleRepository;
            this.SearchManager = searchManager;
            _visitorContext = visitorContext;
        }

        private IProductBundleRepository ProductBundleRepository { get; set; }
        public ISearchManager SearchManager { get; set; }
        private readonly IVisitorContext _visitorContext;
        public ActionResult AddBundleToCart()
        {
            AddToCartRenderingModel addToCartModel = AddToCartRepository.GetAddToCartModel();
            addToCartModel.CatalogName = this.StorefrontContext.CurrentStorefront.Catalog;

            return base.View("~/Views/AddBundleToCart/AddBundleToCart.cshtml", this.ProductBundleRepository.GetProductBundleRenderingModel(_visitorContext));
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult AddBundleCartLine(string addToCart_CatalogName, string addToCart_ProductId, string addtocart_relatedvariantids, Decimal quantity)
        {
            BaseJsonResult baseJsonResult = this.ModelProvider.GetModel<BaseJsonResult>(); ;
            try
            {               
            
                var varIds = addtocart_relatedvariantids.Split(',');
                if (varIds.Count() > 0)
                {
                    foreach (var addToCart_VariantId in varIds.Where(id => !String.IsNullOrEmpty(id)))
                    {                       

                        var bundleProduct = this.SearchManager.GetProduct(addToCart_VariantId.Contains('|') ? addToCart_VariantId.Split('|')[0] : addToCart_VariantId, addToCart_CatalogName);
                        string bundleProductId = addToCart_VariantId;
                        string bundleVariantID = "-1";

                        if (bundleProduct.HasChildren)
                        {
                            bundleVariantID = bundleProduct.Children[0].Name;
                        }
                        if (addToCart_VariantId.Contains('|'))
                        {
                            bundleProductId = addToCart_VariantId.Split('|')[0];
                            bundleVariantID = addToCart_VariantId.Split('|')[1];
                        }

                        baseJsonResult = this.AddToCartRepository.AddLineItemsToCart(this.StorefrontContext, this.VisitorContext, addToCart_CatalogName, bundleProductId, bundleVariantID, quantity);
                    }
                }                
                
                var baseProduct = this.SearchManager.GetProduct(addToCart_ProductId, addToCart_CatalogName);
                string variantID = "-1";

                if (baseProduct.HasChildren)
                {
                    variantID = baseProduct.Children[0].Name;
                }
                baseJsonResult = this.AddToCartRepository.AddLineItemsToCart(this.StorefrontContext, this.VisitorContext, addToCart_CatalogName, addToCart_ProductId, variantID, quantity);

                baseJsonResult.Success = true;
            }
            catch (Exception ex)
            {
                baseJsonResult = this.ModelProvider.GetModel<BaseJsonResult>();
                baseJsonResult.SetErrors(nameof(AddCartLine), ex);
            }
            return this.Json((object)baseJsonResult);
        }
    }
}
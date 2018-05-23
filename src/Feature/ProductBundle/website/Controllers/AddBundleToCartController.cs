using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Commerce.XA.Feature.Cart.Controllers;
using Sitecore.Commerce.XA.Feature.Cart.Models;
using Sitecore.Commerce.XA.Feature.Cart.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.ProductBundle.Repositories;

namespace Sitecore.HabitatHome.Feature.ProductBundle.Controllers
{
    public class AddBundleToCartController : CartController
    {                                
        private readonly IProductBundleRepository _productBundleRepository;
        private readonly ISearchManager _searchManager;
        private readonly IVisitorContext _visitorContext;

        public AddBundleToCartController(IStorefrontContext storefrontContext, IModelProvider modelProvider, IAddToCartRepository addToCartRepository, IMinicartRepository minicartRepository, IPromotionCodesRepository promotionCodesRepository, IShoppingCartLinesRepository shoppingCartLinesRepository, IShoppingCartTotalRepository shoppingCartTotalRepository, IVisitorContext visitorContext, ISiteContext siteContext, IProductBundleRepository productBundleRepository, ISearchManager searchManager, IProductBundleRepository productBundleRepository1)
            : base(storefrontContext, modelProvider, addToCartRepository, minicartRepository, promotionCodesRepository, shoppingCartLinesRepository, shoppingCartTotalRepository, visitorContext)
        {
            Assert.ArgumentNotNull(productBundleRepository, nameof(productBundleRepository));
                                                                         
            _searchManager = searchManager;
            _productBundleRepository = productBundleRepository1;
            _visitorContext = visitorContext;
        }

        public ActionResult AddBundleToCart()
        {
            AddToCartRenderingModel addToCartModel = AddToCartRepository.GetAddToCartModel();
            addToCartModel.CatalogName = StorefrontContext.CurrentStorefront.Catalog;

            return View("~/Views/ProductBundle/AddBundleToCart.cshtml", _productBundleRepository.GetProductBundleRenderingModel(_visitorContext));
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are the names in the HTML")]
        public JsonResult AddBundleCartLine(string addToCart_CatalogName, string addToCart_ProductId, string addtocart_relatedvariantids, Decimal quantity)
        {
            BaseJsonResult baseJsonResult = this.ModelProvider.GetModel<BaseJsonResult>();
            try
            {        
                string[] relatedVariantIds = addtocart_relatedvariantids.Split(',');
                if (relatedVariantIds.Any())
                {
                    foreach (string relatedVariantId in relatedVariantIds.Where(id => !String.IsNullOrEmpty(id)))
                    {                       

                        var bundleProduct = _searchManager.GetProduct(relatedVariantId.Contains('|') ? relatedVariantId.Split('|')[0] : relatedVariantId, addToCart_CatalogName);
                        string bundleProductId = relatedVariantId;
                        string bundleVariantId = "-1";

                        if (bundleProduct.HasChildren)
                        {
                            bundleVariantId = bundleProduct.Children[0].Name;
                        }
                        if (relatedVariantId.Contains('|'))
                        {
                            bundleProductId = relatedVariantId.Split('|')[0];
                            bundleVariantId = relatedVariantId.Split('|')[1];
                        }

                        baseJsonResult = this.AddToCartRepository.AddLineItemsToCart(this.StorefrontContext, this.VisitorContext, addToCart_CatalogName, bundleProductId, bundleVariantId, quantity);
                    }
                }                
                
                Item baseProduct = _searchManager.GetProduct(addToCart_ProductId, addToCart_CatalogName);
                string variantId = "-1";

                if (baseProduct.HasChildren)
                {
                    variantId = baseProduct.Children[0].Name;
                }

                baseJsonResult = this.AddToCartRepository.AddLineItemsToCart(this.StorefrontContext, this.VisitorContext, addToCart_CatalogName, addToCart_ProductId, variantId, quantity);
                baseJsonResult.Success = true;
            }
            catch (Exception ex)
            {
                baseJsonResult = this.ModelProvider.GetModel<BaseJsonResult>();
                baseJsonResult.SetErrors(nameof(AddCartLine), ex);
            }
            return this.Json(baseJsonResult);
        }
    }
}
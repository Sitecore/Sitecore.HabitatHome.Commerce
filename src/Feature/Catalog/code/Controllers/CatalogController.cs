using Sitecore.Commerce.XA.Feature.Cart.Repositories;
using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace Sitecore.Feature.Catalog.Controllers
{
    public class CatalogWithCartController : Commerce.XA.Feature.Catalog.Controllers.CatalogController
    {
        public IAddToCartRepository AddToCartRepository { get; protected set; }

        public IVisitorContext VisitorContext { get; protected set; }

        public CatalogWithCartController(IModelProvider modelProvider, IProductListHeaderRepository productListHeaderRepository,
            IProductListRepository productListRepository, IPromotedProductsRepository promotedProductsRepository,
            IProductInformationRepository productInformationRepository,
            IProductImagesRepository productImagesRepository, IProductInventoryRepository productInventoryRepository,
            IProductPriceRepository productPriceRepository, IProductVariantsRepository productVariantsRepository,
            IProductListPagerRepository productListPagerRepository, IProductFacetsRepository productFacetsRepository,
            IProductListSortingRepository productListSortingRepository,
            IProductListPageInfoRepository productListPageInfoRepository,
            IProductListItemsPerPageRepository productListItemsPerPageRepository,
            ICatalogItemContainerRepository catalogItemContainerRepository,
            IAddToCartRepository addToCartRepository,
            IVisitorContext visitorContext,
            IStorefrontContext storefrontContext, ISiteContext siteContext)
            : base(modelProvider, productListHeaderRepository, productListRepository, promotedProductsRepository,
                productInformationRepository, productImagesRepository, productInventoryRepository,
                productPriceRepository, productVariantsRepository, productListPagerRepository, productFacetsRepository,
                productListSortingRepository, productListPageInfoRepository, productListItemsPerPageRepository,
                catalogItemContainerRepository, storefrontContext, siteContext)
        {
            this.AddToCartRepository = addToCartRepository;
            this.VisitorContext = visitorContext;
        }

        public ActionResult AddToCart()
        {
            return (ActionResult)this.View(this.GetRenderingView(nameof(AddToCart)), (object)this.AddToCartRepository.GetAddToCartModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult AddCartLine(string addToCart_CatalogName, string addToCart_ProductId, string addToCart_VariantId, Decimal quantity)
        {
            BaseJsonResult baseJsonResult;
            try
            {
                baseJsonResult = this.AddToCartRepository.AddLineItemsToCart(this.StorefrontContext, this.VisitorContext, addToCart_CatalogName, addToCart_ProductId, addToCart_VariantId, quantity);
            }
            catch (Exception ex)
            {
                baseJsonResult = this.ModelProvider.GetModel<BaseJsonResult>();
                baseJsonResult.SetErrors(nameof(AddCartLine), ex);
            }
            return this.Json((object)baseJsonResult);
        }

        protected override string GetIndexViewName()
        {
            return "~/Views/Catalog/ProductListWithCart.cshtml";
        }
    }
}
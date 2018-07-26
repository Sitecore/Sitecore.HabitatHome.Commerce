using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Commerce.XA.Feature.Catalog.Repositories;   
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Arguments;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.DependencyInjection;
using Sitecore.HabitatHome.Feature.Catalog.Models;
using Sitecore.HabitatHome.Feature.Catalog.Repositories;

namespace Sitecore.HabitatHome.Feature.Catalog.Controllers
{
    public class PurchasableProductListController : Commerce.XA.Feature.Catalog.Controllers.CatalogController
    {
        private readonly ICartManager _cartManager;
        private readonly IVisitorContext _visitorContext;
        private readonly IPurchasableProductListRepository _purchasableProductListRepository;

        public PurchasableProductListController(IModelProvider modelProvider, IProductListHeaderRepository productListHeaderRepository,
            IProductListRepository productListRepository, IPromotedProductsRepository promotedProductsRepository,
            IProductInformationRepository productInformationRepository,
            IProductImagesRepository productImagesRepository, IProductInventoryRepository productInventoryRepository,
            IProductPriceRepository productPriceRepository, IProductVariantsRepository productVariantsRepository,
            IProductListPagerRepository productListPagerRepository, IProductFacetsRepository productFacetsRepository,
            IProductListSortingRepository productListSortingRepository,
            IProductListPageInfoRepository productListPageInfoRepository,
            IProductListItemsPerPageRepository productListItemsPerPageRepository,
            ICatalogItemContainerRepository catalogItemContainerRepository,
            ICartManager cartManager, IVisitorContext visitorContext, 
            IVisitedCategoryPageRepository visitedCategoryPageRepository, IVisitedProductDetailsPageRepository visitedProductDetailsPageRepository, 
            ISearchInitiatedRepository searchInitiatedRepository, IStorefrontContext storefrontContext, 
            ISiteContext siteContext, IContext context, IPurchasableProductListRepository purchasableProductListRepository)
            : base(modelProvider, productListHeaderRepository, productListRepository, promotedProductsRepository,
                productInformationRepository, productImagesRepository, productInventoryRepository, productPriceRepository,
                productVariantsRepository, productListPagerRepository, productFacetsRepository, productListSortingRepository, 
                productListPageInfoRepository, productListItemsPerPageRepository, catalogItemContainerRepository,
                visitedCategoryPageRepository,  visitedProductDetailsPageRepository,  searchInitiatedRepository, 
                storefrontContext, siteContext, context) 
        {
            _cartManager = cartManager;
            _visitorContext = visitorContext;
            _purchasableProductListRepository = purchasableProductListRepository;
        }
              
        [HttpPost]                                                                   
        public JsonResult AddCartLine(IContext context,string productId, string variantId, string quantity = "1.0")
        {   
            BaseJsonResult model = new BaseJsonResult(context,StorefrontContext);
            CommerceStorefront currentStorefront = StorefrontContext.CurrentStorefront;
            ManagerResponse<CartResult, Cart> currentCart = _cartManager.GetCurrentCart(_visitorContext, StorefrontContext, false);
            if (!currentCart.ServiceProviderResult.Success || currentCart.Result == null)
            {
                string systemMessage = StorefrontContext.GetSystemMessage("Cart Not Found Error", true);
                currentCart.ServiceProviderResult.SystemMessages.Add(new SystemMessage
                {
                    Message = systemMessage
                });
                model.SetErrors(currentCart.ServiceProviderResult);
                return model;
            }

            List<CartLineArgument> list = new List<CartLineArgument>();
            list.Add(new CartLineArgument
            {
                CatalogName = StorefrontContext.CurrentStorefront.Catalog,
                ProductId = productId, 
                VariantId = variantId,
                Quantity = decimal.Parse(quantity)
            });

            ManagerResponse<CartResult, Cart> managerResponse = _cartManager.AddLineItemsToCart(currentStorefront, _visitorContext, currentCart.Result, list);
            if (!managerResponse.ServiceProviderResult.Success)
            {
                model.SetErrors(managerResponse.ServiceProviderResult);
                return model;
            }
            model.Success = true;
            return base.Json(model);
        }                                                                      

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult GetPurchasableProductList([Bind(Prefix = "q")] string searchKeyword, [Bind(Prefix = "pg")] int? pageNumber, [Bind(Prefix = "f")] string facetValues, [Bind(Prefix = "s")] string sortField, [Bind(Prefix = "ps")] int? pageSize, [Bind(Prefix = "sd")] Sitecore.Commerce.XA.Foundation.Common.Constants.SortDirection? sortDirection, [Bind(Prefix = "cci")] string currentCatalogItemId, [Bind(Prefix = "ci")] string currentItemId)
        {
            IVisitorContext service = ServiceLocator.ServiceProvider.GetService<IVisitorContext>();
            PurchasableProductListJsonResult productListJsonResult = _purchasableProductListRepository.GetPurchasableProductListJsonResult(service, currentItemId, currentCatalogItemId, searchKeyword, pageNumber, facetValues, sortField, pageSize, sortDirection);
            return base.Json(productListJsonResult);
        }

        public ActionResult PurchasableProductList()
        {
            return View("~/Views/Catalog/PurchasableProductList.cshtml", ProductListRepository.GetProductListRenderingModel());
        }       
    }
}
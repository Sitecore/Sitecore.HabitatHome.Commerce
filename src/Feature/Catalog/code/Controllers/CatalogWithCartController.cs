using Sitecore.Commerce.Engine.Connect;
using Sitecore.Commerce.Engine.Connect.Interfaces;
using Sitecore.Commerce.Engine.Connect.Search;
using Sitecore.Commerce.XA.Feature.Cart.Repositories;
using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Entities;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Security;
using System;
using System.Linq;
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
        public JsonResult AddCartLine(string addToCart_CatalogName, string addToCart_ProductId, string addToCart_VariantId, Decimal quantity = 1)
        {
            //var productRenderingModel = this.ProductVariantsRepository.GetProductVariantRenderingModel(this.VisitorContext);
            //var variant = productRenderingModel?.Variants?.FirstOrDefault()?.VariantId;
            //addToCart_VariantId = variant;

            //Get product with search
            using (IProviderSearchContext searchContext = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>()
                .GetIndex(addToCart_CatalogName).CreateSearchContext(SearchSecurityOptions.Default))
            {
                var query = searchContext.GetQueryable<CommerceSellableItemSearchResultItem>().Where(item =>
                    item.CommerceSearchItemType == "SellableItem" && item.Language == Context.Language.Name &&
                    item.Name == addToCart_ProductId.ToLowerInvariant());

                var result = query.GetResults<CommerceSellableItemSearchResultItem>().Hits.FirstOrDefault()?.Document;
                var productItem = Sitecore.Context.Database.GetItem(result?.SitecoreId);

                if (productItem != null && productItem.HasChildren)
                {
                    VariantEntity model = this.ModelProvider.GetModel<VariantEntity>();
                    model.Initialize(productItem.Children.FirstOrDefault());
                    addToCart_VariantId = model.VariantId;
                }
                else
                {
                    addToCart_VariantId = addToCart_ProductId;
                }
            }

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

        public ActionResult ProductListWithCart()
        {
            return (ActionResult)this.View("~/Views/Catalog/ProductListWithCart.cshtml", (object)this.ProductListRepository.GetProductListRenderingModel());
        }
    }
}
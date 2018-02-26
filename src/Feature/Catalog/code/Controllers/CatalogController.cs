using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;

namespace Sitecore.Feature.Catalog.Controllers
{
    public class CatalogWithCartController : Commerce.XA.Feature.Catalog.Controllers.CatalogController
    {
        public CatalogWithCartController(IModelProvider modelProvider, IProductListHeaderRepository productListHeaderRepository,
            IProductListRepository productListRepository, IPromotedProductsRepository promotedProductsRepository,
            IProductInformationRepository productInformationRepository,
            IProductImagesRepository productImagesRepository, IProductInventoryRepository productInventoryRepository,
            IProductPriceRepository productPriceRepository, IProductVariantsRepository productVariantsRepository,
            IProductListPagerRepository productListPagerRepository, IProductFacetsRepository productFacetsRepository,
            IProductListSortingRepository productListSortingRepository,
            IProductListPageInfoRepository productListPageInfoRepository,
            IProductListItemsPerPageRepository productListItemsPerPageRepository,
            ICatalogItemContainerRepository catalogItemContainerRepository, IStorefrontContext storefrontContext,
            ISiteContext siteContext)
            : base(modelProvider, productListHeaderRepository, productListRepository, promotedProductsRepository,
                productInformationRepository, productImagesRepository, productInventoryRepository,
                productPriceRepository, productVariantsRepository, productListPagerRepository, productFacetsRepository,
                productListSortingRepository, productListPageInfoRepository, productListItemsPerPageRepository,
                catalogItemContainerRepository, storefrontContext, siteContext)
        {
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Entities.Inventory;
using Sitecore.Commerce.XA.Feature.Catalog;
using Sitecore.Commerce.XA.Feature.Catalog.Cache;
using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
using Sitecore.Commerce.XA.Foundation.Catalog.Managers;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Repositories;
using Sitecore.Commerce.XA.Foundation.Common.Search;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Entities;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Feature.Catalog.Models;

namespace Sitecore.HabitatHome.Feature.Catalog.Repositories
{
    public class PurchasableProductListRepository : ProductListRepository, IPurchasableProductListRepository
    {                                                             
        private readonly IStorefrontContext _storefrontContext;

        public PurchasableProductListRepository(IModelProvider modelProvider, IStorefrontContext storefrontContext, ISiteContext siteContext, ISearchInformation searchInformation, ISearchManager searchManager, ICatalogManager catalogManager, IInventoryManager inventoryManager, ICatalogUrlManager catalogUrlManager, IContext context, IProductListCacheProvider productListCacheProvider, IBulkManager bulkManager)
        : base(modelProvider, storefrontContext, siteContext, searchInformation, searchManager, catalogManager, inventoryManager, catalogUrlManager, context, productListCacheProvider, bulkManager)
        {                                            
            _storefrontContext = storefrontContext;
        }

        public PurchasableProductListJsonResult GetPurchasableProductListJsonResult(IVisitorContext visitorContext, string currentItemId, string currentCatalogItemId, string searchKeyword, int? pageNumber, string facetValues, string sortField, int? pageSize, SortDirection? sortDirection)
        {
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            PurchasableProductListJsonResult productListJsonResult = base.ModelProvider.GetModel<PurchasableProductListJsonResult>();
            Item item = (!string.IsNullOrEmpty(currentCatalogItemId)) ? Context.Database.GetItem(currentCatalogItemId) : null;
            if (string.IsNullOrEmpty(currentCatalogItemId) && !string.IsNullOrEmpty(searchKeyword))
            {
                item = base.StorefrontContext.CurrentStorefront.CatalogItem;
            }
            if (item != null)
            {
                Item currentItem = Context.Database.GetItem(currentItemId);
                base.SiteContext.CurrentCatalogItem = item;
                base.SiteContext.CurrentItem = currentItem;
                CategorySearchInformation categorySearchInformation = base.SearchInformation.GetCategorySearchInformation(item);
                this.GetSortParameters(categorySearchInformation, ref sortField, ref sortDirection);
                CommerceSearchOptions commerceSearchOptions = new CommerceSearchOptions(this.GetDefaultItemsPerPage(pageSize, categorySearchInformation), pageNumber.GetValueOrDefault(0));
                if (!string.IsNullOrWhiteSpace(searchKeyword))
                {
                    commerceSearchOptions.SearchKeyword = searchKeyword;
                    commerceSearchOptions.CatalogName = base.StorefrontContext.CurrentStorefront.Catalog;
                }
                this.UpdateOptionsWithFacets(categorySearchInformation.RequiredFacets, facetValues, commerceSearchOptions);
                this.UpdateOptionsWithSorting(sortField, sortDirection, commerceSearchOptions);
                SearchResults childProducts = base.GetChildProducts(commerceSearchOptions, item);
                List<ProductEntity> productEntityList = this.AdjustProductPriceAndStockStatus(visitorContext, childProducts, item).ToList();
                productListJsonResult.Initialize(this, productEntityList, false, searchKeyword);
            }
            else if (Sitecore.Context.PageMode.IsExperienceEditor)
            {
                productListJsonResult = InitializeMockData(this, productListJsonResult);
            }
            return productListJsonResult;
        }               
                                                                                  
        public static PurchasableProductListJsonResult InitializeMockData(BaseCommerceModelRepository repository, PurchasableProductListJsonResult model)
        {
            List<ProductEntity> productEntities = GetProductEntities();
            model.Initialize(repository, productEntities, true, "");
            foreach (PurchasableProductSummaryViewModel product in model.ChildProducts)
            {
                MediaItem item = Sitecore.Context.Database.GetItem(CatalogFeatureConstants.MockDataItems.MockProductId);
                product.Images.Add(item);
                product.DisplayName = "Lorem ipsum";
                product.Description = "Lorem ipsum";
                product.DisplayStartingFrom = true;
                product.Link = "/";
            }
            return model;
        }

        private static List<ProductEntity> GetProductEntities()
        {
            List<ProductEntity> list = new List<ProductEntity>();
            for (int i = 0; i < 12; i++)
            {
                ProductEntity productEntity = new ProductEntity();
                productEntity.ListPrice = new decimal?(14.95m);
                productEntity.ProductId = "12345";
                productEntity.CustomerAverageRating = i % 5;
                if (i % 4 == 0)
                {
                    productEntity.AdjustedPrice = new decimal?(12.95m);
                    productEntity.LowestPricedVariantListPrice = new decimal?(13.85m);
                    productEntity.LowestPricedVariantAdjustedPrice = new decimal?(12.95m);
                }
                if (i % 5 == 0)
                {
                    productEntity.AdjustedPrice = new decimal?(12.95m);
                    productEntity.HighestPricedVariantAdjustedPrice = new decimal?(13.95m);
                    productEntity.LowestPricedVariantListPrice = new decimal?(14.95m);
                    productEntity.LowestPricedVariantAdjustedPrice = new decimal?(12.95m);
                }
                if (i % 7 == 0)
                {
                    productEntity.StockStatus = StockStatus.OutOfStock;
                    productEntity.StockStatusName = "Out-of-Stock";
                }
                else
                {
                    productEntity.StockStatus = StockStatus.InStock;
                    productEntity.StockStatusName = "In-Stock";
                }
                list.Add(productEntity);
            }
            return list;
        }

    }
}
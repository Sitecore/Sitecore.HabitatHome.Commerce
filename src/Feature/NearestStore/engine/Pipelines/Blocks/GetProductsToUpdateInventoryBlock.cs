using System.Collections.Generic;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.Framework.Caching;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("StoreInventory.block.GetStoreDetails")]
    public class GetProductsToUpdateInventoryBlock : PipelineBlock<string, List<string>, CommercePipelineExecutionContext>
    {
        private readonly IFindEntitiesInListPipeline _findEntitiesInListPipeline;
        private readonly IGetEnvironmentCachePipeline _cachePipeline;
        private readonly IFindEntityPipeline _findEntityPipeline;


        public GetProductsToUpdateInventoryBlock(IFindEntityPipeline findEntityPipeline, IGetEnvironmentCachePipeline cachePipeline, IFindEntitiesInListPipeline findEntitiesPipeline)
      : base((string)null)
        {
           
            this._cachePipeline = cachePipeline;
            this._findEntitiesInListPipeline = findEntitiesPipeline;
            this._findEntityPipeline = findEntityPipeline;
        }

        public override async Task<List<string>> Run(string catalogName, CommercePipelineExecutionContext context)
        {
            List<InventorySet> inventorySets = new List<InventorySet>();

            GetProductsToUpdateInventoryBlock getProductsToUpdateInventoryBlock = this;

            // get the sitecoreid from catalog based on catalogname
            var catalogSitecoreId = "";
            FindEntitiesInListArgument entitiesInListArgumentCatalog = await getProductsToUpdateInventoryBlock._findEntitiesInListPipeline.Run(new FindEntitiesInListArgument(typeof(Catalog), string.Format("{0}", (object)CommerceEntity.ListName<Catalog>()), 0, int.MaxValue), context);
            foreach (CommerceEntity commerceEntity in (await getProductsToUpdateInventoryBlock._findEntitiesInListPipeline.Run(entitiesInListArgumentCatalog, (IPipelineExecutionContextOptions)context.ContextOptions)).List.Items)
            {

                var item = commerceEntity as Catalog;
                
                if(item.Name == catalogName)
                {
                    catalogSitecoreId = item.SitecoreId;
                    break;
                }
            }


            if(string.IsNullOrEmpty(catalogSitecoreId))
            {
                return null;
            }

            List<string> productIds = new List<string>();        


            string cacheKey = string.Format("{0}|{1}|{2}", context.CommerceContext.Environment.Name, context.CommerceContext.CurrentLanguage(), context.CommerceContext.CurrentShopName() ?? "");
            CatalogCachePolicy cachePolicy = context.GetPolicy<CatalogCachePolicy>();
            ICache cache = null;
            List<SellableItem> sellableItems = null;
            if (cachePolicy.AllowCaching)
            {
                IGetEnvironmentCachePipeline cachePipeline = getProductsToUpdateInventoryBlock._cachePipeline;
                EnvironmentCacheArgument environmentCacheArgument = new EnvironmentCacheArgument();
                environmentCacheArgument.CacheName = cachePolicy.CatalogsCacheName;
                CommercePipelineExecutionContext context1 = context;
                cache = await cachePipeline.Run(environmentCacheArgument, context1).ConfigureAwait(false);
                sellableItems = await cache.Get(cacheKey).ConfigureAwait(false) as List<SellableItem>;
                if (sellableItems != null)
                {
                    foreach(var item in sellableItems)
                    {
                        await GetProductId(context, getProductsToUpdateInventoryBlock, catalogSitecoreId, productIds, item);                       
                    }
                }                    
            }
            else
            {
                sellableItems = new List<SellableItem>();
                FindEntitiesInListArgument entitiesInListArgument = new FindEntitiesInListArgument(typeof(SellableItem), string.Format("{0}", (object)CommerceEntity.ListName<SellableItem>()), 0, int.MaxValue);
                foreach (CommerceEntity commerceEntity in (await getProductsToUpdateInventoryBlock._findEntitiesInListPipeline.Run(entitiesInListArgument, context.ContextOptions).ConfigureAwait(false)).List.Items)
                {
                    await GetProductId(context, getProductsToUpdateInventoryBlock, catalogSitecoreId, productIds, commerceEntity as SellableItem).ConfigureAwait(false);
                }

                if (cachePolicy.AllowCaching)
                {
                    if (cache != null)
                        await cache.Set(cacheKey, new Cachable<List<SellableItem>>(sellableItems, 1L), cachePolicy.GetCacheEntryOptions()).ConfigureAwait(false);
                }
            }

            return productIds;
        }

        private static async Task GetProductId(CommercePipelineExecutionContext context, GetProductsToUpdateInventoryBlock getProductsToUpdateInventoryBlock, string catalogSitecoreId, List<string> productIds,  SellableItem item)
        {            

            if (item.ParentCatalogList == catalogSitecoreId)
            {
                CommerceEntity entity = await getProductsToUpdateInventoryBlock._findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), item.Id, false), context);

                if ((entity is SellableItem))
                {
                    SellableItem sellableItem = entity as SellableItem;
                    var variants = sellableItem.GetComponent<ItemVariationsComponent>();

                    if (variants != null && variants.ChildComponents.Count > 0)
                    {
                        foreach (ItemVariationComponent variant in variants.ChildComponents)
                        {
                            var variantData = variant;
                            productIds.Add($"{item.Id}|{variantData.Id}");
                        }                        
                    }
                    else
                    {
                        productIds.Add($"{item.Id}");
                    }
                }
            }
        }
    }
}

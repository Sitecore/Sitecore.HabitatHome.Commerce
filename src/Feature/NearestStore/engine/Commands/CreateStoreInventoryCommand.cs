using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Commands
{
    public class CreateStoreInventoryCommand : CommerceCommand
    {
        private readonly ICreateStoreInventorySetPipeline _createStoreInventorySetPipeline;
        private readonly IAssociateSellableItemToInventorySetPipeline _associateSellableItemToInventorySetPipeline;
        private readonly IEditInventoryInformationPipeline _editInventoryInformationPipeline;
        private readonly IAssociateStoreInventoryToSellableItem _associateStoreInventoryToSellableItem;
        private readonly IGetProductsToUpdateInventoryPipeline _getProductsToUpdateInventoryPipeline

;
        public CreateStoreInventoryCommand(IGetProductsToUpdateInventoryPipeline getProductsToUpdateInventoryPipeline, ICreateStoreInventorySetPipeline createInventorySetPipeline, IAssociateStoreInventoryToSellableItem associateStoreInventoryToSellableItem, IAssociateSellableItemToInventorySetPipeline associateSellableItemToInventorySetPipeline, IEditInventoryInformationPipeline editInventoryInformationPipeline, IServiceProvider serviceProvider)
        : base(serviceProvider)
        {
            this._createStoreInventorySetPipeline = createInventorySetPipeline;
            this._editInventoryInformationPipeline = editInventoryInformationPipeline;
            this._associateSellableItemToInventorySetPipeline = associateSellableItemToInventorySetPipeline;
            this._associateStoreInventoryToSellableItem = associateStoreInventoryToSellableItem;
            this._getProductsToUpdateInventoryPipeline = getProductsToUpdateInventoryPipeline;
        }

        public async Task<List<InventorySet>> Process(CommerceContext commerceContext, List<CreateStoreInventorySetArgument> inputArgumentList, List<string> productsToAssociate, string catalogName)
        {
            CreateStoreInventoryCommand createStoreInventoryCommand = this;
       
            List<InventorySet> sets = new List<InventorySet>();
            using (CommandActivity.Start(commerceContext, createStoreInventoryCommand))
            {
                await createStoreInventoryCommand.PerformTransaction(commerceContext, async () =>
                {

                    CommercePipelineExecutionContextOptions pipelineContextOptions = commerceContext.PipelineContextOptions;
                    
                    foreach(CreateStoreInventorySetArgument arg in inputArgumentList)
                    {
                        InventorySet inventorySet2 = await this._createStoreInventorySetPipeline.Run(arg, pipelineContextOptions).ConfigureAwait(false);

                        if (inventorySet2 != null)
                        {
                            sets.Add(inventorySet2);
                        }
                        else
                        {
                            // find the entity set id and add - for further processng in associate
                            sets.Add(new InventorySet() { Id = CommerceEntity.IdPrefix<InventorySet>() + arg.Name });                            
                        }
                    }                                      
                });
            }

            // Update all products if no input passed
            if (productsToAssociate.Count == 0)
            {

                var products = await this._getProductsToUpdateInventoryPipeline.Run(catalogName, commerceContext.PipelineContextOptions).ConfigureAwait(false);
                productsToAssociate = products;

                if(productsToAssociate == null)
                {
                    return null;
                }
            }

            // Once Done.. then assign inventory to products in the sets

            // Associate Sellable Item to Inventory Set           

            foreach (var product in productsToAssociate)
            {
                using (CommandActivity.Start(commerceContext, createStoreInventoryCommand))
                {
                    var productIds = product.Split('|');
                    string variantId = null;
                    var productId = product.Split('|').FirstOrDefault();

                    if (productIds.Count() > 1)
                    {
                        variantId = product.Split('|').Skip(1).FirstOrDefault();
                    }

                    SellableItemInventorySetsArgument args = new SellableItemInventorySetsArgument()
                    {
                        InventorySetIds = sets.Select(x => x.Id).ToList(),
                        SellableItemId = productId,
                        VariationId = variantId
                    };

                    bool result = await this._associateStoreInventoryToSellableItem.Run(args, commerceContext.PipelineContextOptions).ConfigureAwait(false);
                }
            }

            return sets;
        }
    }
}

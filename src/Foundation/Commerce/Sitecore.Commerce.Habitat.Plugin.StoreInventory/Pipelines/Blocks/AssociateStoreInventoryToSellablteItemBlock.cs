using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.Commerce.Plugin.Inventory;
using Plugin.Demo.HabitatHome.StoreInventorySet.Components;
using Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines.Arguments;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Commerce.Plugin.ManagedLists;

namespace Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines.Blocks
{
    [PipelineDisplayName("StoreInventory.block.AssociateStoreInventoryToSellablteItemBlock")]
    public class AssociateStoreInventoryToSellablteItemBlock : PipelineBlock<SellableItemInventorySetsArgument, bool, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findEntityPipeline;
        private readonly IFindEntitiesInListPipeline _findEntitiesInListPipeline;
        private readonly ICreateRelationshipPipeline _createRelationshipPipeline;
        private readonly IPersistEntityPipeline _persistEntityPipeline;
        private readonly GetInventoryInformationCommand _getInventoryInformationCommand;

        public AssociateStoreInventoryToSellablteItemBlock(IFindEntityPipeline findEntityPipeline, IFindEntitiesInListPipeline findEntitiesInListPipeline, ICreateRelationshipPipeline createRelationshipPipeline, IPersistEntityPipeline persistEntityPipeline, GetInventoryInformationCommand getInventoryInformationCommand)
      : base((string)null)
        {
            this._findEntityPipeline = findEntityPipeline;
            this._findEntitiesInListPipeline = findEntitiesInListPipeline;
            this._createRelationshipPipeline = createRelationshipPipeline;
            this._persistEntityPipeline = persistEntityPipeline;
            this._getInventoryInformationCommand = getInventoryInformationCommand;
        }


        public override async Task<bool> Run(SellableItemInventorySetsArgument argument, CommercePipelineExecutionContext context)
        {
            AssociateStoreInventoryToSellablteItemBlock associateStoreInventoryToSellablteItemBlock = this;
            Condition.Requires<SellableItemInventorySetsArgument>(argument).IsNotNull<SellableItemInventorySetsArgument>(string.Format("{0}: The argument can not be null", argument));

            string sellableItemId = argument.SellableItemId;
            CommerceEntity entity = await associateStoreInventoryToSellablteItemBlock._findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), sellableItemId, false), context);
            CommercePipelineExecutionContext executionContext;

            if (!(entity is SellableItem))
            {
                executionContext = context;
                CommerceContext commerceContext = context.CommerceContext;
                string validationError = context.GetPolicy<KnownResultCodes>().ValidationError;
                string commerceTermKey = "EntityNotFound";
                object[] args = new object[1]
                {
                    (object) argument.SellableItemId
                };
                string defaultMessage = string.Format("Entity {0} was not found.", (object)argument.SellableItemId);
                executionContext.Abort(await commerceContext.AddMessage(validationError, commerceTermKey, args, defaultMessage), (object)context);
                executionContext = (CommercePipelineExecutionContext)null;
                return false;
            }



            SellableItem sellableItem = entity as SellableItem;
            ItemVariationComponent sellableItemVariation = sellableItem.GetVariation(argument.VariationId);
            if (!string.IsNullOrEmpty(argument.VariationId) && sellableItemVariation == null)
            {
                executionContext = context;
                CommerceContext commerceContext = context.CommerceContext;
                string validationError = context.GetPolicy<KnownResultCodes>().ValidationError;
                string commerceTermKey = "ItemNotFound";
                object[] args = new object[1]
                {
                    (object) argument.VariationId
                };
                string defaultMessage = string.Format("Item '{0}' was not found.", (object)argument.VariationId);
                executionContext.Abort(await commerceContext.AddMessage(validationError, commerceTermKey, args, defaultMessage), (object)context);
                executionContext = (CommercePipelineExecutionContext)null;
                return false;
            }


            if (string.IsNullOrEmpty(argument.VariationId) & sellableItem.HasComponent<ItemVariationsComponent>())
            {
                executionContext = context;
                executionContext.Abort(await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().ValidationError, "AssociateInventoryWithVariant", new object[0], "Can not associate inventory to the base sellable item. Use one of the variants instead."), (object)context);
                executionContext = (CommercePipelineExecutionContext)null;
                return false;
            }


            List<InventoryAssociation> inventoryAssociations = new List<InventoryAssociation>();
            

            foreach(var inventorySetId in argument.InventorySetIds)
            {
                bool isUpdate = false;
                Random rnd = new Random();
                InventoryInformation inventoryInformation = await associateStoreInventoryToSellablteItemBlock._getInventoryInformationCommand.Process(context.CommerceContext, inventorySetId, argument.SellableItemId, argument.VariationId, false);
                IFindEntitiesInListPipeline entitiesInListPipeline = associateStoreInventoryToSellablteItemBlock._findEntitiesInListPipeline;
                FindEntitiesInListArgument entitiesInListArgument1 = new FindEntitiesInListArgument(typeof(SellableItem), string.Format("{0}-{1}", (object)"InventorySetToInventoryInformation", inventorySetId.SimplifyEntityName()), 0, int.MaxValue);
                entitiesInListArgument1.LoadEntities = false;
                CommercePipelineExecutionContext context1 = context;
                FindEntitiesInListArgument entitiesInListArgument2 = await entitiesInListPipeline.Run(entitiesInListArgument1, context1);
                if (inventoryInformation != null && entitiesInListArgument2 != null)
                {
                    List<string> idList = entitiesInListArgument2.IdList;
                    bool? nullable1;
                    if (idList == null)
                    {
                        nullable1 = new bool?();
                    }
                    else
                    {
                        string id = inventoryInformation.Id;
                        nullable1 = new bool?((idList.Contains(id)));
                    }
                    bool? nullable2 = nullable1;
                    bool flag = true;
                    if ((nullable2.GetValueOrDefault() == flag ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
                    {
                        inventoryInformation.Quantity = rnd.Next(50);
                        isUpdate = true;
                        //executionContext = context;
                        //CommerceContext commerceContext = context.CommerceContext;
                        //string error = context.GetPolicy<KnownResultCodes>().Error;
                        //string commerceTermKey = "SellableItemAlreadyAssociated";
                        //object[] args = new object[1]
                        //{
                        //(object) argument.SellableItemId
                        //};
                        //string defaultMessage = string.Format("The sellable item '{0}' is already associated.", (object)argument.SellableItemId);
                        //executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, args, defaultMessage), (object)context);
                        //executionContext = (CommercePipelineExecutionContext)null;
                        //return false;
                    }
                }


                if(!isUpdate)
                {                
                    string str1 = string.Format("{0}-{1}", inventorySetId.SimplifyEntityName(), (object)sellableItem.ProductId);
                    if (!string.IsNullOrEmpty(argument.VariationId))
                        str1 += string.Format("-{0}", (object)argument.VariationId);
                    InventoryInformation inventoryInformation1 = new InventoryInformation();
                    //string str2 = string.Format("{0}{1}", (object)CommerceEntity.IdPrefix<InventoryInformation>(), (object)str1);
                    inventoryInformation1.Id = string.Format("{0}{1}", (object)CommerceEntity.IdPrefix<InventoryInformation>(), (object)str1);
                    string str3 = str1;
                    inventoryInformation1.FriendlyId = str3;
                    EntityReference entityReference1 = new EntityReference(inventorySetId, "");
                    inventoryInformation1.InventorySet = entityReference1;
                    EntityReference entityReference2 = new EntityReference(argument.SellableItemId, "");
                    inventoryInformation1.SellableItem = entityReference2;
                    string variationId = argument.VariationId;
                    inventoryInformation1.VariationId = variationId;
                    inventoryInformation1.Quantity = rnd.Next(50);
                    inventoryInformation = inventoryInformation1;
                }                

                inventoryInformation.GetComponent<TransientListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<InventoryInformation>());
                PersistEntityArgument persistEntityArgument1 = await associateStoreInventoryToSellablteItemBlock._persistEntityPipeline.Run(new PersistEntityArgument((CommerceEntity)inventoryInformation), context);
                RelationshipArgument relationshipArgument = await associateStoreInventoryToSellablteItemBlock._createRelationshipPipeline.Run(new RelationshipArgument(inventorySetId, inventoryInformation.Id, "InventorySetToInventoryInformation"), context);

                InventoryAssociation inventoryAssociation = new InventoryAssociation()
                {
                    InventoryInformation = new EntityReference(inventoryInformation.Id, ""),
                    InventorySet = new EntityReference(inventorySetId, "")
                };

                inventoryAssociations.Add(inventoryAssociation);
            }



                //(sellableItemVariation != null ? sellableItemVariation.GetComponent<InventoryComponent>() : sellableItem.GetComponent<InventoryComponent>()).InventoryAssociations.Add(new InventoryAssociation()
                //{
                //    InventoryInformation = new EntityReference(inventoryInformation.Id, ""),
                //    InventorySet = new EntityReference(argument.InventorySetId, "")
                //});

            (sellableItemVariation != null ? sellableItemVariation.GetComponent<InventoryComponent>() : sellableItem.GetComponent<InventoryComponent>()).InventoryAssociations.AddRange(inventoryAssociations);


            PersistEntityArgument persistEntityArgument2 = await associateStoreInventoryToSellablteItemBlock._persistEntityPipeline.Run(new PersistEntityArgument((CommerceEntity)sellableItem), context);
            return true;

        }
    }
}

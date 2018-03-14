using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System.Runtime.CompilerServices;
using Sitecore.Commerce.Plugin.Inventory;
using Plugin.Demo.HabitatHome.StoreInventorySet.Components;
using Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines.Arguments;

namespace Plugin.Demo.HabitatHome.StoreInventorySet.Pipelines.Blocks
{
    [PipelineDisplayName("StoreInventory.block.CreateStoreInventorySet")]
    public class CreateStoreInventorySetBlock : PipelineBlock<CreateStoreInventorySetArgument, InventorySet, CommercePipelineExecutionContext>
    {
        private readonly IDoesEntityExistPipeline _doesEntityExistPipeline;
        private readonly IPersistEntityPipeline _persistEntityPipeline;
        private readonly IAddListEntitiesPipeline _addListEntitiesPipeline;

        public CreateStoreInventorySetBlock(IDoesEntityExistPipeline doesEntityExistPipeline, IPersistEntityPipeline persistEntityPipeline, IAddListEntitiesPipeline addListEntitiesPipeline)
            : base((string)null)
        {
            this._doesEntityExistPipeline = doesEntityExistPipeline;
            this._persistEntityPipeline = persistEntityPipeline;
            this._addListEntitiesPipeline = addListEntitiesPipeline;
        }

        public override async Task<InventorySet> Run(CreateStoreInventorySetArgument arg, CommercePipelineExecutionContext context)
        {
            CreateStoreInventorySetBlock inventorySetBlock = this;

            string id = CommerceEntity.IdPrefix<InventorySet>() + arg.Name;
            var doesExist = await inventorySetBlock._doesEntityExistPipeline.Run(new FindEntityArgument(typeof(InventorySet), id, false), context);
            if(doesExist)
            {
                CommercePipelineExecutionContext executionContext = context;
                CommerceContext commerceContext = context.CommerceContext;
                string validationError = context.GetPolicy<KnownResultCodes>().ValidationError;
                //string commerceTermKey = "InventorySetNameAlreadyInUse";
                object[] args = new object[1] { (object)arg.Name };
                string defaultMessage = string.Format("Inventory set name {0} is already in use.", (object)arg.Name);
                //executionContext.Abort(await commerceContext.AddMessage(validationError, commerceTermKey, args, defaultMessage), (object)context);
                //executionContext = (CommercePipelineExecutionContext)null;
                return new InventorySet() { Id = CommerceEntity.IdPrefix<InventorySet>() + arg.Name };
            }


            InventorySet inventorySet = new InventorySet();
            string str = id;
            inventorySet.Id = str;
            string name1 = arg.Name;
            inventorySet.FriendlyId = name1;
            string name2 = arg.Name;
            inventorySet.Name = name2;
            string displayName = arg.DisplayName;
            inventorySet.DisplayName = displayName;
            string description = arg.Description;
            inventorySet.Description = description;

            // Set component for store
            StoreDetailsComponent storeDetailsComponent = new StoreDetailsComponent()
            {
                StoreName = arg.StoreName,
                Address = arg.Address,
                City = arg.City,
                State = arg.State,
                StateCode = arg.Abbreviation,
                ZipCode = arg.ZipCode,
                CountryCode = arg.CountryCode,
                Long = arg.Long,
                Lat = arg.Lat
            };

            inventorySet.SetComponent(storeDetailsComponent);            

            PersistEntityArgument persistEntityArgument = await inventorySetBlock._persistEntityPipeline.Run(new PersistEntityArgument(inventorySet), context);
            context.CommerceContext.AddEntity(inventorySet);
            ListEntitiesArgument entitiesArgument1 = new ListEntitiesArgument((IEnumerable<string>)new string[1]
            {
                inventorySet.Id
            }, CommerceEntity.ListName<InventorySet>());
            ListEntitiesArgument entitiesArgument2 = await inventorySetBlock._addListEntitiesPipeline.Run(entitiesArgument1, context);
            return inventorySet;            
        }
    }
}

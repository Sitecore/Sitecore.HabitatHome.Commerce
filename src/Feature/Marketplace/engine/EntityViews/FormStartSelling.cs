using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.EBay.Engine.Commands;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.EntityViews
{
    /// <summary>
    /// Defines a block which populates an EntityView for a list of Coupons with the View named Public Coupons.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("FormStartSelling")]
    public class FormStartSelling : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormStartSelling"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public FormStartSelling(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>Runs the Command.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "Ebay-FormStartSelling")
            {
                return entityView;
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            //var pluginPolicy = context.GetPolicy<PluginPolicy>();

            var inventorySets = await this._commerceCommander.Command<GetInventorySetsCommand>().Process(context.CommerceContext);


            var inventoryAvailable = 0;
            var sellableItem = entityViewArgument.Entity as SellableItem;

            if (sellableItem == null)
            {
                sellableItem = await this._commerceCommander.GetEntity<SellableItem>(context.CommerceContext, entityView.ItemId);
            }

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "",
                    IsHidden = false,
                    IsReadOnly = true,
                    OriginalType = "Html",
                    UiType = "Html",
                    RawValue = "<img alt='This is the alternate' height=50 width=100 src='https://www.paypalobjects.com/webstatic/en_AR/mktg/merchant/pages/sell-on-ebay/image-ebay.png' style=''/>"
                });

            if (sellableItem.HasComponent<ItemVariationsComponent>())
            {
                //has variations
                foreach(var itemVariationComponent in sellableItem.GetComponent<ItemVariationsComponent>().GetComponents<ItemVariationComponent>())
                {
                    if (itemVariationComponent.HasComponent<InventoryComponent>())
                    {
                        var inventoryComponent = itemVariationComponent.GetComponent<InventoryComponent>();

                        var firstInventorySet = inventoryComponent.InventoryAssociations.FirstOrDefault();

                        if(firstInventorySet != null)
                        {
                            var inventoryItem = await this._commerceCommander.GetEntity<InventoryInformation>(context.CommerceContext, firstInventorySet.InventoryInformation.EntityTarget);

                            if (inventoryAvailable < inventoryItem.Quantity)
                            {
                                inventoryAvailable = inventoryItem.Quantity;
                            }

                            var inventorySlotKey = $"SlotKey-{firstInventorySet.InventorySet.EntityTarget.Replace("Entity-InventorySet-", "")}-{firstInventorySet.InventoryInformation.EntityTarget.Replace("Entity-InventoryInformation-", "")}";

                            entityView.Properties.Add(
                                new ViewProperty
                                {
                                    Name = $"IncludeVariant-{itemVariationComponent.Id}",
                                    DisplayName = $"Include Variant-{itemVariationComponent.Name} ({inventoryItem.Quantity})",
                                    IsHidden = false,
                                    //IsReadOnly = true,
                                    IsRequired = true,
                                    RawValue = true
                                });
                        }

                        // Commenting out because for ebay, we want only 1 inventory set.
                        //foreach (var inventoryAssociationTarget in inventoryComponent.InventoryAssociations)
                        //{
                        //    var inventoryItem = await this._commerceCommander.GetEntity<InventoryInformation>(context.CommerceContext, inventoryAssociationTarget.InventoryInformation.EntityTarget);

                        //    if (inventoryAvailable < inventoryItem.Quantity)
                        //    {
                        //        inventoryAvailable = inventoryItem.Quantity;
                        //    }

                        //    var inventorySlotKey = $"SlotKey-{inventoryAssociationTarget.InventorySet.EntityTarget.Replace("Entity-InventorySet-","")}-{inventoryAssociationTarget.InventoryInformation.EntityTarget.Replace("Entity-InventoryInformation-","")}";

                        //    entityView.Properties.Add(
                        //        new ViewProperty
                        //        {
                        //            Name = $"IncludeVariant-{itemVariationComponent.Id}",
                        //            DisplayName = $"Include Variant-{itemVariationComponent.Name} ({inventoryItem.Quantity})",
                        //            IsHidden = false,
                        //            //IsReadOnly = true,
                        //            IsRequired = true,
                        //            RawValue = true
                        //        });                           
                        //}
                    }
                }
            }

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Quantity",
                    DisplayName = "Quantity to Sell",
                    IsHidden = false,
                    //IsReadOnly = true,
                    IsRequired = true,
                    RawValue = inventoryAvailable
                });


            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "ListingDuration",
                    DisplayName = "Listing Duration (Days)",
                    IsHidden = false,
                    //IsReadOnly = true,
                    IsRequired = true,
                    RawValue = 10
                });

            

            //entityView.Properties.Add(
            //    new ViewProperty
            //    {
            //        Name = "AllocateInventory",
            //        DisplayName = "Allocate Inventory",
            //        IsHidden = false,
            //        //IsReadOnly = true,
            //        IsRequired = true,
            //        RawValue = true
            //    });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Subtitle",
                    IsHidden = false,
                    //IsReadOnly = true,
                    IsRequired = true,
                    RawValue = "Item Default Subtitle"
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "ImmediateListing",
                    DisplayName = "Publish Immediately to Ebay",
                    IsHidden = false,
                    //IsReadOnly = true,
                    IsRequired = true,
                    RawValue = true
                });

            //sellableItem.Tags.First();
            var categories = await this._commerceCommander.Command<EbayCommand>().GetSuggestedCategories(context.CommerceContext, sellableItem.Tags.First().Name);


            return entityView;
        }


    }

}

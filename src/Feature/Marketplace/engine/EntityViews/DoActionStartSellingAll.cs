using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.EBay.Engine.Commands;
using Sitecore.HabitatHome.Feature.EBay.Engine.Components;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.EntityViews
{
    /// <summary>
    /// Defines the do action edit line item block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionStartSellingAll")]
    public class DoActionStartSellingAll : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionStartSellingAll"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionStartSellingAll(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="entityView">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="EntityView"/>.
        /// </returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {

            if (entityView == null
                || !entityView.Action.Equals("Ebay-StartSellingAll", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

                var immediateListing = entityView.Properties.First(p => p.Name == "ImmediateListing").Value ?? "";
                var isImmediateListing = System.Convert.ToBoolean(immediateListing);

                var foundEntity = context.CommerceContext.GetObjects<CommerceEntity>().FirstOrDefault(p => p.Id == entityView.EntityId);
                if (foundEntity != null)
                {
                    //this._commerceCommander.Command<Sell>
                    var category = foundEntity as Category;

                    var listName = $"{CatalogConstants.CategoryToSellableItem}-{category.Id.SimplifyEntityName()}";

                    var sellableItems = await this._commerceCommander.Command<ListCommander>()
                            .GetListItems<SellableItem>(context.CommerceContext, listName, 0,10).ConfigureAwait(false);

                    foreach(var sellableItem in sellableItems)
                    {
                        
                        if (isImmediateListing)
                        {
                            if (sellableItem.HasComponent<EbayItemComponent>())
                            {
                                //This item may already be listed
                                var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();
                                if (ebayItemComponent.Status == "Ended")
                                {
                                    try
                                    {
                                        var result = await this._commerceCommander.Command<EbayCommand>().RelistItem(context.CommerceContext, sellableItem).ConfigureAwait(false);
                                    }
                                    catch (Exception ex)
                                    {
                                        context.Logger.LogError($"Ebay.DoActionStartSelling.Exception: Message={ex.Message}");
                                        await context.CommerceContext.AddMessage("Error", "DoActionStartSelling.Run.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
                                    }
                                }
                                else
                                {
                                    var ebayItem = await this._commerceCommander.Command<EbayCommand>().AddItem(context.CommerceContext, sellableItem).ConfigureAwait(false);
                                }
                            }
                            else
                            {
                                var ebayItem = await this._commerceCommander.Command<EbayCommand>().AddItem(context.CommerceContext, sellableItem).ConfigureAwait(false);
                            }
                            
                        }
                        else
                        {
                            var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();
                            ebayItemComponent.Status = "Pending";
                            sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Pending");
                        }
                        var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, sellableItem).ConfigureAwait(false);
                    }
                    //var sellableItemIds = category.ChildrenSellableItemList.Split("|".ToCharArray());
                    //foreach(var sellableItemId1 in sellableItemIds)
                    //{
                    //    var sellableItem2 = await this._commerceCommander.GetEntity<CatalogItemBase>(context.CommerceContext, sellableItemId1, false);

                    //}
                }
                //var category = this._commerceCommander.GetEntity<Category>(context.CommerceContext, e)
                //var listingDuration = entityView.Properties.First(p => p.Name == "ListingDuration").Value ?? "";

                //var sellableItemId = entityView.EntityId;
                //if (entityView.EntityId.Contains("Entity-Category-"))
                //{
                //    sellableItemId = entityView.ItemId;
                //}

                //var sellableItem = await this._commerceCommander.GetEntity<SellableItem>(context.CommerceContext, sellableItemId);

                //var ebayItem = await this._commerceCommander.Command<EbayCommand>().PrepareItem(context.CommerceContext, sellableItem);

                //ebayItem.ListingDuration = "Days_" + listingDuration;

                //ebayItem = await this._commerceCommander.Command<EbayCommand>().AddItem(context.CommerceContext, sellableItem);

                //var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();
                //ebayItemComponent.Name = "EbayItemComponent";
                //ebayItemComponent.EbayId = ebayItem.ItemID;
                //ebayItemComponent.Status = "Listed";

                //sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Listed");

                //var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, sellableItem);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionStartSellingAll.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "DoActionStartSelling.Run.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
            }

            return entityView;
        }
    }
}

using System;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.EBay.Engine.Commands;

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
    [PipelineDisplayName("DoActionPublishPending")]
    public class DoActionPublishPending : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionPublishPending"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionPublishPending(CommerceCommander commerceCommander)
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
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {

            if (entityView == null
                || !entityView.Action.Equals("Ebay-PublishPending", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(entityView);
            }

            //var result = await this._commerceCommander.Command<EbayCommand>().PublishPending(context.CommerceContext);
            //Task task = Task.Run((Action)MyFunction);
            Task.Factory.StartNew(() => this._commerceCommander.Command<EbayCommand>().PublishPending(context.CommerceContext));

            //try
            //{
            //    var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            //    var ebayPendingSellableItems = await this._commerceCommander.Command<ListCommander>()
            //                .GetListItems<SellableItem>(context.CommerceContext, "Ebay_Pending", 0, 10);

            //    foreach (var sellableItem in ebayPendingSellableItems)
            //    {
            //        var ebayItem = await this._commerceCommander.Command<EbayCommand>().PrepareItem(context.CommerceContext, sellableItem);

            //        ebayItem.ListingDuration = "Days_10";

            //        var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();
            //        //ebayItemComponent.Name = "EbayItemComponent";

            //        ebayItem = await this._commerceCommander.Command<EbayCommand>().AddItem(context.CommerceContext, ebayItem);
            //        ebayItemComponent.EbayId = ebayItem.ItemID;
            //        ebayItemComponent.Status = "Listed";
            //        sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Listed");

            //        var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, sellableItem);

            //        var listRemoveResult = await this._commerceCommander.Command<ListCommander>()
            //            .RemoveItemsFromList(context.CommerceContext, "Ebay_Pending", new List<String>() { sellableItem.Id });

            //    }

            //    //    var foundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p => p.EntityId == entityView.EntityId);
            //    //if (foundEntity != null)
            //    //{
            //    //    //this._commerceCommander.Command<Sell>
            //    //    var category = foundEntity.Entity as Category;

            //    //    var sellableItemIds = category.ChildrenSellableItemList.Split("|".ToCharArray());
            //    //    foreach(var sellableItemId1 in sellableItemIds)
            //    //    {
            //    //        var sellableItem2 = await this._commerceCommander.GetEntity<CatalogItemBase>(context.CommerceContext, sellableItemId1, false);

            //    //    }
            //    //}
            //    ////var category = this._commerceCommander.GetEntity<Category>(context.CommerceContext, e)
            //    //var listingDuration = entityView.Properties.First(p => p.Name == "ListingDuration").Value ?? "";

            //    //var sellableItemId = entityView.EntityId;
            //    //if (entityView.EntityId.Contains("Entity-Category-"))
            //    //{
            //    //    sellableItemId = entityView.ItemId;
            //    //}

            //    //var sellableItem = await this._commerceCommander.GetEntity<SellableItem>(context.CommerceContext, sellableItemId);

            //    //var ebayItem = await this._commerceCommander.Command<EbayCommand>().PrepareItem(context.CommerceContext, sellableItem);

            //    //ebayItem.ListingDuration = "Days_" + listingDuration;

            //    //ebayItem = await this._commerceCommander.Command<EbayCommand>().AddItem(context.CommerceContext, ebayItem);

            //    //var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();
            //    //ebayItemComponent.Name = "EbayItemComponent";
            //    //ebayItemComponent.EbayId = ebayItem.ItemID;
            //    //ebayItemComponent.Status = "Listed";

            //    //sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Listed");

            //    //var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, sellableItem);
            //}
            //catch (Exception ex)
            //{
            //    context.Logger.LogError($"Catalog.DoActionAddDashboardEntity.Exception: Message={ex.Message}");
            //}

            return Task.FromResult(entityView);
        }
    }
}

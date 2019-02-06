using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
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
    [PipelineDisplayName("DoActionEndListingAll")]
    public class DoActionEndListingAll : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionEndListingAll"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionEndListingAll(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("Ebay-EndListingAllItems", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);
                //var reason = entityView.Properties.First(p => p.Name == "Reason").Value ?? "";
                var foundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p => p.EntityId == entityView.EntityId);

                IEnumerable<SellableItem> ebayListedSellableItems = new List<SellableItem>();
                if (entityView.EntityId.Contains("Entity-Category-"))
                {
                    var category = foundEntity.Entity as Category;

                    var listName = $"{CatalogConstants.CategoryToSellableItem}-{category.Id.SimplifyEntityName()}";

                    ebayListedSellableItems = await this._commerceCommander.Command<ListCommander>()
                    .GetListItems<SellableItem>(context.CommerceContext, listName, 0, 100).ConfigureAwait(false);
                }
                else
                {
                    ebayListedSellableItems = await this._commerceCommander.Command<ListCommander>()
                .GetListItems<SellableItem>(context.CommerceContext, "Ebay_Listed", 0, 100).ConfigureAwait(false);
                }

                    

                foreach(var listedItem in ebayListedSellableItems)
                {
                    var endListingResult = await this._commerceCommander.Command<EbayCommand>().EndItemListing(context.CommerceContext, listedItem, "Incorrect").ConfigureAwait(false);
                }

            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Ebay.DoActionEndItem.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "DoActionEndListingAll.Run.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
            }

            return entityView;
        }
    }
}

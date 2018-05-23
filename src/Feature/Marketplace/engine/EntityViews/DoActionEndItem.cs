using System;
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
    [PipelineDisplayName("DoActionEndItem")]
    public class DoActionEndItem : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionStartSelling"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionEndItem(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("Ebay-EndItem", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);
                var reason = entityView.Properties.First(p => p.Name == "Reason").Value ?? "";

                var sellableItemId = entityView.EntityId;
                if (string.IsNullOrEmpty(sellableItemId))
                {
                    sellableItemId = entityView.ItemId;
                }
                if (sellableItemId.Contains("Entity-Category-"))
                {
                    sellableItemId = entityView.ItemId;
                }
                var sellableItem = await this._commerceCommander.GetEntity<SellableItem>(context.CommerceContext, sellableItemId);

                await this._commerceCommander.Command<EbayCommand>().EndItemListing(context.CommerceContext, sellableItem, reason);

            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Ebay.DoActionEndItem.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "DocActionEndItem.Exception", new Object[] { ex }, ex.Message);
            }

            return entityView;
        }
    }
}

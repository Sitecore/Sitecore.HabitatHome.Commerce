using System;
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
    [PipelineDisplayName("DoActionSyncItem")]
    public class DoActionFixSyncItem : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionFixSyncItem"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionFixSyncItem(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("Ebay-FixSyncItem", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);
                //var reason = entityView.Properties.First(p => p.Name == "Reason").Value ?? "";

                var sellableItemId = entityView.EntityId;
                if (string.IsNullOrEmpty(sellableItemId))
                {
                    sellableItemId = entityView.ItemId;
                }
                if (sellableItemId.Contains("Entity-Category-"))
                {
                    sellableItemId = entityView.ItemId;
                }
                var sellableItem = await this._commerceCommander
                    .GetEntity<SellableItem>(context.CommerceContext, sellableItemId).ConfigureAwait(false);

                var ebayItem = await this._commerceCommander.Command<EbayCommand>()
                    .AddItem(context.CommerceContext, sellableItem).ConfigureAwait(false);

                var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, sellableItem).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Ebay.DoActionSyncItem.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "DoActionFixSyncItem.Run.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
            }

            return entityView;
        }
    }
}

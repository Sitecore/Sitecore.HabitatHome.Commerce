using System;
using System.Linq;
using System.Threading.Tasks;
using eBay.Service.Core.Soap;
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
    [PipelineDisplayName("DoActionStartSelling")]
    public class DoActionStartSelling : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionStartSelling"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionStartSelling(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("Ebay-StartSelling", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);
                var immediateListing = entityView.Properties.First(p => p.Name == "ImmediateListing").Value ?? "";
                var isImmediateListing = System.Convert.ToBoolean(immediateListing);

                var sellableItemId = entityView.EntityId;
                if (entityView.EntityId.Contains("Entity-Category-"))
                {
                    sellableItemId = entityView.ItemId;
                }

                var sellableItem = await this._commerceCommander.GetEntity<SellableItem>(context.CommerceContext, sellableItemId).ConfigureAwait(false);

                var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();

                var ebayItem = new ItemType();
                if (isImmediateListing)
                {
                    if (ebayItemComponent.Status == "Ended")
                    {
                        try
                        {
                            var result = await this._commerceCommander.Command<EbayCommand>().RelistItem(context.CommerceContext, sellableItem).ConfigureAwait(false);
                        }
                        catch(Exception ex)
                        {
                            context.Logger.LogError($"Ebay.DoActionStartSelling.Exception: Message={ex.Message}");
                            await context.CommerceContext.AddMessage("Error", "DoActionStartSelling.Run.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        ebayItem = await this._commerceCommander.Command<EbayCommand>().AddItem(context.CommerceContext, sellableItem).ConfigureAwait(false);
                    }
                }
                else
                {
                    ebayItemComponent.Status = "Pending";
                    sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Pending");
                }

                var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, sellableItem).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Ebay.DoActionStartSelling.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "DoActionStartSelling.Run.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
            }

            return entityView;
        }
    }
}

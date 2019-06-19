using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.EBay.Engine.Components;
using Sitecore.HabitatHome.Feature.EBay.Engine.Entities;

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
    [PipelineDisplayName("DoActionConfigure")]
    public class DoActionConfigure : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionConfigure"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionConfigure(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("Ebay-Configure", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

                var returnsPolicy = entityView.Properties.First(p => p.Name == "ReturnsPolicy").Value ?? "";
                var inventorySet = entityView.Properties.First(p => p.Name == "InventorySet").Value ?? "";

                //var businessUser = await this._commerceCommander.Command<BusinessUserCommander>().CurrentBusinessUser(context.CommerceContext);

                var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true).ConfigureAwait(false);
                if (!ebayConfig.IsPersisted)
                {
                    ebayConfig.Id = "Entity-EbayConfigEntity-Global";
                }

                //var ebayBusinessUserComponent = businessUser.GetComponent<EbayBusinessUserComponent>();
                //ebayBusinessUserComponent.EbayToken = ebayToken;

                var ebayConfigComponent = ebayConfig.GetComponent<EbayGlobalConfigComponent>();
                ebayConfigComponent.ReturnsPolicy = returnsPolicy;
                ebayConfigComponent.InventorySet = inventorySet;

                //var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, businessUser);
                var persistEbayConfigResult = await this._commerceCommander.PersistEntity(context.CommerceContext, ebayConfig).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionConfigure.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "DoActionConfigure.Run.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
            }

            return entityView;
        }
    }
}

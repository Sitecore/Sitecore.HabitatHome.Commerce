using System;
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
    [PipelineDisplayName("DoActionRemoveToken")]
    public class DoActionRemoveToken : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionRemoveToken"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionRemoveToken(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("Ebay-RemoveToken", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

                //var ebayToken = entityView.Properties.First(p => p.Name == "EbayToken").Value ?? "";

                //var businessUser = await this._commerceCommander.Command<BusinessUserCommander>().CurrentBusinessUser(context.CommerceContext);

                var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true);
                if (!ebayConfig.IsPersisted)
                {
                    ebayConfig.Id = "Entity-EbayConfigEntity-Global";
                }

                //var ebayBusinessUserComponent = businessUser.GetComponent<EbayBusinessUserComponent>();
                //ebayBusinessUserComponent.EbayToken = ebayToken;

                var ebayConfigComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();
                ebayConfigComponent.EbayToken = "";

                //var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, businessUser);
                var persistEbayConfigResult = await this._commerceCommander.PersistEntity(context.CommerceContext, ebayConfig);

            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionRemoveToken.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "DoActionRemoveToken.Run.Exception", new Object[] { ex }, ex.Message);
            }

            return entityView;
        }
    }
}

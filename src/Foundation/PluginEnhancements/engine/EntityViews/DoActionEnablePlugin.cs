using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Foundation.PluginEnhancements.Engine.Commands;

namespace Sitecore.HabitatHome.Foundation.PluginEnhancements.Engine.EntityViews
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
    [PipelineDisplayName("DoActionEnablePlugin")]
    public class DoActionEnablePlugin : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionEnablePlugin"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionEnablePlugin(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }


        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {

            if (entityView == null
                || !entityView.Action.Contains("Roles.EnablePlugin"))
            {
                return entityView;
            }

            try
            {
                var pluginName = entityView.Action.Replace("Roles.EnablePlugin.", "");

                var userPluginOptions = await this._commerceCommander.Command<PluginCommander>().CurrentUserSettings(context.CommerceContext, this._commerceCommander).ConfigureAwait(false);

                userPluginOptions.EnabledPlugins.Add(pluginName);

                var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, userPluginOptions).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddDashboardEntity.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}

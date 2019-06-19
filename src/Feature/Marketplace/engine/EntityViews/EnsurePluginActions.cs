using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Pipelines;                         
using Sitecore.HabitatHome.Foundation.PluginEnhancements.Engine.Commands;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.EntityViews
{
    /// <summary>
    /// Defines a block which populates an EntityView for a Sample Page in the Sitecore Commerce Focused Commerce Experience.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EnsurePluginActions")]
    public class EnsurePluginActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {

        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsurePluginActions"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsurePluginActions(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {

            if (!string.IsNullOrEmpty(entityView.Action))
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<Policies.PluginPolicy>();
            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            var userPluginOptions = await this._commerceCommander.Command<PluginCommander>().CurrentUserSettings(context.CommerceContext, this._commerceCommander).ConfigureAwait(false);

            if (userPluginOptions.EnabledPlugins.Contains("Sitecore.HabitatHome.Feature.Ebay.Engine"))
            {
                if (userPluginOptions.HasPolicy<Policies.PluginPolicy>())
                {
                    pluginPolicy = userPluginOptions.GetPolicy<Policies.PluginPolicy>();
                }
                else
                {
                    pluginPolicy.IsDisabled = false;
                }
            }
            else
            {
                pluginPolicy.IsDisabled = true;
            }

            if (pluginPolicy.IsDisabled)
            {
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = $"Roles.EnablePlugin.Sitecore.HabitatHome.Feature.Ebay.Engine",
                    DisplayName = $"Enable Ebay Integration",
                    Description = $"Enable Ebay",
                    IsEnabled = true, ConfirmationMessage = "This is where a confirmation message goes",
                    
                    RequiresConfirmation = true,
                    EntityView = "",
                    Icon = "box_into"
                });
            }
            else
            {
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = $"Roles.DisablePlugin.Sitecore.HabitatHome.Feature.Ebay.Engine",
                    DisplayName = $"Disable Plugin (Sitecore.HabitatHome.Feature.Ebay.Engine)",
                    Description = $"Disable Plugin (Sitecore.HabitatHome.Feature.Ebay.Engine)",
                    IsEnabled = true,
                    ConfirmationMessage = "This is where a confirmation message goes",

                    RequiresConfirmation = true,
                    EntityView = "",
                    Icon = "box_out"
                });
            }

            return entityView;
        }
    }
}

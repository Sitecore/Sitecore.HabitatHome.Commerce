
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;                                          
using Sitecore.HabitatHome.Feature.EBay.Engine.Policies;
using Sitecore.HabitatHome.Foundation.PluginEnhancements.Engine.Commands;


namespace Sitecore.HabitatHome.Feature.EBay.Engine.EntityViews
{
    /// <summary>
    /// Defines a block which populates an EntityView for a list of Coupons with the View named Public Coupons.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EnsureNavigationView")]
    public class EnsureNavigationView : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureNavigationView"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureNavigationView(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "ToolsNavigation")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<Policies.PluginPolicy>();

            var userPluginOptions = await this._commerceCommander.Command<PluginCommander>()
                .CurrentUserSettings(context.CommerceContext, this._commerceCommander);
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

            if (!pluginPolicy.IsDisabled)
            {
                var newEntityView = new EntityView
                {
                    Name = "MarketplacesDashboard",
                    DisplayName = "Marketplaces",
                    Icon = pluginPolicy.Icon,
                    ItemId = "MarketplacesDashboard"
                };

                entityView.ChildViews.Add(newEntityView);
            }
            return entityView;
        }
    }
}

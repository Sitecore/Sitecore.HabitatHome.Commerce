using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.EBay.Engine.Components;
using Sitecore.HabitatHome.Feature.EBay.Engine.Entities;

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
    [PipelineDisplayName("FormConfigure")]
    public class FormConfigure : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormConfigure"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public FormConfigure(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>Runs the Command.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "Ebay-FormConfigure")
            {
                return entityView;
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", new int?(), true).ConfigureAwait(false);

            var ebayGlobalConfigComponent = ebayConfig.GetComponent<EbayGlobalConfigComponent>();


            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "ReturnsPolicy",
                    DisplayName = "Returns Policy",
                    IsHidden = false,
                    //IsReadOnly = true,
                    IsRequired = true,
                    RawValue = ebayGlobalConfigComponent.ReturnsPolicy
                });

            //entityView.Properties.Add(
            //    new ViewProperty
            //    {
            //        Name = "InventorySet",
            //        DisplayName = "Inventory Set",
            //        IsHidden = false,
            //        //IsReadOnly = true,
            //        IsRequired = true,
            //        RawValue = ebayGlobalConfigComponent.InventorySet
            //    });

            var inventorySets = await this._commerceCommander.Command<GetInventorySetsCommand>().Process(context.CommerceContext).ConfigureAwait(false);


            return entityView;
        }


    }

}

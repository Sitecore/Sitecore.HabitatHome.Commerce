
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

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
    [PipelineDisplayName("FormRegisterToken")]
    public class FormRegisterToken : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormRegisterToken"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public FormRegisterToken(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>Runs the Command.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "Ebay-FormRegisterToken")
            {
                return Task.FromResult(entityView);
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            //var pluginPolicy = context.GetPolicy<PluginPolicy>();

            //entityView.Properties.Add(
            //    new ViewProperty
            //    {
            //        Name = "",
            //        IsHidden = false,
            //        IsReadOnly = true,
            //        OriginalType = "Html",
            //        UiType = "Html",
            //        RawValue = "<img alt='This is the alternate' height=50 width=100 src='https://www.paypalobjects.com/webstatic/en_AR/mktg/merchant/pages/sell-on-ebay/image-ebay.png' style=''/>"
            //    });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "EbayToken",
                    DisplayName = "Ebay User Token",
                    IsHidden = false,
                    IsReadOnly = false,
                    IsRequired = true,
                    UiType = "", 
                    RawValue = ""
                });

            //entityView.Properties.Add(
            //    new ViewProperty
            //    {
            //        Name = "Subtitle",
            //        IsHidden = false,
            //        //IsReadOnly = true,
            //        IsRequired = true,
            //        RawValue = "Item Default Subtitle"
            //    });

            return Task.FromResult(entityView);
        }


    }

}

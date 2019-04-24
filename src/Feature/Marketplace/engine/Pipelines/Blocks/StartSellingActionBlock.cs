// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartSellingActionBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using eBay.Service.Core.Soap;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.Pipelines.Blocks
{
    /// <summary>
    /// Defines a block
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Sample.SampleArgument,
    ///         Sitecore.Commerce.Plugin.Sample.SampleEntity, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("Ebay.StartSellingActionBlock")]
    public class StartSellingActionBlock : PipelineBlock<ItemType, ItemType, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartSellingActionBlock"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public StartSellingActionBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="ebayItem">
        /// The ebayItem argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="EbayConfigEntity"/>.
        /// </returns>
        public override async Task<ItemType> Run(ItemType ebayItem, CommercePipelineExecutionContext context)
        {
            Condition.Requires(ebayItem).IsNotNull($"{this.Name}: The argument can not be null");

            try
            {
                var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

                var entityView = context.CommerceContext.GetObjects<EntityView>().FirstOrDefault();
                if (entityView != null && entityView.Action == "Ebay-StartSelling")
                {
                    var listingDuration = entityView.Properties.First(p => p.Name == "ListingDuration").Value ?? "";
                    var quantitySubmitted = entityView.Properties.First(p => p.Name == "Quantity").Value ?? "";
                    var quantity = System.Convert.ToInt32(quantitySubmitted);

                    ebayItem.ListingDuration = "Days_" + listingDuration;
                    ebayItem.Quantity = quantity;
                    ebayItem.QuantityAvailable = quantity;

                }
            }
            catch(Exception ex)
            {
                context.Logger.LogError($"Ebay.StartSellingActionBlock.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "StartSellingActionBlock.Run.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
            }
            return ebayItem;
        }
    }
}

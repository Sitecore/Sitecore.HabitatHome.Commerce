// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using eBay.Service.Core.Soap;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
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
    [PipelineDisplayName("Ebay.PrepareItemStart")]
    public class PrepareItemStartBlock : PipelineBlock<SellableItem, ItemType, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="sellableItem">
        /// The sellableItem argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="EbayConfigEntity"/>.
        /// </returns>
        public override async Task<ItemType> Run(SellableItem sellableItem, CommercePipelineExecutionContext context)
        {
            Condition.Requires(sellableItem).IsNotNull($"{this.Name}: The argument can not be null");
            //var result2 = await Task.Run(() => new EbayConfigEntity() { Id = Guid.NewGuid().ToString() });

            var item = new ItemType();
            try
            {
                item = new ItemType
                {
                    SKU = sellableItem.Id.Replace("Entity-SellableItem-", ""),
                    Currency = CurrencyCodeType.USD,
                    Country = CountryCodeType.US,
                    ListingDuration = "Days_7",
                    //PrimaryCategory = new CategoryType { CategoryID = "12" },
                    //PrimaryCategory = new CategoryType { CategoryID = "71258" },
                    PrimaryCategory = new CategoryType { CategoryID = "20713" },
                    Location = "Dallas, TX",
                    Quantity = 10
                };

            }
            catch(Exception ex)
            {
                context.Logger.LogError($"Ebay.PrepareItemStartBlock.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "PrepareItemStartBlock.Run.Exception", new Object[] { ex }, ex.Message);
            }
            return item;
        }
    }
}

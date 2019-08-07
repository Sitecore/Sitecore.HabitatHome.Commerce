// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrepareItemVariationsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using eBay.Service.Core.Soap;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.Pipelines.Blocks
{
    /// <summary>
    /// Defines a PrepareItemVariationsBlock block
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Sample.SampleArgument,
    ///         Sitecore.Commerce.Plugin.Sample.SampleEntity, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("Ebay.PrepareItemVariationsBlock")]
    public class PrepareItemVariationsBlock : PipelineBlock<ItemType, ItemType, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="item">
        /// An ItemType.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="EbayConfigEntity"/>.
        /// </returns>
        public override async Task<ItemType> Run(ItemType item, CommercePipelineExecutionContext context)
        {
            Condition.Requires(item).IsNotNull($"{this.Name}: The argument can not be null");
            //var result2 = await Task.Run(() => new EbayConfigEntity() { Id = Guid.NewGuid().ToString() });

            //var item = new ItemType();
            try
            {
                //item = new ItemType
                //{
                //    SKU = sellableItem.Id.Replace("Entity-SellableItem-", ""),
                //    Currency = CurrencyCodeType.USD,
                //    Country = CountryCodeType.US,
                //    ListingDuration = "Days_7",
                //    PrimaryCategory = new CategoryType { CategoryID = "12" },
                //    Location = "Dallas, TX",
                //    Quantity = 10
                //};
                var foundEntity = context.CommerceContext.GetObjects<CommerceEntity>().FirstOrDefault(p => p is SellableItem) ;
                if (foundEntity != null)
                {
                    var sellableItem = foundEntity as SellableItem;
                    if (sellableItem.HasComponent<ItemVariationsComponent>())
                    {
                        //This item has variations
                        item.Variations = new VariationsType();
                        item.Variations.Variation = new VariationTypeCollection();

                        var itemVariationsComponent = sellableItem.GetComponent<ItemVariationsComponent>();


                        //if (itemVariationsComponent.GetComponents<ItemVariationComponent>().Count > 1)
                        //{
                            var allColors = new StringCollection() { };

                            foreach (var variationComponent in itemVariationsComponent.ChildComponents.OfType<ItemVariationComponent>())
                            {
                                var newVariation = new VariationType() { VariationTitle = variationComponent.DisplayName };

                                var listPricingPolicy = sellableItem.GetPolicy<ListPricingPolicy>();
                                var listPrice = listPricingPolicy.Prices.FirstOrDefault();

                                newVariation.StartPrice = new AmountType
                                {
                                    currencyID = CurrencyCodeType.USD,
                                    Value = System.Convert.ToDouble(listPrice.Amount, System.Globalization.CultureInfo.InvariantCulture)
                                };

                                newVariation.SKU = variationComponent.Id;
                                newVariation.Quantity = 10;
                                newVariation.VariationSpecifics = new NameValueListTypeCollection();
                                var displayPropertiesComponent = variationComponent.GetComponent<DisplayPropertiesComponent>();

                            if (string.IsNullOrEmpty(displayPropertiesComponent.Color))
                            {
                                displayPropertiesComponent.Color = "None";
                                
                            }

                            newVariation.VariationSpecifics.Add(new NameValueListType() { Name = "Color", Value = new StringCollection() { displayPropertiesComponent.Color } });




                            if (!allColors.Contains(displayPropertiesComponent.Color))
                                {
                                    allColors.Add(displayPropertiesComponent.Color);
                                }

                                item.Variations.Variation.Add(newVariation);

                                item.Variations.VariationSpecificsSet = new NameValueListTypeCollection();
                                item.Variations.VariationSpecificsSet.Add(new NameValueListType() { Name = "Color", Value = allColors });


                            }
                        //} 
                    }
                }
                
                //item.Variations.
                

                //item.Variations.Variation[0].VariationTitle = "variation1";
                //item.Variations.VariationSpecificsSet.Add(new NameValueListType());

            }
            catch(Exception ex)
            {
                context.Logger.LogError($"Ebay.PrepareItemVariationsBlock.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "PrepareItemVariationsBlock.Run.Exception", new Object[] { ex }, ex.Message).ConfigureAwait(false);
            }
            return item;
        }
    }
}

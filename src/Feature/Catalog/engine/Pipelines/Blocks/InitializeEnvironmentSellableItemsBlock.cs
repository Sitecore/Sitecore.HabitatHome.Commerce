// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeEnvironmentSellableItemsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.HabitatHome.Feature.Catalog.Engine.Pipelines.Blocks
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Availability;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which bootstraps sellable items the Habitat sample environment.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{System.String, System.String,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(HabitatHomeConstants.Pipelines.Blocks.InitializeSellableItemsBlock)]
    public class InitializeEnvironmentSellableItemsBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;
        private readonly IFindEntityPipeline _findEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeEnvironmentSellableItemsBlock"/> class.
        /// </summary>
        /// <param name="persistEntityPipeline">
        /// The persist entity pipeline.
        /// </param>
        /// <param name="findEntityPipeline">
        /// The find entity pipeline.
        /// </param>
        public InitializeEnvironmentSellableItemsBlock(IPersistEntityPipeline persistEntityPipeline, IFindEntityPipeline findEntityPipeline)
        {
            this._persistEntityPipeline = persistEntityPipeline;
            this._findEntityPipeline = findEntityPipeline;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="arg">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var artifactSet = "Environment.Habitat.SellableItems-1.0";

            // Check if this environment has subscribed to this Artifact Set
            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
            {
                return arg;
            }

            context.Logger.LogInformation($"{this.Name}.InitializingArtifactSet: ArtifactSet={artifactSet}");

            await this.BootstrapAppliances(context).ConfigureAwait(false);
            await this.BootstrapAudio(context).ConfigureAwait(false);
            await this.BootstrapCameras(context).ConfigureAwait(false);
            await this.BootstrapComputers(context).ConfigureAwait(false);
            await this.BootstrapGiftCards(context).ConfigureAwait(false);

            return arg;
        }

        /// <summary>
        /// Bootstraps the appliances.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task BootstrapAppliances(CommercePipelineExecutionContext context)
        {
            var item = new SellableItem(new List<Component>
                {
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent(new List<Policy>
                            {
                                new ListPricingPolicy(new List<Money>
                                {
                                    new Money("USD", 3029.99M),
                                    new Money("CAD", 3030.99M)
                                })
                            })
                            {
                                Id = "56042591",
                                Name =
                                    "Habitat Viva 4-Door 34.0 Cubic Foot Refrigerator w/ Ice Maker and Wifi (Stainless)",
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent(new List<Policy>
                            {
                                new ListPricingPolicy(new List<Money>
                                {
                                    new Money("USD", 3029.99M),
                                    new Money("CAD", 3030.99M)
                                })
                            })
                            {
                                Id = "56042592",
                                Name = "Habitat Viva 4-Door 34.0 Cubic Foot Refrigerator w/ Ice Maker and Wifi (Black)",
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent(new List<Policy>
                            {
                                new ListPricingPolicy(new List<Money>
                                {
                                    new Money("USD", 3029.99M),
                                    new Money("CAD", 3030.99M)
                                })
                            })
                            {
                                Id = "56042593",
                                Name = "Habitat Viva 4-Door 34.0 Cubic Foot Refrigerator w/ Ice Maker and Wifi (White)",
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                new List<Policy>
                {
                    new ListPricingPolicy(new List<Money> {new Money("USD", 2302.79M), new Money("CAD", 2303.79M)})
                })
            {
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}6042591",
                Name = "Habitat Viva 4-Door 34.0 Cubic Foot Refrigerator with Ice Maker and Wifi"
            };
            await UpsertSellableItem(item, context).ConfigureAwait(false);
        }

        /// <summary>
        /// Bootstraps the audio.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task BootstrapAudio(CommercePipelineExecutionContext context)
        {
            var item = new SellableItem(new List<Component>
                {

                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent(new List<Policy>
                            {
                                new ListPricingPolicy(
                                    new List<Money> {new Money("USD", 423.99M), new Money("CAD", 424.99M)})
                            })
                            {
                                Id = "56042122",
                                Name =
                                    "XSound 7” CD DVD, In-Dash Receiver, 3-Way Speakers, and HabitatPro Installation",
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                new List<Policy>
                {
                    new ListPricingPolicy(new List<Money> {new Money("USD", 423.99M), new Money("CAD", 424.99M)})
                })
            {

                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}6042122",
                Name = "XSound 7” CD DVD, In-Dash Receiver, 3-Way Speakers, and HabitatPro Installation"
            };
            await UpsertSellableItem(item, context).ConfigureAwait(false);
        }

        /// <summary>
        /// Bootstraps the cameras.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>a <see cref="Task"/></returns>
        private async Task BootstrapCameras(CommercePipelineExecutionContext context)
        {
            var item = new SellableItem(new List<Component>
                {
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent(new List<Policy>
                            {
                                new ListPricingPolicy(
                                    new List<Money> {new Money("USD", 189.99M), new Money("CAD", 190.99M)})
                            })
                            {
                                Id = "57042124",
                                Name = "Optix HD Mini Action Camcorder with Remote (White)",
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent(new List<Policy>
                            {
                                new ListPricingPolicy(
                                    new List<Money> {new Money("USD", 189.99M), new Money("CAD", 190.99M)})
                            })
                            {
                                Id = "57042125",
                                Name = "Optix HD Mini Action Camcorder with Remote (Orange)",
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                new List<Policy>
                {
                    new ListPricingPolicy(new List<Money> {new Money("USD", 117.79M), new Money("CAD", 118.79M)})
                })
            {
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}7042124",
                Name = "Optix HD Mini Action Camcorder with Remote"
            };
            await UpsertSellableItem(item, context).ConfigureAwait(false);
        }

        /// <summary>
        /// Bootstraps the computers.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task BootstrapComputers(CommercePipelineExecutionContext context)
        {
            var item = new SellableItem(new List<Component>
                {
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent(new List<Policy>
                            {
                                new ListPricingPolicy(
                                    new List<Money> {new Money("USD", 429.00M), new Money("CAD", 430.00M)})
                            })
                            {
                                Id = "56042179",
                                Name = "Mira 15.6 Laptop—4GB Memory, 1TB Hard Drive",
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                new List<Policy>
                {
                    new ListPricingPolicy(new List<Money> {new Money("USD", 429.00M), new Money("CAD", 430.00M)})
                })
            {
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}6042179",
                Name = "Mira 15.6 Laptop—4GB Memory, 1TB Hard Drive"
            };
            await UpsertSellableItem(item, context).ConfigureAwait(false);

            item = new SellableItem(new List<Component>
            {
                new ItemVariationsComponent
                {
                    ChildComponents = new List<Component>
                    {
                        new ItemVariationComponent(new List<Policy>
                        {
                            new ListPricingPolicy(new List<Money>
                            {
                                new Money("USD", 989.00M),
                                new Money("CAD", 990.00M)
                            })
                        })
                        {
                            Id = "56042190",
                            Name = "Fusion 13.3” 2-in-1—8GB Memory, 256GB Hard Drive",
                            ChildComponents = new List<Component>
                            {
                                //new PhysicalItemComponent()
                            }
                        }
                    }
                }
            }, new List<Policy>
            {
                new ListPricingPolicy(new List<Money> {new Money("USD", 989.00M), new Money("CAD", 990.00M)})
            })
            {
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}6042190",
                Name = "Fusion 13.3” 2-in-1—8GB Memory, 256GB Hard Drive"
            };
            await UpsertSellableItem(item, context).ConfigureAwait(false);

            item = new SellableItem(new List<Component>
                {
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent(new List<Policy>
                            {
                                new ListPricingPolicy(new List<Money>
                                {
                                    new Money("USD", 289.00M),
                                    new Money("CAD", 290.00M)
                                })
                            })
                            {
                                Id = "56042178",
                                Name = "Mira 15.6 Laptop—4GB Memory, 500GB Hard Drive",
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                new List<Policy>
                {
                    new ListPricingPolicy(new List<Money> {new Money("USD", 289.00M), new Money("CAD", 290.00M)})
                })
            {
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}6042178",
                Name = "Mira 15.6 Laptop—4GB Memory, 500GB Hard Drive"
            };
            await UpsertSellableItem(item, context).ConfigureAwait(false);
        }

        /// <summary>
        /// Bootstraps the gift cards.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/></returns>
        private async Task BootstrapGiftCards(CommercePipelineExecutionContext context)
        {
            var giftCardSellableItem = new SellableItem(new List<Component>(), new List<Policy>
            {
                new AvailabilityAlwaysPolicy()
            })
            {
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}GiftCardV2",
                ProductId = "DefaultGiftCardV2",
                Name = "Default GiftCard V2"
                //Components = new List<Component>
                //{
                //    new ListMembershipsComponent { Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() } }
                //}
            };

            await UpsertSellableItem(giftCardSellableItem, context).ConfigureAwait(false);

            var giftCard = new SellableItem(new List<Component>
            {
                new ItemVariationsComponent
                {
                    ChildComponents = new List<Component>
                    {
                        new ItemVariationComponent(new List<Policy>
                        {
                            new AvailabilityAlwaysPolicy(),
                            new ListPricingPolicy(
                                new List<Money> {new Money("USD", 25M), new Money("CAD", 26M)})
                        })
                        {
                            Id = "56042986",
                            Name = "Gift Card"
                        },
                        new ItemVariationComponent(new List<Policy>
                        {
                            new AvailabilityAlwaysPolicy(),
                            new ListPricingPolicy(
                                new List<Money> {new Money("USD", 50M), new Money("CAD", 51M)})
                        })
                        {
                            Id = "56042987",
                            Name = "Gift Card"
                        },
                        new ItemVariationComponent(new List<Policy>
                        {
                            new AvailabilityAlwaysPolicy(),
                            new ListPricingPolicy(
                                new List<Money> {new Money("USD", 100M), new Money("CAD", 101M)})
                        })
                        {
                            Id = "56042988",
                            Name = "Gift Card"
                        }
                    }
                }
            }, new List<Policy>
            {
                new AvailabilityAlwaysPolicy()
            })
            {
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}6042986",
                ProductId = "GiftCard",
                Name = "Default GiftCard"
            };

            await UpsertSellableItem(giftCard, context).ConfigureAwait(false);
        }

        private async Task UpsertSellableItem(SellableItem item, CommercePipelineExecutionContext context)
        {
            if (string.IsNullOrEmpty(item.ProductId))
            {
                item.ProductId = item.Id.SimplifyEntityName().ProposeValidId();
            }

            if (string.IsNullOrEmpty(item.FriendlyId))
            {
                item.FriendlyId = item.Id.SimplifyEntityName();
            }

            if (string.IsNullOrEmpty(item.SitecoreId))
            {
                item.SitecoreId = GuidUtils.GetDeterministicGuidString(item.Id);
            }

            var entity = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), item.Id), context).ConfigureAwait(false);
            if (entity == null)
            {
                await _persistEntityPipeline.Run(new PersistEntityArgument(item), context).ConfigureAwait(false);
                return;
            }

            if (!(entity is SellableItem))
            {
                return;
            }

            var existingSellableItem = entity as SellableItem;

            // Try to merge the items.
            existingSellableItem.Name = item.Name;

            foreach (var policy in item.Policies)
            {
                if (existingSellableItem.HasPolicy(policy.GetType()))
                {
                    existingSellableItem.RemovePolicy(policy.GetType());
                }

                existingSellableItem.SetPolicy(policy);
            }

            if (item.HasComponent<ItemVariationsComponent>())
            {
                var variations = existingSellableItem.GetComponent<ItemVariationsComponent>();

                foreach (var variation in item.GetComponent<ItemVariationsComponent>().ChildComponents.OfType<ItemVariationComponent>())
                {
                    var existingVariation = existingSellableItem.GetVariation(variation.Id);
                    if (existingVariation != null)
                    {
                        existingVariation.Name = variation.Name;

                        foreach (var policy in variation.Policies)
                        {
                            if (existingVariation.Policies.Any(x => x.GetType() == policy.GetType()))
                            {
                                existingVariation.RemovePolicy(policy.GetType());
                            }

                            existingVariation.SetPolicy(policy);
                        }
                    }
                    else
                    {
                        variations.ChildComponents.Add(variation);
                    }
                }
            }

            await _persistEntityPipeline.Run(new PersistEntityArgument(existingSellableItem), context).ConfigureAwait(false);
        }
    }
}

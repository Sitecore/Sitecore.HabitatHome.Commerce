// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeEnvironmentSellableItemsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.AdventureWorks
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
    /// Defines a block which bootstraps sellable items the AdventureWorks sample environment.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{System.String, System.String,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(AwConstants.Pipelines.Blocks.InitializeSellableItemsBlock)]
    public class InitializeEnvironmentSellableItemsBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findEntityPipeline;
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeEnvironmentSellableItemsBlock"/> class.
        /// </summary>
        /// <param name="findEntityPipeline">The find entity pipeline.</param>
        /// <param name="persistEntityPipeline">The persist entity pipeline.</param>
        public InitializeEnvironmentSellableItemsBlock(
            IFindEntityPipeline findEntityPipeline,
            IPersistEntityPipeline persistEntityPipeline)
        {
            _findEntityPipeline = findEntityPipeline;
            _persistEntityPipeline = persistEntityPipeline;
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
            var artifactSet = "Environment.AdventureWorks.SellableItems-1.0";

            // Check if this environment has subscribed to this Artifact Set
            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
            {
                return arg;
            }

            context.Logger.LogInformation($"{this.Name}.InitializingArtifactSet: ArtifactSet={artifactSet}");

            // CS PRODUCTS
            await BootstrapCsGiftCards(context);
            await BootstrapCsSleepingBags(context);
            await BootstrapCsBoots(context);
            await BootstrapCsTents(context);
            await BootstrapCsParkas(context);
            await BootstrapCsPants(context);
            await BootstrapCsHarnesses(context);
            await BootstrapCsCrampos(context);
            await BootstrapCsCarabines(context);
            await BootstrapCsRockshoes(context);
            await BootstrapCsBackpacks(context);
            await BootstrapCsShirts(context);
            await BootstrapCsSupplies(context);

            return arg;
        }

        /// <summary>
        /// Bootstraps the cs gift cards.
        /// </summary>
        /// <param name="context">The context.</param>
        private async Task BootstrapCsGiftCards(CommercePipelineExecutionContext context)
        {
            var giftCard = new SellableItem
            {
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"22565422120".ProposeValidId()}",
                Name = "Default GiftCard",
                Policies = new List<Policy>
                {
                    new AvailabilityAlwaysPolicy()
                },
                Components = new List<Component>
                {
                    //new ListMembershipsComponent { Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() } },
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "010",
                                Name = "Gift Card",
                                Policies = new List<Policy>
                                {
                                    new AvailabilityAlwaysPolicy(),
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 10M), new Money("CAD", 11M) })
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "020",
                                Name = "Gift Card",
                                Policies = new List<Policy>
                                {
                                    new AvailabilityAlwaysPolicy(),
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 20M), new Money("CAD", 21M) })
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "025",
                                Name = "Gift Card",
                                Policies = new List<Policy>
                                {
                                    new AvailabilityAlwaysPolicy(),
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 25M), new Money("CAD", 26M) })
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "050",
                                Name = "Gift Card",
                                Policies = new List<Policy>
                                {
                                    new AvailabilityAlwaysPolicy(),
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 50M), new Money("CAD", 51M) })
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "100",
                                Name = "Gift Card",
                                Policies = new List<Policy>
                                {
                                    new AvailabilityAlwaysPolicy(),
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 100M), new Money("CAD", 101M) })
                                }
                            }
                        }
                    }
                }
            };

            await UpsertSellableItem(giftCard, context);
        }

        /// <summary>
        /// Bootstraps the cs sleeping bags.
        /// </summary>
        /// <param name="context">The context.</param>
        private async Task BootstrapCsSleepingBags(CommercePipelineExecutionContext context)
        {
            var sleepingBag = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "3",
                                Name = "Big Sur (Blue)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 220M), new Money("CAD", 221M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 220M), new Money("CAD", 221M) }) },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW200-12".ProposeValidId()}",
                Name = "Big Sur"
            };
            await UpsertSellableItem(sleepingBag, context);

            var sleepingbag2 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "4",
                                Name = "Day Hike (Blue)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 212M), new Money("CAD", 213M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 212M), new Money("CAD", 213M) }) },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW210-12".ProposeValidId()}",
                Name = "Day Hike"
            };
            await UpsertSellableItem(sleepingbag2, context);

            var sleepingBag3 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "2",
                                Name = "Polar Star (Black)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 320M), new Money("CAD", 321M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW310-12".ProposeValidId()}",
                Name = "Polar Star",
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 320M), new Money("CAD", 321M) }) }
            };
            await UpsertSellableItem(sleepingBag3, context);

            var sleepingBag4 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "1",
                                Name = "North Face Sunspot (Red)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money>
                                    {
                                        new Money("USD", 402M),
                                        new Money("CAD", 403M)
                                    })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW390-12".ProposeValidId()}",
                Name = "North Face Sunspot",
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 402M), new Money("CAD", 403M) }) }
            };
            await UpsertSellableItem(sleepingBag4, context);
        }

        /// <summary>
        /// Bootstraps the cs boots.
        /// </summary>
        /// <param name="context">The context.</param>
        private async Task BootstrapCsBoots(CommercePipelineExecutionContext context)
        {
            var boot = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "9",
                                Name = "Dunes (Light green)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money>
                                    {
                                        new Money("USD", 123M),
                                        new Money("CAD", 124M)
                                    })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 123M), new Money("CAD", 124M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW074-04".ProposeValidId()}",
                Name = "Dunes"
            };
            await UpsertSellableItem(boot, context);

            var boot2 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "6",
                                Name = "Rockies (Green)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 85M), new Money("CAD", 86M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "7",
                                Name = "Rockies (Gray)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 85M), new Money("CAD", 86M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW078-04".ProposeValidId()}",
                Name = "Rockies",
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 85M), new Money("CAD", 86M) }) }
            };
            await UpsertSellableItem(boot2, context);

            var boot3 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "8",
                                Name = "Sierras (Dark brown)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 90M), new Money("CAD", 91M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 90M), new Money("CAD", 91M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW087-04".ProposeValidId()}",
                Name = "Sierras"
            };
            await UpsertSellableItem(boot3, context);

            var boot4 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "5",
                                Name = "Everglades (Brown)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 105M), new Money("CAD", 106M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW098-04".ProposeValidId()}",
                Name = "Everglades",
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 105M), new Money("CAD", 106M) }) }
            };
            await UpsertSellableItem(boot4, context);
        }

        /// <summary>
        /// Bootstraps the cs tents.
        /// </summary>
        /// <param name="context">The context.</param>
        private async Task BootstrapCsTents(CommercePipelineExecutionContext context)
        {
            var tent = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "16",
                                Name = "Scirocco (Black)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 190M), new Money("CAD", 191M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "17",
                                Name = "Scirocco (Grey)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 190M), new Money("CAD", 191M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "18",
                                Name = "Scirocco (Purple)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 190M), new Money("CAD", 191M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                        }
                    }
                },
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 190M), new Money("CAD", 191M) }) },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW190-11".ProposeValidId()}",
                Name = "Scirocco"
            };
            await UpsertSellableItem(tent, context);

            var tent2 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "14",
                                Name = "Aptos (Green)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 340M), new Money("CAD", 341M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "15",
                                Name = "Aptos (Black)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 340M), new Money("CAD", 341M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW335-11".ProposeValidId()}",
                Name = "Aptos",
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 340M), new Money("CAD", 341M) }) }
            };
            await UpsertSellableItem(tent2, context);

            var tent3 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "10",
                                Name = "Starlight (Black)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 425M), new Money("CAD", 426M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "11",
                                Name = "Starlight (Grey)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 425M), new Money("CAD", 426M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW425-11".ProposeValidId()}",
                Name = "Starlight",
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 425M), new Money("CAD", 426M) }) }
            };
            await UpsertSellableItem(tent3, context);

            var tent4 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "12",
                                Name = "Galaxy (Beige)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 535M), new Money("CAD", 536M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "13",
                                Name = "Galaxy (Purple)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 535M), new Money("CAD", 536M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW535-11".ProposeValidId()}",
                Name = "Galaxy",
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 535M), new Money("CAD", 536M) }) }
            };
            await UpsertSellableItem(tent4, context);
        }

        /// <summary>
        /// Bootstraps the cs parkas.
        /// </summary>
        /// <param name="context">The context.</param>
        private async Task BootstrapCsParkas(CommercePipelineExecutionContext context)
        {
            var parka = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent(),
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "23",
                                Name = "Sahara (Green)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 120M), new Money("CAD", 121M) })
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "24",
                                Name = "Sahara (Purple)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 120M), new Money("CAD", 121M) })
                                }
                            }
                        }
                    }
                },
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 120M), new Money("CAD", 121M) }) },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW114-06".ProposeValidId()}",
                Name = "Sahara"
            };
            await UpsertSellableItem(parka, context);

            var parka2 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent(),
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "25",
                                Name = "Crystal (Purple)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 125M), new Money("CAD", 126M) })
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "26",
                                Name = "Crystal (Blue)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 125M), new Money("CAD", 126M) })
                                }
                            }
                        }
                    }
                },
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 125M), new Money("CAD", 126M) }) },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW125-09".ProposeValidId()}",
                Name = "Crystal"
            };
            await UpsertSellableItem(parka2, context);

            var parka3 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent(),
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "19",
                                Name = "Alpine (Red)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 190M), new Money("CAD", 191M) })
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "20",
                                Name = "Alpine (Red)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 190M), new Money("CAD", 191M) })
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "21",
                                Name = "Alpine (Black)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 190M), new Money("CAD", 191M) })
                                }
                            },
                        }
                    }
                },
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 190M), new Money("CAD", 191M) }) },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW188-06".ProposeValidId()}",
                Name = "Alpine"
            };
            await UpsertSellableItem(parka3, context);

            var parka4 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent(),
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "22",
                                Name = "Campos (Blue)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 250M), new Money("CAD", 251M) })
                                }
                            }
                        }
                    }
                },
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 250M), new Money("CAD", 251M) }) },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW250-06".ProposeValidId()}",
                Name = "Campos"
            };
            await UpsertSellableItem(parka4, context);
        }

        /// <summary>
        /// Bootstraps the cs pants.
        /// </summary>
        /// <param name="context">The context.</param>
        private async Task BootstrapCsPants(CommercePipelineExecutionContext context)
        {
            var pant = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "27",
                                Name = "Women's 4 pocket pant. (Green, Size 2)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 35M), new Money("CAD", 36M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "28",
                                Name = "Women's 4 pocket pant. (Beige, Size 2)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 35M), new Money("CAD", 36M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "37",
                                Name = "Women's 4 pocket pant. (Green, Size 16)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 35M), new Money("CAD", 36M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "38",
                                Name = "Women's 4 pocket pant. (Beige, Size 16)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 35M), new Money("CAD", 36M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 35M), new Money("CAD", 36M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW032-01".ProposeValidId()}",
                Name = "Women's 4 pocket pant."
            };
            await UpsertSellableItem(pant, context);

            var pant2 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "29",
                                Name = "Men's 8-pocket conversion pants. (Gray, Size 28)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 46M), new Money("CAD", 47M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "30",
                                Name = "Men's 8-pocket conversion pants. (Brown, Size 28)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 46M), new Money("CAD", 47M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "39",
                                Name = "Men's 8-pocket conversion pants. (Gray, Size 46)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 46M), new Money("CAD", 47M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "40",
                                Name = "Men's 8-pocket conversion pants. (Brown, Size 46)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 46M), new Money("CAD", 47M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 46M), new Money("CAD", 47M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW046-01".ProposeValidId()}",
                Name = "Men's 8-pocket conversion pants."
            };
            await UpsertSellableItem(pant2, context);

            var pant3 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "31",
                                Name = "Unisex drawstring pants. (Green, Size S)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 50M), new Money("CAD", 51M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "32",
                                Name = "Unisex drawstring pants. (Brown, Size S)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 50M), new Money("CAD", 51M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "41",
                                Name = "Unisex drawstring pants. (Green, Size M)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 50M), new Money("CAD", 51M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "42",
                                Name = "Unisex drawstring pants. (Brown, Size M)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 50M), new Money("CAD", 51M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "51",
                                Name = "Unisex drawstring pants. (Green, Size L)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 50M), new Money("CAD", 51M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "52",
                                Name = "Unisex drawstring pants. (Brown, Size L)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 50M), new Money("CAD", 51M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 50M), new Money("CAD", 51M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW048-01".ProposeValidId()}",
                Name = "Unisex drawstring pants."
            };
            await UpsertSellableItem(pant3, context);

            var pant4 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "33",
                                Name = "Unisex hiking pants (Green, Size M)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 58M), new Money("CAD", 59M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "34",
                                Name = "Unisex hiking pants (Beige, Size L)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 58M), new Money("CAD", 59M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "43",
                                Name = "Unisex hiking pants (Green, Size L)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 58M), new Money("CAD", 59M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "44",
                                Name = "Unisex hiking pants (Beige, Size XL)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 58M), new Money("CAD", 59M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "53",
                                Name = "Unisex hiking pants (Green, Size XL)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 58M), new Money("CAD", 59M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "54",
                                Name = "Unisex hiking pants (Beige, Size XXL)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 58M), new Money("CAD", 59M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies = new List<Policy> { new ListPricingPolicy(new List<Money> { new Money("USD", 58M), new Money("CAD", 59M) }) },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW055-01".ProposeValidId()}",
                Name = "Unisex hiking pants"
            };
            await UpsertSellableItem(pant4, context);
        }

        /// <summary>
        /// Bootstraps the cs harnesses.
        /// </summary>
        /// <param name="context">The context.</param>
        private async Task BootstrapCsHarnesses(CommercePipelineExecutionContext context)
        {
            var harness = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "36",
                                Name = "Black Diamond Alpine Bod (Black)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 33M), new Money("CAD", 34M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 33M), new Money("CAD", 34M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW029-10".ProposeValidId()}",
                Name = "Black Diamond Alpine Bod"
            };
            await UpsertSellableItem(harness, context);

            var harness2 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "35",
                                Name = "Black Diamond Bod (Assorted colors)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 33M), new Money("CAD", 34M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 33M), new Money("CAD", 34M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW032-10".ProposeValidId()}",
                Name = "Black Diamond Bod"
            };
            await UpsertSellableItem(harness2, context);

            var harness3 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "37",
                                Name = "El Capitan (Blue)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 60M), new Money("CAD", 61M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 60M), new Money("CAD", 61M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW053-10".ProposeValidId()}",
                Name = "El Capitan"
            };
            await UpsertSellableItem(harness3, context);

            var harness4 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "38",
                                Name = "Petzl Mercury (Assorted colors)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 75M), new Money("CAD", 76M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 75M), new Money("CAD", 76M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW072-10".ProposeValidId()}",
                Name = "Petzl Mercury"
            };
            await UpsertSellableItem(harness4, context);
        }

        /// <summary>
        /// Bootstraps the cs crampos.
        /// </summary>
        /// <param name="context">The context.</param>
        private async Task BootstrapCsCrampos(CommercePipelineExecutionContext context)
        {
            var crampon = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent()
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 109M), new Money("CAD", 110M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW109-15".ProposeValidId()}",
                Name = "Wafflestomper"
            };
            await UpsertSellableItem(crampon, context);

            var crampon2 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent()
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 150M), new Money("CAD", 151M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW145-15".ProposeValidId()}",
                Name = "Edgehugger"
            };
            await UpsertSellableItem(crampon2, context);

            var crampon3 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent()
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 149M), new Money("CAD", 150M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW149-15".ProposeValidId()}",
                Name = "Glory Grip"
            };
            await UpsertSellableItem(crampon3, context);
        }

        /// <summary>
        /// Bootstraps the cs carabines.
        /// </summary>
        /// <param name="context">The context.</param>
        private async Task BootstrapCsCarabines(CommercePipelineExecutionContext context)
        {
            var carabiner = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent()
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 14M), new Money("CAD", 15M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW014-08".ProposeValidId()}",
                Name = "Petzl Spirit"
            };
            await UpsertSellableItem(carabiner, context);

            var carabiner2 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent()
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 10M), new Money("CAD", 11M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW007-08".ProposeValidId()}",
                Name = "Black Diamond Quicksilver II"
            };
            await UpsertSellableItem(carabiner2, context);
        }

        /// <summary>
        /// Bootstraps the cs rockshoes.
        /// </summary>
        /// <param name="context">The context.</param>
        private async Task BootstrapCsRockshoes(CommercePipelineExecutionContext context)
        {
            var rockshoe = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent()
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 135M), new Money("CAD", 136M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW125-05".ProposeValidId()}",
                Name = "Plymouth"
            };
            await UpsertSellableItem(rockshoe, context);

            var rockshoe2 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent()
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 95M), new Money("CAD", 96M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW082-05".ProposeValidId()}",
                Name = "Morro"
            };
            await UpsertSellableItem(rockshoe2, context);

            var rockshoe3 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent()
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 124M), new Money("CAD", 125M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW086-06".ProposeValidId()}",
                Name = "Tuscany"
            };
            await UpsertSellableItem(rockshoe3, context);
        }

        /// <summary>
        /// Bootstraps the cs backpacks.
        /// </summary>
        /// <param name="context">The context.</param>
        private async Task BootstrapCsBackpacks(CommercePipelineExecutionContext context)
        {
            var backpack = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "41",
                                Name = "University (Blau)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money>
                                    {
                                        new Money("USD", 155M),
                                        new Money("CAD", 156M)
                                    })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 155M), new Money("CAD", 156M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW140-13".ProposeValidId()}",
                Name = "University"
            };
            await UpsertSellableItem(backpack, context);

            var backpack2 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "42",
                                Name = "Pacific (Blue)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money>
                                    {
                                        new Money("USD", 151M),
                                        new Money("CAD", 152M)
                                    })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 151M), new Money("CAD", 152M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW151-13".ProposeValidId()}",
                Name = "Pacific"
            };
            await UpsertSellableItem(backpack2, context);

            var backpack3 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "43",
                                Name = "Conestoga (Red)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money>
                                    {
                                        new Money("USD", 184M),
                                        new Money("CAD", 185M)
                                    })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 184M), new Money("CAD", 185M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW175-13".ProposeValidId()}",
                Name = "Conestoga"
            };
            await UpsertSellableItem(backpack3, context);

            var backpack4 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "39",
                                Name = "Aces (Blue)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money>
                                    {
                                        new Money("USD", 325M),
                                        new Money("CAD", 326M)
                                    })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "40",
                                Name = "Aces (Black)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money>
                                    {
                                        new Money("USD", 325M),
                                        new Money("CAD", 326M)
                                    })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 325M), new Money("CAD", 326M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW325-13".ProposeValidId()}",
                Name = "Aces"
            };
            await UpsertSellableItem(backpack4, context);
        }

        /// <summary>
        /// Bootstraps the cs shirts.
        /// </summary>
        /// <param name="context">The context.</param>
        private async Task BootstrapCsShirts(CommercePipelineExecutionContext context)
        {
            var shirt = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "44",
                                Name = "Women's woven tee (Blue, Size S)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 35M), new Money("CAD", 36M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "45",
                                Name = "Women's woven tee (Beige, Size M)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 35M), new Money("CAD", 36M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "46",
                                Name = "Women's woven tee (Green, Size S)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 35M), new Money("CAD", 36M), new Money("EUR", 33M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "54",
                                Name = "Women's woven tee (Blue, Size M)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 35M), new Money("CAD", 36M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "55",
                                Name = "Women's woven tee (Beige, Size L)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 35M), new Money("CAD", 36M), new Money("EUR", 32M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "56",
                                Name = "Women's woven tee (Green, Size M)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 35M), new Money("CAD", 36M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "64",
                                Name = "Women's woven tee (Blue, Size XL)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 35M), new Money("CAD", 36M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 35M), new Money("CAD", 36M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW029-03".ProposeValidId()}",
                Name = "Women's woven tee"
            };
            await UpsertSellableItem(shirt, context);

            var shirt2 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "51",
                                Name = "Men's button-down (Blue, Size S)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 48M), new Money("CAD", 49M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "52",
                                Name = "Men's button-down (Red, Size S)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 48M), new Money("CAD", 49M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "53",
                                Name = "Men's button-down (Beige, Size S)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 48M), new Money("CAD", 49M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "61",
                                Name = "Men's button-down (Blue, Size M)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 48M), new Money("CAD", 49M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "62",
                                Name = "Men's button-down (Red ,Size L)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 48M), new Money("CAD", 49M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "63",
                                Name = "Men's button-down (Beige, Size M)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 48M), new Money("CAD", 49M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "71",
                                Name = "Men's button-down (Blue, Size L)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 48M), new Money("CAD", 49M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 48M), new Money("CAD", 49M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW042-03".ProposeValidId()}",
                Name = "Men's button-down"
            };
            await UpsertSellableItem(shirt2, context);

            var shirt3 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "47",
                                Name = "Men's loose-weave polo (Red, Size S)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 45M), new Money("CAD", 46M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "48",
                                Name = "Men's loose-weave polo (Blue, Size L)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 45M), new Money("CAD", 46M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "49",
                                Name = "Men's loose-weave polo (White, Size S)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 45M), new Money("CAD", 46M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "50",
                                Name = "Men's loose-weave polo (Beige, Size M)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 45M), new Money("CAD", 46M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "57",
                                Name = "Men's loose-weave polo (Red, Size M)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 45M), new Money("CAD", 46M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "58",
                                Name = "Men's loose-weave polo (Blue, Size XL)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 45M), new Money("CAD", 46M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "59",
                                Name = "Men's loose-weave polo (White, Size M)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 45M), new Money("CAD", 46M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "60",
                                Name = "Men's loose-weave polo (Beige, Size L)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 45M), new Money("CAD", 46M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "67",
                                Name = "Men's loose-weave polo (Red, Size XXL)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 45M), new Money("CAD", 46M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "68",
                                Name = "Men's loose-weave polo (Blue, Size XXL)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 45M), new Money("CAD", 46M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "69",
                                Name = "Men's loose-weave polo (White, Size XL)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 45M), new Money("CAD", 46M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "70",
                                Name = "Men's loose-weave polo (Beige, Size XL)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 45M), new Money("CAD", 46M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 45M), new Money("CAD", 46M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW045-03".ProposeValidId()}",
                Name = "Men's loose-weave polo"
            };
            await UpsertSellableItem(shirt3, context);

            var shirt4 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    new ItemVariationsComponent
                    {
                        ChildComponents = new List<Component>
                        {
                            new ItemVariationComponent
                            {
                                Id = "54",
                                Name = "Unisex long-sleeve button-down (Green, Size S)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 75M), new Money("CAD", 76M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "55",
                                Name = "Unisex long-sleeve button-down (Beige, Size M)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 75M), new Money("CAD", 76M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "64",
                                Name = "Unisex long-sleeve button-down (Green, Size M)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 75M), new Money("CAD", 76M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "65",
                                Name = "Unisex long-sleeve button-down (Beige, Size L)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 75M), new Money("CAD", 75M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "74",
                                Name = "Unisex long-sleeve button-down (Green, Size XL)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 75M), new Money("CAD", 76M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            },
                            new ItemVariationComponent
                            {
                                Id = "75",
                                Name = "Unisex long-sleeve button-down (Beige, Size XL)",
                                Policies = new List<Policy>
                                {
                                    new ListPricingPolicy(new List<Money> { new Money("USD", 75M), new Money("CAD", 76M) })
                                },
                                ChildComponents = new List<Component>
                                {
                                    //new PhysicalItemComponent()
                                }
                            }
                        }
                    }
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 75M), new Money("CAD", 76M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW075-03".ProposeValidId()}",
                Name = "Unisex long-sleeve button-down"
            };
            await UpsertSellableItem(shirt4, context);
        }

        /// <summary>
        /// Bootstraps the cs supplies.
        /// </summary>
        /// <param name="context">The context.</param>
        private async Task BootstrapCsSupplies(CommercePipelineExecutionContext context)
        {
            var supply = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent()
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> { new Money("USD", 44.75M), new Money("CAD", 45.75M) })
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW051-14".ProposeValidId()}",
                Name = "Wolfgang"
            };
            await UpsertSellableItem(supply, context);

            var supply2 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent {Name = "Adventure Works Catalog"},
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> {CommerceEntity.ListName<SellableItem>()}
                    //},
                    //new PhysicalItemComponent()
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> {new Money("USD", 59M), new Money("CAD", 60M)})
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW475-14".ProposeValidId()}",
                Name = "Scoutpride"
            };
            await UpsertSellableItem(supply2, context);

            var supply3 = new SellableItem
            {
                Components = new List<Component>
                {
                    //new CatalogComponent { Name = "Adventure Works Catalog" },
                    //new ListMembershipsComponent
                    //{
                    //    Memberships = new List<string> { CommerceEntity.ListName<SellableItem>() }
                    //},
                    //new PhysicalItemComponent()
                },
                Policies =
                    new List<Policy>
                    {
                        new ListPricingPolicy(new List<Money> {new Money("USD", 9M), new Money("CAD", 10M)})
                    },
                Id = $"{CommerceEntity.IdPrefix<SellableItem>()}{"AW425-14".ProposeValidId()}",
                Name = "Surelite"
            };
            await UpsertSellableItem(supply3, context);
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

            var entity = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), item.Id), context);
            if (entity == null)
            {
                await _persistEntityPipeline.Run(new PersistEntityArgument(item), context);
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

            await _persistEntityPipeline.Run(new PersistEntityArgument(existingSellableItem), context);
        }
    }
}

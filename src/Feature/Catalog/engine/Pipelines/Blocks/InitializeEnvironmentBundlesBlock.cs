// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeEnvironmentBundlesBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.HabitatHome.Feature.Catalog.Engine.Pipelines.Blocks
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Ensure that a bundle is created
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{System.String, System.String,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(HabitatHomeConstants.Pipelines.Blocks.InitializeBundlesBlock)]
    public class InitializeEnvironmentBundlesBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        private readonly IFindEntityPipeline _findEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeEnvironmentBundlesBlock"/> class.
        /// </summary>
        public InitializeEnvironmentBundlesBlock(CommerceCommander commerceCommander, IFindEntityPipeline findEntityPipeline)
        {
            this._commerceCommander = commerceCommander;
            this._findEntityPipeline = findEntityPipeline;
        }

        /// <summary>
        /// Executes the block.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var artifactSet = "Environment.Habitat.SellableItems-1.0";

            // Check if this environment has subscribed to this Artifact Set
            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
            {
                return arg;
            }

            // Ensure that the new items will be published right away
            context.CommerceContext.Environment.SetPolicy(new AutoApprovePolicy());

            await CreateExampleBundles(context).ConfigureAwait(false);

            context.CommerceContext.Environment.RemovePolicy(typeof(AutoApprovePolicy));

            return arg;
        }

        private async Task CreateExampleBundles(CommercePipelineExecutionContext context)
        {
            var bundle1 = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-6001001"), context).ConfigureAwait(false);
            if (bundle1 == null)
            {
                // First bundle
                bundle1 =
                await this._commerceCommander.Command<CreateBundleCommand>().Process(
                    context.CommerceContext,
                    "Static",
                    "6001001",
                    "SmartWiFiBundle",
                    "Smart WiFi Bundle",
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    new[] { "smart", "wifi", "bundle" },
                    new List<BundleItem>
                    {
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042964|56042964",
                            Quantity = 1
                        },
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042971|56042971",
                            Quantity = 1
                        }
                    }).ConfigureAwait(false);

                // Set image and list price for bundle
                bundle1.GetComponent<ImagesComponent>().Images.Add("65703328-1456-48da-a693-bad910d7d1fe");

                bundle1.SetPolicy(
                    new ListPricingPolicy(
                        new List<Money>
                        {
                        new Money("USD", 200.00M),
                        new Money("CAD", 250.00M)
                        }));

                await this._commerceCommander.Pipeline<IPersistEntityPipeline>()
                    .Run(new PersistEntityArgument(bundle1), context).ConfigureAwait(false);

                // Associate bundle to parent category
                await this._commerceCommander.Command<AssociateSellableItemToParentCommand>().Process(
                    context.CommerceContext,
                    "Entity-Catalog-Habitat_Master",
                    "Entity-Category-Habitat_Master-Connected home",
                    bundle1.Id).ConfigureAwait(false);
            }

            var bundle2 = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-6001002"), context).ConfigureAwait(false);
            if (bundle2 == null)
            {
                // Second bundle
                bundle2 =
                await this._commerceCommander.Command<CreateBundleCommand>().Process(
                    context.CommerceContext,
                    "Static",
                    "6001002",
                    "ActivityTrackerCameraBundle",
                    "Activity Tracker & Camera Bundle",
                    "Sample bundle containting two activity trackers and two cameras.",
                    "Striva Wearables",
                    string.Empty,
                    string.Empty,
                    new[] { "activitytracker", "camera", "bundle" },
                    new List<BundleItem>
                    {
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042896|56042896",
                            Quantity = 2
                        },
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-7042066|57042066",
                            Quantity = 2
                        }
                    }).ConfigureAwait(false);

                // Set image and list price for bundle
                bundle2.GetComponent<ImagesComponent>().Images.Add("003c9ee5-2d97-4a6c-bb9e-24e110cd7645");

                bundle2.SetPolicy(
                    new ListPricingPolicy(
                        new List<Money>
                        {
                        new Money("USD", 220.00M),
                        new Money("CAD", 280.00M)
                        }));

                await this._commerceCommander.Pipeline<IPersistEntityPipeline>()
                    .Run(new PersistEntityArgument(bundle2), context).ConfigureAwait(false);

                // Associate bundle to parent category
                await this._commerceCommander.Command<AssociateSellableItemToParentCommand>().Process(
                    context.CommerceContext,
                    "Entity-Catalog-Habitat_Master",
                    "Entity-Category-Habitat_Master-Fitness Activity Trackers",
                    bundle2.Id).ConfigureAwait(false);
            }

            var bundle3 = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-6001003"), context).ConfigureAwait(false);
            if (bundle3 == null)
            {
                // Third bundle
                bundle3 =
                await this._commerceCommander.Command<CreateBundleCommand>().Process(
                    context.CommerceContext,
                    "Static",
                    "6001003",
                    "RefrigeratorFlipPhoneBundle",
                    "Refrigerator & Flip Phone Bundle",
                    "Sample bundle containting a refrigerator and two flip phones.",
                    "Viva Refrigerators",
                    string.Empty,
                    string.Empty,
                    new[] { "refrigerator", "flipphone", "bundle" },
                    new List<BundleItem>
                    {
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042567|56042568",
                            Quantity = 1
                        },
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042331|56042331",
                            Quantity = 2
                        },
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042896|56042896",
                            Quantity = 3
                        },
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-7042066|57042066",
                            Quantity = 4
                        }
                    }).ConfigureAwait(false);

                // Set image and list price for bundle
                bundle3.GetComponent<ImagesComponent>().Images.Add("372d8bc6-6888-4375-91c1-f3bee2d31558");

                bundle3.SetPolicy(
                    new ListPricingPolicy(
                        new List<Money>
                        {
                        new Money("USD", 10.00M),
                        new Money("CAD", 20.00M)
                        }));

                await this._commerceCommander.Pipeline<IPersistEntityPipeline>()
                    .Run(new PersistEntityArgument(bundle3), context).ConfigureAwait(false);

                // Associate bundle to parent category
                await this._commerceCommander.Command<AssociateSellableItemToParentCommand>().Process(
                    context.CommerceContext,
                    "Entity-Catalog-Habitat_Master",
                    "Entity-Category-Habitat_Master-Appliances",
                    bundle3.Id).ConfigureAwait(false);
            }

            var bundle4 = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-6001004"), context).ConfigureAwait(false);
            if (bundle4 == null)
            {
                // Fourth bundle with digital items
                bundle4 =
                await this._commerceCommander.Command<CreateBundleCommand>().Process(
                    context.CommerceContext,
                    "Static",
                    "6001004",
                    "GiftCardAndSubscriptionBundle",
                    "Gift Card & Subscription Bundle",
                    "Sample bundle containting a gift card and two subscriptions.",
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    new[] { "bundle", "giftcard", "entitlement" },
                    new List<BundleItem>
                    {
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042986|56042987",
                            Quantity = 1
                        },
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042453|56042453",
                            Quantity = 2
                        }
                    }).ConfigureAwait(false);

                // Set image and list price for bundle
                bundle4.GetComponent<ImagesComponent>().Images.Add("7b57e6e0-a4ef-417e-809c-572f2e30aef7");

                bundle4.SetPolicy(
                    new ListPricingPolicy(
                        new List<Money>
                        {
                        new Money("USD", 10.00M),
                        new Money("CAD", 20.00M)
                        }));

                await this._commerceCommander.Pipeline<IPersistEntityPipeline>()
                    .Run(new PersistEntityArgument(bundle4), context).ConfigureAwait(false);

                // Associate bundle to parent category
                await this._commerceCommander.Command<AssociateSellableItemToParentCommand>().Process(
                    context.CommerceContext,
                    "Entity-Catalog-Habitat_Master",
                    "Entity-Category-Habitat_Master-eGift Cards and Gift Wrapping",
                    bundle4.Id).ConfigureAwait(false);
            }

            var bundle5 = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-6001005"), context).ConfigureAwait(false);
            if (bundle5 == null)
            {
                // Preorderable bundle
                bundle5 =
                await this._commerceCommander.Command<CreateBundleCommand>().Process(
                    context.CommerceContext,
                    "Static",
                    "6001005",
                    "PreorderableBundle",
                    "Preorderable Bundle",
                    "Sample bundle containting a phone and headphones.",
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    new[] { "bundle" },
                    new List<BundleItem>
                    {
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042305|56042305",
                            Quantity = 1
                        },
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042059|56042059",
                            Quantity = 1
                        }
                    }).ConfigureAwait(false);

                // Set image and list price for bundle
                bundle5.GetComponent<ImagesComponent>().Images.Add("b0b07d7b-ddaf-4798-8eb9-af7f570af3fe");

                bundle5.SetPolicy(
                    new ListPricingPolicy(
                        new List<Money>
                        {
                        new Money("USD", 44.99M),
                        new Money("CAD", 59.99M)
                        }));

                await this._commerceCommander.Pipeline<IPersistEntityPipeline>()
                    .Run(new PersistEntityArgument(bundle5), context).ConfigureAwait(false);

                // Associate bundle to parent category
                await this._commerceCommander.Command<AssociateSellableItemToParentCommand>().Process(
                    context.CommerceContext,
                    "Entity-Catalog-Habitat_Master",
                    "Entity-Category-Habitat_Master-Phones",
                    bundle5.Id).ConfigureAwait(false);
            }

            var bundle6 = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-6001006"), context).ConfigureAwait(false);
            if (bundle6 == null)
            {
                // Backorderable bundle
                bundle6 =
                await this._commerceCommander.Command<CreateBundleCommand>().Process(
                    context.CommerceContext,
                    "Static",
                    "6001006",
                    "BackorderableBundle",
                    "Backorderable Bundle",
                    "Sample bundle containting a phone and headphones.",
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    new[] { "bundle" },
                    new List<BundleItem>
                    {
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042305|56042305",
                            Quantity = 1
                        },
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042058|56042058",
                            Quantity = 1
                        }
                    }).ConfigureAwait(false);

                // Set image and list price for bundle
                bundle6.GetComponent<ImagesComponent>().Images.Add("b0b07d7b-ddaf-4798-8eb9-af7f570af3fe");

                bundle6.SetPolicy(
                    new ListPricingPolicy(
                        new List<Money>
                        {
                        new Money("USD", 44.99M),
                        new Money("CAD", 59.99M)
                        }));

                await this._commerceCommander.Pipeline<IPersistEntityPipeline>()
                    .Run(new PersistEntityArgument(bundle6), context).ConfigureAwait(false);

                // Associate bundle to parent category
                await this._commerceCommander.Command<AssociateSellableItemToParentCommand>().Process(
                    context.CommerceContext,
                    "Entity-Catalog-Habitat_Master",
                    "Entity-Category-Habitat_Master-Phones",
                    bundle6.Id).ConfigureAwait(false);
            }

            var bundle7 = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-6001007"), context).ConfigureAwait(false);
            if (bundle7 == null)
            {
                // Backorderable bundle
                bundle7 =
                await this._commerceCommander.Command<CreateBundleCommand>().Process(
                    context.CommerceContext,
                    "Static",
                    "6001007",
                    "PreorderableBackorderableBundle",
                    "Preorderable / Backorderable Bundle",
                    "Sample bundle containting headphones.",
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    new[] { "bundle" },
                    new List<BundleItem>
                    {
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042058|56042058",
                            Quantity = 1
                        },
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042059|56042059",
                            Quantity = 1
                        }
                    }).ConfigureAwait(false);

                // Set image and list price for bundle
                bundle7.GetComponent<ImagesComponent>().Images.Add("b0b07d7b-ddaf-4798-8eb9-af7f570af3fe");

                bundle7.SetPolicy(
                    new ListPricingPolicy(
                        new List<Money>
                        {
                        new Money("USD", 44.99M),
                        new Money("CAD", 59.99M)
                        }));

                await this._commerceCommander.Pipeline<IPersistEntityPipeline>()
                    .Run(new PersistEntityArgument(bundle7), context).ConfigureAwait(false);

                // Associate bundle to parent category
                await this._commerceCommander.Command<AssociateSellableItemToParentCommand>().Process(
                    context.CommerceContext,
                    "Entity-Catalog-Habitat_Master",
                    "Entity-Category-Habitat_Master-Audio",
                    bundle7.Id).ConfigureAwait(false);
            }

            var bundle8 = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-6001008"), context).ConfigureAwait(false);
            if (bundle8 == null)
            {
                // Eigth bundle with a gift card only
                bundle8 =
                await this._commerceCommander.Command<CreateBundleCommand>().Process(
                    context.CommerceContext,
                    "Static",
                    "6001008",
                    "GiftCardBundle",
                    "Gift Card Bundle",
                    "Sample bundle containting a gift card.",
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    new[] { "bundle", "entitlement", "giftcard" },
                    new List<BundleItem>
                    {
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042986|56042987",
                            Quantity = 1
                        }
                    }).ConfigureAwait(false);

                // Set image and list price for bundle
                bundle8.GetComponent<ImagesComponent>().Images.Add("7b57e6e0-a4ef-417e-809c-572f2e30aef7");

                bundle8.SetPolicy(
                    new ListPricingPolicy(
                        new List<Money>
                        {
                        new Money("USD", 40.00M),
                        new Money("CAD", 50.00M)
                        }));

                await this._commerceCommander.Pipeline<IPersistEntityPipeline>()
                    .Run(new PersistEntityArgument(bundle8), context).ConfigureAwait(false);

                // Associate bundle to parent category
                await this._commerceCommander.Command<AssociateSellableItemToParentCommand>().Process(
                    context.CommerceContext,
                    "Entity-Catalog-Habitat_Master",
                    "Entity-Category-Habitat_Master-eGift Cards and Gift Wrapping",
                    bundle8.Id).ConfigureAwait(false);
            }

            var bundle9 = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-6001009"), context).ConfigureAwait(false);
            if (bundle9 == null)
            {
                // Warranty bundle
                bundle9 =
                await this._commerceCommander.Command<CreateBundleCommand>().Process(
                    context.CommerceContext,
                    "Static",
                    "6001009",
                    "WarrantyBundle",
                    "Warranty Bundle",
                    "Sample bundle containting a warranty.",
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    new[] { "bundle", "warranty" },
                    new List<BundleItem>
                    {
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-7042259|57042259",
                            Quantity = 1
                        }
                    }).ConfigureAwait(false);

                // Set image and list price for bundle
                bundle9.GetComponent<ImagesComponent>().Images.Add("eebf49f2-74df-4fe6-b77f-f2d1d447827c");

                bundle9.SetPolicy(
                    new ListPricingPolicy(
                        new List<Money>
                        {
                        new Money("USD", 150.00M),
                        new Money("CAD", 200.00M)
                        }));

                await this._commerceCommander.Pipeline<IPersistEntityPipeline>()
                    .Run(new PersistEntityArgument(bundle9), context).ConfigureAwait(false);

                // Associate bundle to parent category
                await this._commerceCommander.Command<AssociateSellableItemToParentCommand>().Process(
                    context.CommerceContext,
                    "Entity-Catalog-Habitat_Master",
                    "Entity-Category-Habitat_Master-eGift Cards and Gift Wrapping",
                    bundle9.Id).ConfigureAwait(false);
            }

            var bundle10 = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-6001010"), context).ConfigureAwait(false);
            if (bundle10 == null)
            {
                // Service bundle
                bundle10 =
                await this._commerceCommander.Command<CreateBundleCommand>().Process(
                    context.CommerceContext,
                    "Static",
                    "6001010",
                    "ServiceBundle",
                    "Service Bundle",
                    "Sample bundle containting a service.",
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    new[] { "bundle", "service" },
                    new List<BundleItem>
                    {
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042418|56042418",
                            Quantity = 1
                        }
                    }).ConfigureAwait(false);

                // Set image and list price for bundle
                bundle10.GetComponent<ImagesComponent>().Images.Add("8b59fe2a-c234-4f92-b84b-7515411bf46e");

                bundle10.SetPolicy(
                    new ListPricingPolicy(
                        new List<Money>
                        {
                        new Money("USD", 150.00M),
                        new Money("CAD", 200.00M)
                        }));

                await this._commerceCommander.Pipeline<IPersistEntityPipeline>()
                    .Run(new PersistEntityArgument(bundle10), context).ConfigureAwait(false);

                // Associate bundle to parent category
                await this._commerceCommander.Command<AssociateSellableItemToParentCommand>().Process(
                    context.CommerceContext,
                    "Entity-Catalog-Habitat_Master",
                    "Entity-Category-Habitat_Master-eGift Cards and Gift Wrapping",
                    bundle10.Id).ConfigureAwait(false);
            }

            var bundle11 = await _findEntityPipeline.Run(new FindEntityArgument(typeof(SellableItem), "Entity-SellableItem-6001011"), context).ConfigureAwait(false);
            if (bundle11 == null)
            {
                // Subscription bundle
                bundle11 =
                await this._commerceCommander.Command<CreateBundleCommand>().Process(
                    context.CommerceContext,
                    "Static",
                    "6001011",
                    "SubscriptionBundle",
                    "Subscription Bundle",
                    "Sample bundle containting a subscription.",
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    new[] { "bundle", "subscription" },
                    new List<BundleItem>
                    {
                        new BundleItem
                        {
                            SellableItemId = "Entity-SellableItem-6042453|56042453",
                            Quantity = 1
                        }
                    }).ConfigureAwait(false);

                // Set image and list price for bundle
                bundle11.GetComponent<ImagesComponent>().Images.Add("22d74215-8e5f-4de3-a9d6-ece3042bd64c");

                bundle11.SetPolicy(
                    new ListPricingPolicy(
                        new List<Money>
                        {
                        new Money("USD", 10.00M),
                        new Money("CAD", 15.00M)
                        }));

                await this._commerceCommander.Pipeline<IPersistEntityPipeline>()
                    .Run(new PersistEntityArgument(bundle11), context).ConfigureAwait(false);

                // Associate bundle to parent category
                await this._commerceCommander.Command<AssociateSellableItemToParentCommand>().Process(
                    context.CommerceContext,
                    "Entity-Catalog-Habitat_Master",
                    "Entity-Category-Habitat_Master-eGift Cards and Gift Wrapping",
                    bundle11.Id).ConfigureAwait(false);
            }
        }
    }
}

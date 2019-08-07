// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeEnvironmentPricingBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.HabitatHome.Feature.Catalog.Engine.Pipelines.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which initializes pricing.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{System.String, System.String,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(HabitatHomeConstants.Pipelines.Blocks.InitializePricingBlock)]
    public class InitializeEnvironmentPricingBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IAddPriceBookPipeline _addPriceBookPipeline;
        private readonly IAddPriceCardPipeline _addPriceCardPipeline;
        private readonly IAddPriceSnapshotPipeline _addPriceSnapshotPipeline;
        private readonly IAddPriceTierPipeline _addPriceTierPipeline;
        private readonly IAddPriceSnapshotTagPipeline _addPriceSnapshotTagPipeline;
        private readonly IPersistEntityPipeline _persistEntityPipeline;
        private readonly IAssociateCatalogToBookPipeline _associateCatalogToBookPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeEnvironmentPricingBlock"/> class.
        /// </summary>
        /// <param name="addPriceBookPipeline">The add price book pipeline.</param>
        /// <param name="addPriceCardPipeline">The add price card pipeline.</param>
        /// <param name="addPriceSnapshotPipeline">The add price snapshot pipeline.</param>
        /// <param name="addPriceTierPipeline">The add price tier pipeline.</param>
        /// <param name="addPriceSnapshotTagPipeline">The add price snapshot tag pipeline.</param>
        /// <param name="persistEntityPipeline">The persist entity pipeline.</param>
        /// <param name="associateCatalogToBookPipeline">The add public coupon pipeline.</param>
        public InitializeEnvironmentPricingBlock(
            IAddPriceBookPipeline addPriceBookPipeline,
            IAddPriceCardPipeline addPriceCardPipeline,
            IAddPriceSnapshotPipeline addPriceSnapshotPipeline,
            IAddPriceTierPipeline addPriceTierPipeline,
            IAddPriceSnapshotTagPipeline addPriceSnapshotTagPipeline,
            IPersistEntityPipeline persistEntityPipeline,
            IAssociateCatalogToBookPipeline associateCatalogToBookPipeline)
        {
            this._addPriceBookPipeline = addPriceBookPipeline;
            this._addPriceCardPipeline = addPriceCardPipeline;
            this._addPriceSnapshotPipeline = addPriceSnapshotPipeline;
            this._addPriceTierPipeline = addPriceTierPipeline;
            this._addPriceSnapshotTagPipeline = addPriceSnapshotTagPipeline;
            this._persistEntityPipeline = persistEntityPipeline;
            this._associateCatalogToBookPipeline = associateCatalogToBookPipeline;
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
        /// The <see cref="string"/>.
        /// </returns>
        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var artifactSet = "Environment.Habitat.Pricing-1.0";

            // Check if this environment has subscribed to this Artifact Set
            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
            {
                return arg;
            }

            context.Logger.LogInformation($"{this.Name}.InitializingArtifactSet: ArtifactSet={artifactSet}");

            try
            {
                var currencySetId = context.GetPolicy<GlobalCurrencyPolicy>().DefaultCurrencySet;

                // BOOK
                var book = await this._addPriceBookPipeline.Run(
                    new AddPriceBookArgument("Habitat_PriceBook")
                    {
                        ParentBook = string.Empty,
                        Description = "Habitat Home price book",
                        DisplayName = "Habitat",
                        CurrencySetId = currencySetId
                    },
                    context).ConfigureAwait(false);

                await this.CreateProductsCard(book, context).ConfigureAwait(false);

                await this.CreateVariantsCard(book, context).ConfigureAwait(false);

                await this.CreateTagsCard(book, context).ConfigureAwait(false);

                await this.AssociateCatalogToBook(book.Name, "Habitat_Master", context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                context.CommerceContext.LogException(this.Name, ex);
            }

            return arg;
        }

        /// <summary>
        /// Creates the products card.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        private async Task CreateProductsCard(PriceBook book, CommercePipelineExecutionContext context)
        {
            context.CommerceContext.RemoveModels(m => m is PriceSnapshotAdded || m is PriceTierAdded);

            var date = DateTimeOffset.UtcNow;

            // CARD
            var adventureCard = await this._addPriceCardPipeline.Run(new AddPriceCardArgument(book, "Habitat_PriceCard"), context).ConfigureAwait(false);

            // READY FOR APPROVAL SNAPSHOT
            adventureCard = await this._addPriceSnapshotPipeline.Run(new PriceCardSnapshotArgument(adventureCard, new PriceSnapshotComponent(date.AddMinutes(-10))), context).ConfigureAwait(false);
            var readyForApprovalSnapshot = adventureCard.Snapshots.FirstOrDefault(s => s.Id.Equals(context.CommerceContext.GetModel<PriceSnapshotAdded>()?.PriceSnapshotId, StringComparison.OrdinalIgnoreCase));

            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, readyForApprovalSnapshot, new PriceTier("USD", 1, 2000M)), context).ConfigureAwait(false);

            context.CommerceContext.RemoveModels(m => m is PriceSnapshotAdded || m is PriceTierAdded);

            // CARD FIRST SNAPSHOT
            adventureCard = await this._addPriceSnapshotPipeline.Run(new PriceCardSnapshotArgument(adventureCard, new PriceSnapshotComponent(date.AddHours(-1))), context).ConfigureAwait(false);
            var firstSnapshot = adventureCard.Snapshots.FirstOrDefault(s => s.Id.Equals(context.CommerceContext.GetModel<PriceSnapshotAdded>()?.PriceSnapshotId, StringComparison.OrdinalIgnoreCase));

            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, firstSnapshot, new PriceTier("USD", 1, 10M)), context).ConfigureAwait(false);
            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, firstSnapshot, new PriceTier("USD", 5, 5M)), context).ConfigureAwait(false);
            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, firstSnapshot, new PriceTier("USD", 10, 1M)), context).ConfigureAwait(false);
            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, firstSnapshot, new PriceTier("CAD", 1, 15M)), context).ConfigureAwait(false);
            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, firstSnapshot, new PriceTier("CAD", 5, 10M)), context).ConfigureAwait(false);
            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, firstSnapshot, new PriceTier("CAD", 10, 5M)), context).ConfigureAwait(false);
            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, firstSnapshot, new PriceTier("EUR", 1, 1M)), context).ConfigureAwait(false);

            context.CommerceContext.RemoveModels(m => m is PriceSnapshotAdded || m is PriceTierAdded);

            // DRAFT SNAPSHOT
            adventureCard = await this._addPriceSnapshotPipeline.Run(new PriceCardSnapshotArgument(adventureCard, new PriceSnapshotComponent(date)), context).ConfigureAwait(false);
            var draftSnapshot = adventureCard.Snapshots.FirstOrDefault(s => s.Id.Equals(context.CommerceContext.GetModel<PriceSnapshotAdded>()?.PriceSnapshotId, StringComparison.OrdinalIgnoreCase));

            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, draftSnapshot, new PriceTier("USD", 1, 1000M)), context).ConfigureAwait(false);

            adventureCard = await this._addPriceSnapshotTagPipeline.Run(new PriceCardSnapshotTagArgument(adventureCard, draftSnapshot, new Tag("new pricing")), context).ConfigureAwait(false);

            context.CommerceContext.RemoveModels(m => m is PriceSnapshotAdded || m is PriceTierAdded);

            // CARD SECOND SNAPSHOT
            adventureCard = await this._addPriceSnapshotPipeline.Run(new PriceCardSnapshotArgument(adventureCard, new PriceSnapshotComponent(date.AddDays(30))), context).ConfigureAwait(false);
            var secondSnapshot = adventureCard.Snapshots.FirstOrDefault(s => s.Id.Equals(context.CommerceContext.GetModel<PriceSnapshotAdded>()?.PriceSnapshotId, StringComparison.OrdinalIgnoreCase));

            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, secondSnapshot, new PriceTier("USD", 1, 7M)), context).ConfigureAwait(false);
            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, secondSnapshot, new PriceTier("USD", 5, 4M)), context).ConfigureAwait(false);
            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, secondSnapshot, new PriceTier("USD", 10, 3M)), context).ConfigureAwait(false);
            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, secondSnapshot, new PriceTier("CAD", 1, 6M)), context).ConfigureAwait(false);
            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, secondSnapshot, new PriceTier("CAD", 5, 3M)), context).ConfigureAwait(false);
            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, secondSnapshot, new PriceTier("CAD", 10, 2M)), context).ConfigureAwait(false);
            adventureCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureCard, secondSnapshot, new PriceTier("EUR", 1, 1M)), context).ConfigureAwait(false);

            adventureCard = await this._addPriceSnapshotTagPipeline.Run(new PriceCardSnapshotTagArgument(adventureCard, secondSnapshot, new Tag("future pricing")), context).ConfigureAwait(false);

            context.CommerceContext.RemoveModels(m => m is PriceSnapshotAdded || m is PriceTierAdded);

            readyForApprovalSnapshot?.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().ReadyForApproval));
            firstSnapshot?.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            secondSnapshot?.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));

            await this._persistEntityPipeline.Run(new PersistEntityArgument(adventureCard), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the variants card.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        private async Task CreateVariantsCard(PriceBook book, CommercePipelineExecutionContext context)
        {
            context.CommerceContext.RemoveModels(m => m is PriceSnapshotAdded || m is PriceTierAdded);

            var date = DateTimeOffset.UtcNow;

            // VARIANTS CARD
            var adventureVariantsCard = await this._addPriceCardPipeline.Run(new AddPriceCardArgument(book, "Habitat_VariantsPriceCard"), context).ConfigureAwait(false);

            // READY FOR APPROVAL SNAPSHOT
            adventureVariantsCard = await this._addPriceSnapshotPipeline.Run(new PriceCardSnapshotArgument(adventureVariantsCard, new PriceSnapshotComponent(date.AddMinutes(-10))), context).ConfigureAwait(false);
            var readyForApprovalSnapshot = adventureVariantsCard.Snapshots.FirstOrDefault(s => s.Id.Equals(context.CommerceContext.GetModel<PriceSnapshotAdded>()?.PriceSnapshotId, StringComparison.OrdinalIgnoreCase));

            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, readyForApprovalSnapshot, new PriceTier("USD", 1, 2000M)), context).ConfigureAwait(false);

            context.CommerceContext.RemoveModels(m => m is PriceSnapshotAdded || m is PriceTierAdded);

            // FIRST APPROVED SNAPSHOT
            adventureVariantsCard = await this._addPriceSnapshotPipeline.Run(new PriceCardSnapshotArgument(adventureVariantsCard, new PriceSnapshotComponent(date.AddHours(-1))), context).ConfigureAwait(false);
            var firstSnapshot = adventureVariantsCard.Snapshots.FirstOrDefault(s => s.Id.Equals(context.CommerceContext.GetModel<PriceSnapshotAdded>()?.PriceSnapshotId, StringComparison.OrdinalIgnoreCase));

            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, firstSnapshot, new PriceTier("USD", 1, 9M)), context).ConfigureAwait(false);
            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, firstSnapshot, new PriceTier("USD", 5, 6M)), context).ConfigureAwait(false);
            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, firstSnapshot, new PriceTier("USD", 10, 3M)), context).ConfigureAwait(false);
            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, firstSnapshot, new PriceTier("CAD", 1, 7M)), context).ConfigureAwait(false);
            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, firstSnapshot, new PriceTier("CAD", 5, 4M)), context).ConfigureAwait(false);
            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, firstSnapshot, new PriceTier("CAD", 10, 2M)), context).ConfigureAwait(false);
            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, firstSnapshot, new PriceTier("EUR", 1, 2M)), context).ConfigureAwait(false);

            context.CommerceContext.RemoveModels(m => m is PriceSnapshotAdded || m is PriceTierAdded);

            // DRAFT SNAPSHOT
            adventureVariantsCard = await this._addPriceSnapshotPipeline.Run(new PriceCardSnapshotArgument(adventureVariantsCard, new PriceSnapshotComponent(date)), context).ConfigureAwait(false);
            var draftSnapshot = adventureVariantsCard.Snapshots.FirstOrDefault(s => s.Id.Equals(context.CommerceContext.GetModel<PriceSnapshotAdded>()?.PriceSnapshotId, StringComparison.OrdinalIgnoreCase));

            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, draftSnapshot, new PriceTier("USD", 1, 1000M)), context).ConfigureAwait(false);

            context.CommerceContext.RemoveModels(m => m is PriceSnapshotAdded || m is PriceTierAdded);

            // SECOND APPROVED SNAPSHOT
            adventureVariantsCard = await this._addPriceSnapshotPipeline.Run(new PriceCardSnapshotArgument(adventureVariantsCard, new PriceSnapshotComponent(date.AddDays(30))), context).ConfigureAwait(false);
            var secondSnapshot = adventureVariantsCard.Snapshots.FirstOrDefault(s => s.Id.Equals(context.CommerceContext.GetModel<PriceSnapshotAdded>()?.PriceSnapshotId, StringComparison.OrdinalIgnoreCase));

            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, secondSnapshot, new PriceTier("USD", 1, 8M)), context).ConfigureAwait(false);
            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, secondSnapshot, new PriceTier("USD", 5, 4M)), context).ConfigureAwait(false);
            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, secondSnapshot, new PriceTier("USD", 10, 2M)), context).ConfigureAwait(false);
            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, secondSnapshot, new PriceTier("CAD", 1, 7M)), context).ConfigureAwait(false);
            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, secondSnapshot, new PriceTier("CAD", 5, 3M)), context).ConfigureAwait(false);
            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, secondSnapshot, new PriceTier("CAD", 10, 1M)), context).ConfigureAwait(false);
            adventureVariantsCard = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(adventureVariantsCard, secondSnapshot, new PriceTier("EUR", 1, 2M)), context).ConfigureAwait(false);

            context.CommerceContext.RemoveModels(m => m is PriceSnapshotAdded || m is PriceTierAdded);

            readyForApprovalSnapshot?.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().ReadyForApproval));
            firstSnapshot?.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            secondSnapshot?.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await this._persistEntityPipeline.Run(new PersistEntityArgument(adventureVariantsCard), context).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the tags card.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <param name="context">The context.</param>
        private async Task CreateTagsCard(PriceBook book, CommercePipelineExecutionContext context)
        {
            // TAGS CARD
            var card = await this._addPriceCardPipeline.Run(new AddPriceCardArgument(book, "Habitat_TagsPriceCard"), context).ConfigureAwait(false);

            // TAGS CARD FIRST SNAPSHOT
            card = await this._addPriceSnapshotPipeline.Run(new PriceCardSnapshotArgument(card, new PriceSnapshotComponent(DateTimeOffset.UtcNow)), context).ConfigureAwait(false);
            var firstSnapshot = card.Snapshots.FirstOrDefault();

            // TAGS CARD FIRST SNAPSHOT  TIERS
            card = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(card, firstSnapshot, new PriceTier("USD", 1, 250M)), context).ConfigureAwait(false);
            card = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(card, firstSnapshot, new PriceTier("USD", 5, 200M)), context).ConfigureAwait(false);
            card = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(card, firstSnapshot, new PriceTier("CAD", 1, 251M)), context).ConfigureAwait(false);
            card = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(card, firstSnapshot, new PriceTier("CAD", 5, 201M)), context).ConfigureAwait(false);

            // TAGS CARD FIRST SNAPSHOT TAGS
            card = await this._addPriceSnapshotTagPipeline.Run(new PriceCardSnapshotTagArgument(card, firstSnapshot, new Tag("Habitat")), context).ConfigureAwait(false);
            card = await this._addPriceSnapshotTagPipeline.Run(new PriceCardSnapshotTagArgument(card, firstSnapshot, new Tag("Habitat 2")), context).ConfigureAwait(false);
            card = await this._addPriceSnapshotTagPipeline.Run(new PriceCardSnapshotTagArgument(card, firstSnapshot, new Tag("common")), context).ConfigureAwait(false);

            // TAGS CARD SECOND SNAPSHOT
            card = await this._addPriceSnapshotPipeline.Run(new PriceCardSnapshotArgument(card, new PriceSnapshotComponent(DateTimeOffset.UtcNow.AddSeconds(1))), context).ConfigureAwait(false);
            var secondSnapshot = card.Snapshots.FirstOrDefault(s => !s.Id.Equals(firstSnapshot?.Id, StringComparison.OrdinalIgnoreCase));

            // TAGS CARD SECOND SNAPSHOT TIERS
            card = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(card, secondSnapshot, new PriceTier("USD", 1, 150M)), context).ConfigureAwait(false);
            card = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(card, secondSnapshot, new PriceTier("USD", 5, 100M)), context).ConfigureAwait(false);
            card = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(card, secondSnapshot, new PriceTier("CAD", 1, 101M)), context).ConfigureAwait(false);
            card = await this._addPriceTierPipeline.Run(new PriceCardSnapshotTierArgument(card, secondSnapshot, new PriceTier("CAD", 5, 151M)), context).ConfigureAwait(false);

            // TAGS CARD SECOND SNAPSHOT TAGS
            card = await this._addPriceSnapshotTagPipeline.Run(new PriceCardSnapshotTagArgument(card, secondSnapshot, new Tag("Habitat variants")), context).ConfigureAwait(false);
            card = await this._addPriceSnapshotTagPipeline.Run(new PriceCardSnapshotTagArgument(card, secondSnapshot, new Tag("Habitat variants 2")), context).ConfigureAwait(false);
            card = await this._addPriceSnapshotTagPipeline.Run(new PriceCardSnapshotTagArgument(card, secondSnapshot, new Tag("common")), context).ConfigureAwait(false);

            // TAGS CARD APPROVAl COMPONENT
            card.Snapshots.ForEach(s =>
            {
                s.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            });
            await this._persistEntityPipeline.Run(new PersistEntityArgument(card), context).ConfigureAwait(false);
        }

        private async Task AssociateCatalogToBook(string bookName, string catalogName, CommercePipelineExecutionContext context)
        { 
            // To persist entities conventionally and to prevent any race conditions, create a separate CommercePipelineExecutionContext object and CommerceContext object.
            var pipelineExecutionContext = new CommercePipelineExecutionContext(new CommerceContext(context.CommerceContext.Logger, context.CommerceContext.TelemetryClient)
            {
                GlobalEnvironment = context.CommerceContext.GlobalEnvironment,
                Environment = context.CommerceContext.Environment,
                Headers = new HeaderDictionary(context.CommerceContext.Headers.ToDictionary(x => x.Key, y => y.Value)) // Clone current context headers by shallow copy.
            }.PipelineContextOptions, context.CommerceContext.Logger);

            // To persist entities conventionally, remove policy keys in the newly created CommerceContext object.
            pipelineExecutionContext.CommerceContext.RemoveHeader(CoreConstants.PolicyKeys);

            var arg = new CatalogAndBookArgument(bookName, catalogName);
            await _associateCatalogToBookPipeline.Run(arg, context).ConfigureAwait(false);
        }
    }
}

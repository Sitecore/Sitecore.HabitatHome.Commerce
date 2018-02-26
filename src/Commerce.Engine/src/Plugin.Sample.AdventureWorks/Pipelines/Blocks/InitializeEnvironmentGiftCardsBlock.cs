// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeEnvironmentGiftCardsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.AdventureWorks
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Entitlements;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.Plugin.GiftCards;

    /// <summary>
    /// Defines a block which adds a set of Test GiftCards during the environment initialization.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{System.String, System.String,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(AwConstants.Pipelines.Blocks.InitializeEnvironmentGiftCardsBlock)]
    public class InitializeEnvironmentGiftCardsBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeEnvironmentGiftCardsBlock"/> class.
        /// </summary>
        /// <param name="persistEntityPipeline">The persist entity pipeline.</param>
        public InitializeEnvironmentGiftCardsBlock(
            IPersistEntityPipeline persistEntityPipeline)
        {
            this._persistEntityPipeline = persistEntityPipeline;
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
            var artifactSet = "GiftCards.TestGiftCards-1.0";

            // Check if this environment has subscribed to this Artifact Set
            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
            {
                return arg;
            }

            context.Logger.LogInformation($"{this.Name}.InitializingArtifactSet: ArtifactSet={artifactSet}");

            // Add stock gift cards for testing
            await this._persistEntityPipeline.Run(
                             new PersistEntityArgument(
                                 new GiftCard
                                     {
                                        Id = $"{CommerceEntity.IdPrefix<GiftCard>()}GC1000000",
                                        Name = "Test Gift Card ($1,000,000)",
                                        Balance = new Money("USD", 1000000M),
                                        ActivationDate = DateTimeOffset.UtcNow,
                                        Customer = new EntityReference { EntityTarget = "DefaultCustomer" },
                                        OriginalAmount = new Money("USD", 1000000M),
                                        GiftCardCode = "GC1000000",
                                        Components = new List<Component>
                                        {
                                                new ListMembershipsComponent { Memberships = new List<string> { CommerceEntity.ListName<Entitlement>(), CommerceEntity.ListName<GiftCard>() } }
                                        }
                             }),
                context);

            await this._persistEntityPipeline.Run(
                new PersistEntityArgument(
                    new EntityIndex
                        {
                            Id = $"{EntityIndex.IndexPrefix<GiftCard>("Id")}GC1000000",
                            IndexKey = "GC1000000",
                            EntityId = $"{CommerceEntity.IdPrefix<GiftCard>()}GC1000000"
                        }), 
               context);

            await this._persistEntityPipeline.Run(
                new PersistEntityArgument(
                    new GiftCard
                        {
                            Id = $"{CommerceEntity.IdPrefix<GiftCard>()}GC100",
                            Name = "Test Gift Card ($100)",
                            Balance = new Money("USD", 100M),
                            ActivationDate = DateTimeOffset.UtcNow,
                            Customer = new EntityReference { EntityTarget = "DefaultCustomer" },
                            OriginalAmount = new Money("USD", 100M),
                            GiftCardCode = "GC100",
                            Components = new List<Component>
                                    {
                                            new ListMembershipsComponent { Memberships = new List<string> { CommerceEntity.ListName<Entitlement>(), CommerceEntity.ListName<GiftCard>() } }
                                    }
                        }), 
                context);

            await this._persistEntityPipeline.Run(
                new PersistEntityArgument(
                    new EntityIndex
                        {
                            Id = $"{EntityIndex.IndexPrefix<GiftCard>("Id")}GC100",
                            IndexKey = "GC100",
                            EntityId = $"{CommerceEntity.IdPrefix<GiftCard>()}GC100"
                        }), 
               context);

            return arg;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeEnvironmentShopsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.AdventureWorks
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.Shops;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which bootstraps shops for AdventureWorks Sample environment.
    /// </summary>
    [PipelineDisplayName(AwConstants.Pipelines.Blocks.InitializeEnvironmentShopsBlock)]
    public class InitializeEnvironmentShopsBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeEnvironmentShopsBlock"/> class.
        /// </summary>
        /// <param name="persistEntityPipeline">
        /// The find entity pipeline.
        /// </param>
        public InitializeEnvironmentShopsBlock(IPersistEntityPipeline persistEntityPipeline)
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
            var artifactSet = "Environment.Shops-1.0";

            //// Check if this environment has subscribed to this Artifact Set
            if (!context.GetPolicy<EnvironmentInitializationPolicy>()
                .InitialArtifactSets.Contains(artifactSet))
            {
                return arg;
            }

            context.Logger.LogInformation($"{this.Name}.InitializingArtifactSet: ArtifactSet={artifactSet}");

            // Default Shop Entity
            await this._persistEntityPipeline.Run(
                new PersistEntityArgument(
                    new Shop
                    {
                        Id = $"{CommerceEntity.IdPrefix<Shop>()}Storefront",
                        Name = "Storefront",
                        FriendlyId = "Storefront",
                        DisplayName = "Storefront",
                        Components = new List<Component>
                        {
                            new ListMembershipsComponent { Memberships = new List<string> { CommerceEntity.ListName<Shop>() } }
                        }
                    }),
                context);

            await this._persistEntityPipeline.Run(
                new PersistEntityArgument(
                    new Shop
                    {
                        Id = $"{CommerceEntity.IdPrefix<Shop>()}AwShopCanada",
                        Name = "AwShopCanada",
                        FriendlyId = "AwShopCanada",
                        DisplayName = "Adventure Works Canada",
                        Components = new List<Component>
                        {
                            new ListMembershipsComponent { Memberships = new List<string> { CommerceEntity.ListName<Shop>() } }
                        }
                    }),
                context);

            await this._persistEntityPipeline.Run(
                new PersistEntityArgument(
                    new Shop
                    {
                        Id = $"{CommerceEntity.IdPrefix<Shop>()}AwShopUsa",
                        Name = "AwShopUsa",
                        FriendlyId = "AwShopUsa",
                        DisplayName = "Adventure Works USA",
                        Components = new List<Component>
                        {
                            new ListMembershipsComponent { Memberships = new List<string> { CommerceEntity.ListName<Shop>() } }
                        }
                    }),
                context);

            await this._persistEntityPipeline.Run(
                new PersistEntityArgument(
                   new Shop
                   {
                       Id = $"{CommerceEntity.IdPrefix<Shop>()}AwShopGermany",
                       Name = "AwShopGermany",
                       DisplayName = "Adventure Works Germany",
                       Components = new List<Component>
                       {
                           new ListMembershipsComponent { Memberships = new List<string> { CommerceEntity.ListName<Shop>() } }
                       }
                   }),
                   context);

            return arg;
        }
    }
}

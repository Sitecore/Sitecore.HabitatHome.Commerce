// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeEnvironmentRegionsBlock.cs" company="Sitecore Corporation">
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
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which bootstraps an environments Regions.
    /// </summary>
    [PipelineDisplayName(AwConstants.Pipelines.Blocks.InitializeEnvironmentRegionsBlock)]
    public class InitializeEnvironmentRegionsBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeEnvironmentRegionsBlock"/> class.
        /// </summary>
        /// <param name="persistEntityPipeline">
        /// The find entity pipeline.
        /// </param>
        public InitializeEnvironmentRegionsBlock(IPersistEntityPipeline persistEntityPipeline)
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
            var artifactSet = "Environment.Regions-1.0";

            //// Check if this environment has subscribed to this Artifact Set
            if (!context.GetPolicy<EnvironmentInitializationPolicy>()
                .InitialArtifactSets.Contains(artifactSet))
            {
                return arg;
            }

            context.Logger.LogInformation($"{this.Name}.InitializingArtifactSet: ArtifactSet={artifactSet}");

           await this._persistEntityPipeline.Run(
                new PersistEntityArgument(
                    new Country
                    {
                        Id = $"{CommerceEntity.IdPrefix<Country>()}USA",
                        Name = "United States",
                        IsoCode2 = "US",
                        IsoCode3 = "USA",
                        Components = new List<Component>
                         {
                            new ListMembershipsComponent { Memberships = new List<string> { "Countries" } }
                         },
                        AddressFormat = "1"
                    }), 
                context);

            await this._persistEntityPipeline.Run(
                new PersistEntityArgument(
                    new Country
                    {
                        Id = $"{CommerceEntity.IdPrefix<Country>()}CAN",
                        Name = "Canada",
                        IsoCode2 = "CA",
                        IsoCode3 = "CAN",
                        Components = new List<Component>
                         {
                            new ListMembershipsComponent { Memberships = new List<string> { "Countries" } }
                         },
                        AddressFormat = "1"
                    }), 
                context);

            await this._persistEntityPipeline.Run(
                new PersistEntityArgument(
                    new Country
                    {
                        Id = $"{CommerceEntity.IdPrefix<Country>()}DNK",
                        Name = "Denmark",
                        IsoCode2 = "DK",
                        IsoCode3 = "DNK",
                        Components = new List<Component>
                         {
                            new ListMembershipsComponent { Memberships = new List<string> { "Countries" } }
                         },
                        AddressFormat = "1"
                    }),
                context);

            return arg;
        }
    }
}

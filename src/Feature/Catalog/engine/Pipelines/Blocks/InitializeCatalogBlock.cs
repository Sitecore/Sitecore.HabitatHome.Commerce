// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeCatalogBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.HabitatHome.Feature.Catalog.Engine.Pipelines.Blocks
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http.Internal;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Ensure Habitat Home catalog has been loaded.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{System.String, System.String,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(HabitatHomeConstants.Pipelines.Blocks.InitializeCatalogBlock)]
    public class InitializeCatalogBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ImportCatalogsCommand _importCatalogsCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeCatalogBlock"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        /// <param name="importCatalogsCommand">The import catalogs command.</param>
        public InitializeCatalogBlock(
            IHostingEnvironment hostingEnvironment,
            ImportCatalogsCommand importCatalogsCommand)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._importCatalogsCommand = importCatalogsCommand;
        }

        /// <summary>
        /// Executes the block.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var artifactSet = "Environment.Habitat.Catalog-1.0";

            // Check if this environment has subscribed to this Artifact Set
            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
            {
                return arg;
            }

            // Similar LocalizationEntity entities are imported from a zip archive file and from creating it automatically, therefore skip LocalizeEntityBlock in IPrepPersistEntityPipeline to prevent SQL constraint violation.
            context.CommerceContext.AddPolicyKeys(new[]
            {
                "IgnoreLocalizeEntity"
            });

            await ImportCatalogAsync(context).ConfigureAwait(false);

            // Remove the IgnoreLocalizeEntity, to enable localization for InitializeEnvironmentBundlesBlock
            context.CommerceContext.RemovePolicyKeys(new[]
            {
                "IgnoreLocalizeEntity"
            });

            return arg;
        }


        /// <summary>
        /// Import catalog from file.
        /// </summary>
        /// <param name="context">The context to execute <see cref="ImportCatalogsCommand"/>.</param>
        /// <returns></returns>
        protected virtual async Task ImportCatalogAsync(CommercePipelineExecutionContext context)
        {
            using (var stream = new FileStream(GetPath("Habitat.zip"), FileMode.Open, FileAccess.Read))
            {
                var file = new FormFile(stream, 0, stream.Length, stream.Name, stream.Name);

                await _importCatalogsCommand.Process(context.CommerceContext, file, CatalogConstants.Replace, -1, 10).ConfigureAwait(false);
            }
        }

        private string GetPath(string fileName)
        {
            return Path.Combine(this._hostingEnvironment.WebRootPath, "data", "Catalogs", fileName);
        }
    }
}
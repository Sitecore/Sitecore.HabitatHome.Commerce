// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeCatalogBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Habitat
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http.Internal;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Ensure Habitat catalog has been loaded.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{System.String, System.String,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(HabitatConstants.Pipelines.Blocks.InitializeCatalogBlock)]
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

            using (var stream = new FileStream(GetPath("Habitat.zip"), FileMode.Open, FileAccess.Read))
            {
                var file = new FormFile(stream, 0, stream.Length, stream.Name, stream.Name);

                await this._importCatalogsCommand.Process(context.CommerceContext, file, CatalogConstants.ImportMode.Replace, 10);
            }

            return arg;
        }

        private string GetPath(string fileName)
        {
            return Path.Combine(this._hostingEnvironment.WebRootPath, "data", "Catalogs", fileName);
        }
    }
}

namespace Plugin.Demo.HabitatHome
{
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.Plugin.Inventory;
    using System.IO;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.AspNetCore.Hosting;
                                                   
    [PipelineDisplayName(HabitatHomeConstants.Pipelines.Blocks.InitializeCatalogBlock)]
    public class InitializeInventoryBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeCatalogBlock"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        /// <param name="importInventorySetsCommand">The import catalog command.</param>
        public InitializeInventoryBlock(
            IHostingEnvironment hostingEnvironment,
            ImportInventorySetsCommand importInventorySetsCommand)
        {
            this.HostingEnvironment = hostingEnvironment;
            this.ImportInventorySetsCommand = importInventorySetsCommand;
        }

        /// <summary>
        /// Gets the <see cref="IHostingEnvironment"/> implementation.
        /// </summary>
        protected IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// Gets the <see cref="ImportInventorySetsCommand"/> implementation.
        /// </summary>
        protected ImportInventorySetsCommand ImportInventorySetsCommand { get; }

        /// <summary>
        /// Executes the block.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var artifactSet = "Environment.HabitatHome.Catalog-1.0";

            // Check if this environment has subscribed to this Artifact Set
            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
            {
                return arg;
            }

            using (var stream = new FileStream(this.GetPath("HabitatHome_Inventory.zip"), FileMode.Open, FileAccess.Read))
            {
                var file = new FormFile(stream, 0, stream.Length, stream.Name, stream.Name);
                await this.ImportInventorySetsCommand.Process(context.CommerceContext, file, CatalogConstants.ImportMode.Replace, 10);
            }

            return arg;
        }

        private string GetPath(string fileName)
        {
            return Path.Combine(this.HostingEnvironment.WebRootPath, "data", "Catalogs", fileName);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureOpsServiceApiBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Microsoft.AspNetCore.OData.Builder;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using System.Threading.Tasks;

    /// <summary>
    /// Defines a block which configures the OData model for the plugin
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.Blocks.ConfigureOpsServiceApiBlock)]
    public class ConfigureOpsServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="modelBuilder">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The <see cref="ODataConventionModelBuilder" />.
        /// </returns>
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull($"{this.Name}: The argument can not be null");

            modelBuilder.AddEntityType(typeof(MigrateEnvironmentCommand));

            var migrateEnvironment = modelBuilder.Action("MigrateEnvironment");
            migrateEnvironment.Parameter<string>("sourceName");
            migrateEnvironment.Parameter<string>("newName");
            migrateEnvironment.Parameter<string>("newArtifactStoreId");
            migrateEnvironment.ReturnsFromEntitySet<CommerceCommand>("Commands");

            return Task.FromResult(modelBuilder);
        }
    }
}

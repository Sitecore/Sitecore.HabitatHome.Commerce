// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMigrateEnvironmentMetadataPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Sitecore.Commerce.Core;    
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the upgrade environment metadata pipeline interface.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.IPipeline{Plugin.Sample.Upgrade.MigrateEnvironmentArgument,      
    ///         Sitecore.Commerce.Core.CommerceEnvironment,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.MigrateEnvironmentMetadata)]
    public interface IMigrateEnvironmentMetadataPipeline : IPipeline<MigrateEnvironmentArgument, CommerceEnvironment, CommercePipelineExecutionContext>
    {
    }
}

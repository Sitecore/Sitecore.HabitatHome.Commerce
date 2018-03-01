// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMigrateEnvironmentPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Sitecore.Commerce.Core;    
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the migrate environment pipeline interface.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.IPipeline{Plugin.Sample.Upgrade.MigrateEnvironmentArgument,      
    ///         System.Boolean,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.MigrateEnvironment)]
    public interface IMigrateEnvironmentPipeline : IPipeline<MigrateEnvironmentArgument, bool, CommercePipelineExecutionContext>
    {
    }
}

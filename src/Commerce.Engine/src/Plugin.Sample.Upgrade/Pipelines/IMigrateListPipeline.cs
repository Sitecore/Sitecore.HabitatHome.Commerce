// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMigrateListPipeline.cs" company="Sitecore Corporation">
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
    ///         Sitecore.Framework.Pipelines.IPipeline{Plugin.Sample.Upgrade.MigrateListArgument,      
    ///         System.Boolean,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.MigrateList)]
    public interface IMigrateListPipeline : IPipeline<MigrateListArgument, bool, CommercePipelineExecutionContext>
    {
    }
}

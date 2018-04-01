// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMigrateEntityMetadataPipeline.cs" company="Sitecore Corporation">
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
    ///         Sitecore.Framework.Pipelines.IPipeline{Sitecore.Commerce.Core.FindEntityArgument,      
    ///         Sitecore.Commerce.Core.CommerceEntity,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(UpgradeConstants.Pipelines.MigrateEntityMetadata)]
    public interface IMigrateEntityMetadataPipeline : IPipeline<FindEntityArgument, CommerceEntity, CommercePipelineExecutionContext>
    {
    }
}

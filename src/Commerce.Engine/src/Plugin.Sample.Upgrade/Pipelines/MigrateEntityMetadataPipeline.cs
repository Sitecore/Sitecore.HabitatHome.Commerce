// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrateEntityMetadataPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;  
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the migrate entity metadata pipeline.
    /// </summary>
    /// <seealso>
    ///   <cref>
    /// Sitecore.Commerce.Core.CommercePipeline{Sitecore.Commerce.Core.FindEntityArgument,
    /// Sitecore.Commerce.Core.CommerceEntity
    /// </cref>
    /// </seealso>
    /// <seealso cref="Plugin.Sample.Upgrade.IMigrateEntityMetadataPipeline" />
    public class MigrateEntityMetadataPipeline : CommercePipeline<FindEntityArgument, CommerceEntity>, IMigrateEntityMetadataPipeline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateEntityMetadataPipeline" /> class.
        /// </summary>
        /// <param name="configuration">The definition.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public MigrateEntityMetadataPipeline(IPipelineConfiguration<IMigrateEntityMetadataPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrateEnvironmentMetadataPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;  
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the upgrade environment metadata pipeline.
    /// </summary>    
    /// <seealso>
    ///   <cref>
    /// Sitecore.Commerce.Core.CommercePipeline{Plugin.Sample.Upgrade.MigrateEnvironmentArgument,
    /// Sitecore.Commerce.Core.CommerceEnvironment}
    /// </cref>
    /// </seealso>
    /// <seealso cref="Plugin.Sample.Upgrade.IMigrateEnvironmentMetadataPipeline" />
    public class MigrateEnvironmentMetadataPipeline : CommercePipeline<MigrateEnvironmentArgument, CommerceEnvironment>, IMigrateEnvironmentMetadataPipeline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateEnvironmentMetadataPipeline" /> class.
        /// </summary>
        /// <param name="configuration">The definition.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public MigrateEnvironmentMetadataPipeline(IPipelineConfiguration<IMigrateEnvironmentMetadataPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}

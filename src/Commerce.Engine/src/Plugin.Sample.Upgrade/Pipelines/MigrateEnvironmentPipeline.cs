// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrateEnvironmentPipeline .cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;  
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the migrate an environment pipeline.
    /// </summary>    
    /// <seealso>
    ///   <cref>
    ///     Sitecore.Commerce.Core.CommercePipeline{Plugin.Sample.Upgrade.MigrateEnvironmentArgument,
    ///     System.Boolean}
    /// </cref>
    /// </seealso>
    /// <seealso cref="Plugin.Sample.Upgrade.IMigrateEnvironmentPipeline" />
    public class MigrateEnvironmentPipeline : CommercePipeline<MigrateEnvironmentArgument, bool>, IMigrateEnvironmentPipeline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateEnvironmentPipeline" /> class.
        /// </summary>
        /// <param name="configuration">The definition.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public MigrateEnvironmentPipeline(IPipelineConfiguration<IMigrateEnvironmentPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}

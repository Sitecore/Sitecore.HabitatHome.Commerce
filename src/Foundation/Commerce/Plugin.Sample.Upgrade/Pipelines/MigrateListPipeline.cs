// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrateListPipeline .cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;  
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the migrate a list pipeline.
    /// </summary>    
    /// <seealso>
    ///   <cref>
    /// Sitecore.Commerce.Core.CommercePipeline{Plugin.Sample.Upgrade.MigrateListArgument,
    /// System.Boolean}
    /// </cref>
    /// </seealso>
    /// <seealso cref="Plugin.Sample.Upgrade.IMigrateListPipeline" />
    public class MigrateListPipeline : CommercePipeline<MigrateListArgument, bool>, IMigrateListPipeline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateListPipeline" /> class.
        /// </summary>
        /// <param name="configuration">The definition.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public MigrateListPipeline(IPipelineConfiguration<IMigrateListPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}

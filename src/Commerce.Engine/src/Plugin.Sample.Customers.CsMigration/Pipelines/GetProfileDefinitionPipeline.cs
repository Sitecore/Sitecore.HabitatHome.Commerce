// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetProfileDefinitionPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>
//   Defines the get profile definition pipeline.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the get profile definition pipeline.
    /// </summary>  
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Commerce.Core.CommercePipeline{System.String,    ///     
    ///         System.Collections.Generic.IEnumerable{Plugin.Sample.Customers.CsMigration.ProfileDefinition}
    ///     </cref>
    /// </seealso>
    /// <seealso cref="Plugin.Sample.Customers.CsMigration.IGetProfileDefinitionPipeline" />
    public class GetProfileDefinitionPipeline : CommercePipeline<string, IEnumerable<ProfileDefinition>>, IGetProfileDefinitionPipeline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetProfileDefinitionPipeline" /> class.
        /// </summary>
        /// <param name="configuration">The definition.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public GetProfileDefinitionPipeline(IPipelineConfiguration<IGetProfileDefinitionPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}

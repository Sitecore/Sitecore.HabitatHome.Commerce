// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SamplePipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.ProductKit.Engine.Entities;
using Sitecore.HabitatHome.Feature.ProductKit.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.ProductKit.Engine.Pipelines
{
    /// <inheritdoc />
    /// <summary>
    ///  Defines the SamplePipeline pipeline.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Commerce.Core.CommercePipeline{Sitecore.Commerce.Plugin.Sample.SampleArgument,
    ///         Sitecore.Commerce.Plugin.Sample.SampleEntity}
    ///     </cref>
    /// </seealso>
    /// <seealso cref="T:Sitecore.HabitatHome.Feature.ProductKit.Engine.Pipelines.ISamplePipeline" />
    public class SamplePipeline : CommercePipeline<SampleArgument, SampleEntity>, ISamplePipeline
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.HabitatHome.Feature.ProductKit.Engine.Pipelines.SamplePipeline" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public SamplePipeline(IPipelineConfiguration<ISamplePipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}


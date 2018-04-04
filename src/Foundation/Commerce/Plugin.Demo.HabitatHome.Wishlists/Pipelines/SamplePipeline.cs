// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SamplePipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Demo.HabitatHome.Wishlists
{
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    /// <inheritdoc />
    /// <summary>
    ///  Defines the SamplePipeline pipeline.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Commerce.Core.CommercePipeline{Plugin.Demo.HabitatHome.Wishlists.SampleArgument,
    ///         Plugin.Demo.HabitatHome.Wishlists.SampleEntity}
    ///     </cref>
    /// </seealso>
    /// <seealso cref="T:Plugin.Demo.HabitatHome.Wishlists.ISamplePipeline" />
    public class SamplePipeline : CommercePipeline<SampleArgument, SampleEntity>, ISamplePipeline
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Plugin.Demo.HabitatHome.Wishlists.SamplePipeline" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public SamplePipeline(IPipelineConfiguration<ISamplePipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}


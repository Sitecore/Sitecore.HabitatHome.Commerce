// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISamplePipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.HabitatHome.Feature.ProductKit.Engine.Entities;
using Sitecore.HabitatHome.Feature.ProductKit.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.ProductKit.Engine.Pipelines
{
    /// <summary>
    /// Defines the ISamplePipeline interface
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.IPipeline{Sitecore.Commerce.Plugin.Sample.SampleArgument,
    ///         Sitecore.Commerce.Plugin.Sample.SampleEntity, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("SamplePipeline")]
    public interface ISamplePipeline : IPipeline<SampleArgument, SampleEntity, CommercePipelineExecutionContext>
    {
    }
}

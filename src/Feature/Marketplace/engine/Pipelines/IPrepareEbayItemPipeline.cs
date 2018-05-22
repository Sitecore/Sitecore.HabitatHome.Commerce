// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISamplePipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using eBay.Service.Core.Soap;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Pipelines;

namespace Sitecore.HabitatHome.Feature.EBay.Engine.Pipelines
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
    [PipelineDisplayName("Prepare an Ebay Item from a SellableItem for Syncing to Ebay")]
    public interface IPrepareEbayItemPipeline : IPipeline<SellableItem, ItemType, CommercePipelineExecutionContext>
    {
    }
}

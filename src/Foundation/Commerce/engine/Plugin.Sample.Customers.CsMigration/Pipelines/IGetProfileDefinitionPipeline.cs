// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGetProfileDefinitionPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>
//   Defines the get profile definition pipeline interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System.Collections.Generic;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the get profile definition pipeline interface.
    /// </summary>   
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.IPipeline{System.String,      
    ///         System.Collections.Generic.IEnumerable{Plugin.Sample.Customers.CsMigration.ProfileDefinition}
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(CustomersCsConstants.Pipelines.GetProfileDefinition)]
    public interface IGetProfileDefinitionPipeline : IPipeline<string, IEnumerable<ProfileDefinition>, CommercePipelineExecutionContext>
    {
    }
}

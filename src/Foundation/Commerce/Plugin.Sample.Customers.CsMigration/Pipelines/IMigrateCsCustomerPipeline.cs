// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMigrateCsCustomerPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>
//   Defines the find customer pipeline interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System.Data;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the find customer pipeline interface.
    /// </summary>
    /// <seealso cref="Sitecore.Framework.Pipelines.IPipeline{DataRow, Customer, CommercePipelineExecutionContext}" />
    [PipelineDisplayName(CustomersCsConstants.Pipelines.MigrateCsCustomer)]
    public interface IMigrateCsCustomerPipeline : IPipeline<DataRow, Customer, CommercePipelineExecutionContext>
    {
    }
}

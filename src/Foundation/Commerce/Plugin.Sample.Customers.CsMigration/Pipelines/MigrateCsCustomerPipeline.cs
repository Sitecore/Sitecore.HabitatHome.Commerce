// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrateCsCustomerPipeline .cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>
//   Defines the find a customer pipeline.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System.Data;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the create a customer pipeline.
    /// </summary>
    /// <seealso>
    /// <cref>
    ///   Sitecore.Commerce.Core.CommercePipeline{Sitecore.Commerce.Plugin.Customers.Customer, 
    ///   Sitecore.Commerce.Plugin.Customers.Customer}
    /// </cref>
    /// </seealso> 
    /// <seealso cref="Plugin.Sample.Customers.CsMigration.IMigrateCsCustomerPipeline" />
    public class MigrateCsCustomerPipeline : CommercePipeline<DataRow, Customer>, IMigrateCsCustomerPipeline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateCsCustomerPipeline" /> class.
        /// </summary>
        /// <param name="configuration">The definition.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public MigrateCsCustomerPipeline(IPipelineConfiguration<IMigrateCsCustomerPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}

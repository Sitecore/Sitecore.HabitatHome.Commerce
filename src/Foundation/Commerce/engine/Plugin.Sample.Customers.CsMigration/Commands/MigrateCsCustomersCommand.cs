// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrateCsCustomersCommand.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>// 
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Customers;

    /// <summary>
    /// Defines a get user site terms command
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Commands.CommerceCommand" />
    public class MigrateCsCustomersCommand : CommerceCommand
    {
        /// <summary>
        /// The _get user site terms pipeline.
        /// </summary>
        private readonly IMigrateCsCustomerPipeline _migrateCustomerPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateCsCustomersCommand" /> class.
        /// </summary>
        /// <param name="migrateCustomerPipeline">The migrate customer pipeline.</param>
        /// <param name="serviceProvider">The service Provider.</param>
        public MigrateCsCustomersCommand(IMigrateCsCustomerPipeline migrateCustomerPipeline, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this._migrateCustomerPipeline = migrateCustomerPipeline;
        }

        /// <summary>
        /// Processes the specified commerce context.
        /// </summary>
        /// <param name="commerceContext">The commerce context.</param>
        /// <returns>User site terms</returns>
        public virtual async Task<IEnumerable<Customer>> Process(CommerceContext commerceContext)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var sqlContext = ConnectionHelper.GetProfilesSqlContext(commerceContext);
                var rows = await sqlContext.GetAllProfiles();
                var migratedCustomers = new List<Customer>();

                foreach (DataRow row in rows)
                {
                    try
                    {                       
                        var contextOptions = commerceContext.GetPipelineContextOptions();

                        var csCustomer = await this._migrateCustomerPipeline.Run(row, contextOptions);
                        if (csCustomer != null)
                        {
                            migratedCustomers.Add(csCustomer);
                        }
                    }
                    catch (Exception ex)
                    {
                        await commerceContext.AddMessage(
                                commerceContext.GetPolicy<KnownResultCodes>().Error,
                                "EntityNotFound",
                                new object[] { row["u_user_id"] as string, ex },
                                $"Customer {row["u_user_id"] as string} was not migrated.");
                    }
                }

                return migratedCustomers;
            }
        }
    }
}

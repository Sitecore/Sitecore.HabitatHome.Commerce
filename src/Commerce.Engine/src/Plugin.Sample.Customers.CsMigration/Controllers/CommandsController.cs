// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Sitecore.Commerce.Core;
    using System.Threading.Tasks; 

    /// <summary>
    ///  Defines the commands controller for the carts plugin.
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.CommerceController" />
    public class CommandsController : CommerceController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandsController"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="globalEnvironment">The global environment.</param>
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {
        }

        /// <summary>
        /// Migrates customers.
        /// </summary>
        /// <returns>Migrated CS Customers</returns>
        [HttpPut]
        [Route("MigrateCustomers()")]
        public async Task<IActionResult> MigrateCustomers()
        {
            if (!this.ModelState.IsValid)
            {
                return new BadRequestObjectResult(this.ModelState);
            }
           
            var command = this.Command<MigrateCsCustomersCommand>();
            await command.Process(this.CurrentContext).ConfigureAwait(continueOnCapturedContext: false);

            return new ObjectResult(command);
        }       
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Plugin.Sample
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using Sitecore.Commerce.Core;

    /// <inheritdoc />
    /// <summary>
    /// Defines a controller
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.CommerceController" />
    [Microsoft.AspNetCore.OData.EnableQuery]
    [Route("api/Sample")]
    public class SampleController : CommerceController
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Commerce.Plugin.Sample.SampleController" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="globalEnvironment">The global environment.</param>
        public SampleController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A <see cref="IActionResult"/></returns>
        [HttpGet]
        [Route("(Id={id})")]
        [Microsoft.AspNetCore.OData.EnableQuery]
        public async Task<IActionResult> Get(string id)
        {
            if (!this.ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var process = this.Command<SampleCommand>()?.Process(this.CurrentContext, id);
            if (process == null)
            {
                return new BadRequestObjectResult(this.ModelState);
            }

            var result = await process;
            if (result == null)
            {
                return this.NotFound();
            }

            return new ObjectResult(result);
        }
    }
}

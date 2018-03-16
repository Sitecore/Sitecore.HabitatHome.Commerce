// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Plugin.Demo.ImportOrders.Commands;
using Plugin.Demo.ImportOrders.Pipelines.Arguments;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Plugin.Demo.ImportOrders.Entities;

namespace Sitecore.Commerce.Plugin.Sample
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http.OData;

    using Microsoft.AspNetCore.Mvc;

    using Sitecore.Commerce.Core;

    /// <inheritdoc />
    /// <summary>
    /// Defines a controller
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.CommerceController" />
    public class CommandsController : CommerceController
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Commerce.Plugin.Sample.CommandsController" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="globalEnvironment">The global environment.</param>
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
            : base(serviceProvider, globalEnvironment)
        {
        }

        /// <summary>
        /// Samples the command.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="IActionResult"/></returns>
        [HttpPut]
        [Route("CreateOfflineOrder()")]
        public async Task<IActionResult> CreateOfflineOrder([FromBody] ODataActionParameters value)
        {
            //var id = value["Id"].ToString();
            var command = this.Command<CreateOfflineOrderCommand>();

            if(!value.ContainsKey("Order"))
                return (IActionResult)new BadRequestObjectResult((object)value);            

            var inputArgs = JsonConvert.DeserializeObject<OfflineStoreOrderArgument>(value["Order"].ToString());         

            var result = await command.Process(this.CurrentContext, inputArgs);

            return new ObjectResult(command);
        }
    }
}


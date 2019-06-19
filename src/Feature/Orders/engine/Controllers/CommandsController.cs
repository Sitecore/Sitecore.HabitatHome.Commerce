// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http.OData;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sitecore.Commerce.Core;
using Sitecore.HabitatHome.Feature.Orders.Engine.Commands;
using Sitecore.HabitatHome.Feature.Orders.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.Orders.Engine.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Defines a controller
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.CommerceController" />
    public class CommandsController : CommerceController
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.HabitatHome.Feature.Orders.Engine.Controllers.CommandsController" /> class.
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
        public async Task<string> CreateOfflineOrder([FromBody] ODataActionParameters value)
        {                                              
            var command = this.Command<CreateOfflineOrderCommand>();

            if (!value.ContainsKey("Order"))
            {
                return "Bad Request, Cannot Find Order key";
            } 

            var inputArgs = JsonConvert.DeserializeObject<OfflineStoreOrderArgument>(value["Order"].ToString());

            var storeName = Regex.Replace(inputArgs.StoreDetails.Name, "[^0-9a-zA-Z]+", "");
            inputArgs.ShopName = storeName;

            var result = await command.Process(this.CurrentContext, inputArgs).ConfigureAwait(false);

            return JsonConvert.SerializeObject(result);            
        }
    }
}


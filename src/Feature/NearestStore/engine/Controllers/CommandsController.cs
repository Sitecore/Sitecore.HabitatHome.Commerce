// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http.OData;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Sitecore.Commerce.Core;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Commands;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Models;
using Sitecore.HabitatHome.Feature.NearestStore.Engine.Pipelines.Arguments;

namespace Sitecore.HabitatHome.Feature.NearestStore.Engine.Controllers
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
        /// Initializes a new instance of the <see cref="T:Sitecore.HabitatHome.Feature.NearestStore.Engine.Controllers.CommandsController" /> class.
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
        /// <param name = "value" > The value.</param>
        /// <returns>A<see cref="IActionResult"/></returns>
        [HttpPut]
        [Route("CreateStoreInventory()")]
        public async Task<IActionResult> CreateStoreInventory([FromBody] ODataActionParameters value)
        {
            if (!value.ContainsKey("Stores") || !(value["Stores"] is JArray))
            {
                return new BadRequestObjectResult(value);
            }

            if (!value.ContainsKey("ProductsToAssociate") || !(value["ProductsToAssociate"] is JArray))
            {
                return new BadRequestObjectResult(value);
            }

            JArray jarray = (JArray)value["Stores"];
            JArray jarrayProducts = (JArray)value["ProductsToAssociate"];
          
            var storeInfos =  jarray != null ? jarray.ToObject<IEnumerable<StoreDetailsModel>>() : null;
            var productsToAssociate = jarrayProducts != null ? jarrayProducts.ToObject<IEnumerable<string>>() : null;

            string catalogName = null;

            // You need to have catalog mentioned if you are not providing a list of products to update inventory.
            if(productsToAssociate == null || string.IsNullOrEmpty(productsToAssociate.FirstOrDefault()))
            {
                if (!value.ContainsKey("Catalog"))
                {
                    return new BadRequestObjectResult(value);
                }

                catalogName = Convert.ToString(value["Catalog"]);
            }

            List<CreateStoreInventorySetArgument> args = new List<CreateStoreInventorySetArgument>();

            foreach(var store in storeInfos)
            {
                var storeName = Regex.Replace(store.StoreName, "[^0-9a-zA-Z]+", "");
                CreateStoreInventorySetArgument arg = new CreateStoreInventorySetArgument(storeName, store.StoreName, store.StoreName);

                // Default to US if no country code provided
                if(string.IsNullOrEmpty(store.Country))
                {
                    store.Country = "US";
                }

                arg.Address = store.Address;
                arg.City = store.City;
                arg.Abbreviation = store.Abbreviation;
                arg.State = store.State;
                arg.ZipCode = store.ZipCode;
                arg.Long = store.Long;
                arg.Lat = store.Lat;
                arg.StoreName = store.StoreName;
                arg.CountryCode = store.Country;

                args.Add(arg);
            }
            
            var command = this.Command<CreateStoreInventoryCommand>();
            var result = await command.Process(this.CurrentContext, args, productsToAssociate.ToList(), catalogName).ConfigureAwait(false);

            if(result == null)
            {
                return new BadRequestObjectResult(value);
            }

            return new ObjectResult(command);
        }
    }
}


// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Demo.HabitatHome.Wishlists
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http.OData;

    using Microsoft.AspNetCore.Mvc;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Carts;
    using Plugin.Demo.HabitatHome.Wishlists.Commands;

    /// <inheritdoc />
    /// <summary>
    /// Defines a controller
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.CommerceController" />
    public class CommandsController : CommerceController
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Plugin.Demo.HabitatHome.Wishlists.CommandsController" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="globalEnvironment">The global environment.</param>
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
            : base(serviceProvider, globalEnvironment)
        {
        }
      

        [HttpPut]
        [Route("AddWishListLineItem()")]
        public async Task<IActionResult> AddWishListLineItem([FromBody] ODataActionParameters value)
        {
            CommandsController commandsController = this;
            if (!commandsController.ModelState.IsValid || value == null)
                return (IActionResult)new BadRequestObjectResult(commandsController.ModelState);
            if (value.ContainsKey("wishlistId"))
            {
                object obj1 = value["wishlistId"];
                if (!string.IsNullOrEmpty(obj1 != null ? obj1.ToString() : (string)null) && value.ContainsKey("itemId"))
                {
                    object obj2 = value["itemId"];
                    if (!string.IsNullOrEmpty(obj2 != null ? obj2.ToString() : (string)null) && value.ContainsKey("quantity"))
                    {
                        object obj3 = value["quantity"];
                        if (!string.IsNullOrEmpty(obj3 != null ? obj3.ToString() : (string)null))
                        {
                            string cartId = value["wishlistId"].ToString();
                            string str = value["itemId"].ToString();
                            Decimal result;
                            if (!Decimal.TryParse(value["quantity"].ToString(), out result))
                                return (IActionResult)new BadRequestObjectResult((object)value);
                            AddWishListLineItemCommand command = commandsController.Command<AddWishListLineItemCommand>();
                            CartLineComponent line = new CartLineComponent()
                            {
                                ItemId = str,
                                Quantity = result
                            };
                            Cart cart = await command.Process(commandsController.CurrentContext, cartId, line).ConfigureAwait(false);
                            return (IActionResult)new ObjectResult((object)command);
                        }
                    }
                }
            }
            return (IActionResult)new BadRequestObjectResult((object)value);
        }
    }
}


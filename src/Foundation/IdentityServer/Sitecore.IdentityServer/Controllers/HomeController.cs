// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.IdentityServer.Controllers
{
    using System.Threading.Tasks;
    using IdentityServer4.Services;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="interaction">The interaction.</param>
        public HomeController(IIdentityServerInteractionService interaction)
        {
            this._interaction = interaction;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>A <see cref="IActionResult"/></returns>
        public IActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// About this instance.
        /// </summary>
        /// <returns>A <see cref="IActionResult"/></returns>
        public IActionResult About()
        {
            this.ViewData["Message"] = "This is the IdentityServer4 implementation to use with Sitecore Commerce.";

            return this.View();
        }

        /// <summary>
        /// Contacts this instance.
        /// </summary>
        /// <returns>A <see cref="IActionResult"/></returns>
        public IActionResult Contact()
        {
            this.ViewData["Message"] = "Sitecore Commerce.";

            return this.View();
        }

        /// <summary>
        /// Errors the specified error identifier.
        /// </summary>
        /// <param name="errorId">The error identifier.</param>
        /// <returns>A <see cref="IActionResult"/></returns>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identity server
            var message = await this._interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }

            return this.View("Error", vm);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.IdentityServer.Controllers
{
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using IdentityServer4.Contrib.Membership.Helpers;
    using IdentityServer4.Contrib.Membership.Interfaces;
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Models;

    /// <summary>
    /// Account Controller
    /// </summary>
    [SecurityHeaders]
    public class AccountController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="membershipService">The membership service.</param>
        /// <param name="interaction">The interaction.</param>
        /// <param name="clientStore">The client store.</param>
        public AccountController(
            IMembershipService membershipService,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore)
        {
            this._membershipService = membershipService;
            this._interaction = interaction;
            this._clientStore = clientStore;
        }

        /// <summary>
        /// Login the specified return URL.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>A <see cref="IActionResult"/></returns>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var context = await this._interaction.GetAuthorizationContextAsync(returnUrl);
            var vm = await this.BuildLoginViewModelAsync(returnUrl, context);
            return this.View(vm);
        }

        /// <summary>
        /// Login the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="IActionResult"/></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (this.ModelState.IsValid)
            {
                if (!model.Username.StartsWith(@"sitecore\"))
                {
                    model.Username = $@"sitecore\{model.Username}";
                }

                // validate username/password against in-memory store
                if (await this._membershipService.ValidateUser(model.Username, model.Password))
                {
                    // issue authentication cookie with subject ID and username
                    var user = await this._membershipService.GetUserAsync(model.Username);

                    await this.HttpContext.SignInAsync(user.GetSubjectId(), user.UserName);

                    // make sure the returnUrl is still valid, and if yes - redirect back to authorize endpoint
                    return this._interaction.IsValidReturnUrl(model.ReturnUrl) ? this.RedirectToLocal(model.ReturnUrl) : this.Redirect("~/");
                }

                this.ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            // something went wrong, show form with error
            var vm = await this.BuildLoginViewModelAsync(model);
            return this.View(vm);
        }

        /// <summary>
        /// Logout the specified logout identifier.
        /// </summary>
        /// <param name="logoutId">The logout identifier.</param>
        /// <returns>A <see cref="IActionResult"/></returns>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await this.HttpContext.SignOutAsync();
            this.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            var logout = await this._interaction.GetLogoutContextAsync(logoutId);
            return this.Redirect(logout?.PostLogoutRedirectUri);
        }

        /// <summary>
        /// Logout the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>>A <see cref="IActionResult"/></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            // delete authentication cookie
            await this.HttpContext.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            this.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await this._interaction.GetLogoutContextAsync(model.LogoutId);

            var vm = new LoggedOutViewModel
            {
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };

            return this.View("LoggedOut", vm);
        }

        /// <summary>
        /// Externals the login.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>>A <see cref="IActionResult"/></returns>
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            if (returnUrl != null)
            {
                returnUrl = UrlEncoder.Default.Encode(returnUrl);
            }
            returnUrl = "/account/externallogincallback?returnUrl=" + returnUrl;

            // start challenge and roundtrip the return URL
            return new ChallengeResult(
                provider,
                new AuthenticationProperties
                {
                    RedirectUri = returnUrl
                });
        }

        /// <summary>
        /// Builds the login view model asynchronous.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="LogoutViewModel"/></returns>
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl, AuthorizationRequest context)
        {
            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await this._clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;
                }
            }

            return new LoginViewModel
            {
                EnableLocalLogin = allowLocal,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
            };
        }

        /// <summary>
        /// Builds the login view model asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="LogoutViewModel"/></returns>
        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var context = await this._interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            var vm = await this.BuildLoginViewModelAsync(model.ReturnUrl, context);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        /// <summary>
        /// Redirects to local.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>A <see cref="IActionResult"/></returns>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}

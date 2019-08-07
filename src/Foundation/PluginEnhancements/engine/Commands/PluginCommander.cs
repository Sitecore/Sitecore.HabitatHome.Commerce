// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginCommander.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   Defines the ChildViewEnvironmentConnectionPaths command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.HabitatHome.Foundation.PluginEnhancements.Engine.Entities;

namespace Sitecore.HabitatHome.Foundation.PluginEnhancements.Engine.Commands
{
    /// <summary>
    /// Defines the PluginCommander command.
    /// </summary>
    public class PluginCommander : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginCommander"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public PluginCommander(IServiceProvider serviceProvider,
            CommerceCommander commerceCommander) : base(serviceProvider)
        {
            this._commerceCommander = commerceCommander;
        }

        public async Task<UserPluginOptions> CurrentUserSettings(CommerceContext commerceContext, CommerceCommander commerceCommander)
        {
            var userPluginOptionsId = $"Entity-UserPluginOptions-{commerceContext.CurrentCsrId().Replace("\\", "|")}";

            var userPluginOptions = await commerceCommander.GetEntity<UserPluginOptions>(commerceContext, userPluginOptionsId, new int?(), true).ConfigureAwait(false);
            if (!userPluginOptions.IsPersisted)
            {
                userPluginOptions.Id = userPluginOptionsId;
            }

            return userPluginOptions;
        }


    }
}
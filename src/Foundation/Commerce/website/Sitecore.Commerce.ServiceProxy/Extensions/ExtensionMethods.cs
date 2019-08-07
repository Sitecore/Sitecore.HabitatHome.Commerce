// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.ServiceProxy.Extensions
{
    using CommerceOps.Sitecore.Commerce.Core.Commands;
    using CommerceOps.Sitecore.Commerce.Engine;
    using Microsoft.OData.Client;

    /// <summary>
    /// Defines extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Does the ops command.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="query">The query.</param>
        /// <returns>A <see cref="CommerceCommandSingle"/></returns>
        public static CommerceCommandSingle DoOpsCommand(this Container container, DataServiceActionQuerySingle<CommerceCommandSingle> query)
        {
            var response = query.GetValueAsync().Result;
            var commandResponse = response;
            return commandResponse;
        }
    }
}

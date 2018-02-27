// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.ServiceProxy.Extensions
{
    using System;
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
            Console.WriteLine("     ================ OPS COMMAND =================");
            Console.WriteLine("     {query.RequestUri}");

            try
            {
                var response = query.GetValueAsync().Result;
                var commandResponse = response;
                return commandResponse;
            }
            catch (DataServiceQueryException ex)
            {
                Proxy.WriteColoredLine(ConsoleColor.Red, $"Exception {ex.InnerException.Message} on GetValue:{query.RequestUri}");
                throw;
            }
            catch (AggregateException ex)
            {
                Proxy.WriteColoredLine(ConsoleColor.Red, $"Exception {ex.InnerException.Message} on GetValue:{query.RequestUri}");
                throw;
            }
            catch (Exception ex)
            {
                Proxy.WriteColoredLine(ConsoleColor.Red, $"Unknown Exception {ex.Message}  GetValue:{query.RequestUri}");
                throw;
            }
        }
    }
}

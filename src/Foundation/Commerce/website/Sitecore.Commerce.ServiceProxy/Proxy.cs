// © 2015 Sitecore Corporation A/S. All rights reserved. Sitecore® is a registered trademark of Sitecore Corporation A/S.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.OData.Client;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Engine;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.ServiceProxy.Exceptions;
using Sitecore.Diagnostics;

namespace Sitecore.Commerce.ServiceProxy
{
    /// <summary>
    /// Defines the commerce odata proxy.
    /// </summary>
    public class Proxy
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A value</returns>
        public static T GetValue<T>(DataServiceQuerySingle<T> query)
        {
            var tries = 0;
            var maxTries = 3;

            var watch = new Stopwatch();
            watch.Start();

            while (tries < maxTries)
            {
                try
                {
                    var result = query.GetValueAsync().Result;
                    watch.Stop();
                    LogInfo($"     <.><.> GetValue<T>: {query.RequestUri}: {watch.ElapsedMilliseconds}ms");

                    return result;
                }
                catch (DataServiceQueryException ex)
                {
                    LogError($"Exception {ex.InnerException.Message} on GetValue:{ex.Response.Query}", typeof(Proxy));
                    return default(T);
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        if (e is DataServiceQueryException dataserviceQueryException)
                        {
                            switch (dataserviceQueryException.Response.StatusCode)
                            {
                                case 404:

                                    // If the item is not found, we return a null.
                                    // That is easier to code for than throwing an exception
                                    LogError($"Entity Not Found Exception (404) - Query: {dataserviceQueryException.Response.Query} - Message:{dataserviceQueryException.Message}", typeof(Proxy));
                                    return default(T);
                                default:
                                    LogError($"Query Exception - Query:{dataserviceQueryException.Response.Query} - Message:{dataserviceQueryException.Message}", typeof(Proxy));
                                    throw dataserviceQueryException;
                            }
                        }

                        LogError($"Exception {e.GetType()}: {e.Message} on GetValue:{query.RequestUri}", typeof(Proxy));
                        throw e;
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Exception {ex.GetType()}: {ex.Message} GetValue:{query.RequestUri}", typeof(Proxy));
                    throw;
                }

                tries++;
                Thread.Sleep(100);
            }

            throw new CommerceServiceQuerySingleException(query.RequestUri.ToString());
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A value</returns>
        public static T GetValue<T>(DataServiceActionQuerySingle<T> query)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var result = query.GetValueAsync().Result;
                watch.Stop();
                LogInfo($"     <.><.> GetActionValue<T>: {query.RequestUri}: {watch.ElapsedMilliseconds}ms");

                return result;
            }
            catch (DataServiceQueryException ex)
            {
                LogError($"Exception {ex.InnerException.Message} on GetValue:{query.RequestUri}", typeof(Proxy));
                throw;
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    if (e is DataServiceClientException dataserviceClientException)
                    {
                        LogError($"Client Exception {dataserviceClientException.StatusCode} - {dataserviceClientException.Message} on GetValue:{query.RequestUri}", typeof(Proxy));
                    }
                    else
                    {
                        LogError($"Exception {e.GetType()}: {e.Message} on GetValue:{query.RequestUri}", typeof(Proxy));
                    }
                }

                throw;
            }
            catch (Exception ex)
            {
                LogError($"Exception {ex.GetType()}: {ex.Message} GetValue:{query.RequestUri}", typeof(Proxy));
                throw;
            }
        }

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A collection</returns>
        public static IEnumerable<T> Execute<T>(DataServiceActionQuery<T> query)
        {
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                var result = query.ExecuteAsync().Result;
                watch.Stop();
                LogInfo($"     <.><.> ExecuteAction<T>: {query.RequestUri}: {watch.ElapsedMilliseconds}ms");

                return result;
            }
            catch (DataServiceQueryException ex)
            {
                LogError($"Exception {ex.InnerException.Message} on Execute:{query.RequestUri}", typeof(Proxy));
                throw;
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    LogError($"Exception {e.GetType()}: {e.Message} on Execute:{query.RequestUri}", typeof(Proxy));
                }

                throw;
            }
            catch (Exception ex)
            {
                LogError($"Exception {ex.GetType()}: {ex.Message} Execute:{query.RequestUri}", typeof(Proxy));
                throw;
            }
        }

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A collection</returns>
        public static IEnumerable<T> Execute<T>(DataServiceQuery<T> query)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                var result = query.ExecuteAsync().Result;
                watch.Stop();
                LogInfo($"     <.><.> Execute<T>: {query.RequestUri}: {watch.ElapsedMilliseconds}ms");

                return result;
            }
            catch (DataServiceQueryException ex)
            {
                LogError($"Exception {ex.InnerException.Message} on Execute:{query.RequestUri}", typeof(Proxy));
                throw;
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    LogError($"Exception {e.GetType()}: {e.Message} on Execute:{query.RequestUri}", typeof(Proxy));
                }

                throw;
            }
            catch (Exception ex)
            {
                LogError($"Exception {ex.GetType()}: {ex.Message} Execute:{query.RequestUri}", typeof(Proxy));
                throw;
            }
        }

        /// <summary>
        /// Sends a query expecting a CommandResponse back.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A value</returns>
        public static CommerceCommand DoCommand<T>(DataServiceActionQuerySingle<T> query)
        {
            try
            {
                var response = query.GetValueAsync().Result;
                var commandResponse = response as CommerceCommand;
                if (commandResponse == null)
                {
                    return null;
                }

                LogInfo($"     <.><.> DoCommand<T>: {query.RequestUri}");
                WritePerf(commandResponse.Models.ToList());

                if (commandResponse.ResponseCode.Equals("Ok", StringComparison.OrdinalIgnoreCase))
                {
                    return commandResponse;
                }

                foreach (var message in commandResponse.Messages.Where(m => !m.Code.Equals("Information", StringComparison.OrdinalIgnoreCase)))
                {
                    LogError($"DoCommand Failed:{message.Text}", typeof(Proxy));
                }

                return commandResponse;
            }
            catch (DataServiceQueryException ex)
            {
                LogError($"Exception {ex.InnerException.Message} on DoCommand:{query.RequestUri}", typeof(Proxy));
                throw;
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    LogError($"Exception {e.GetType()}: {e.Message} on DoCommand:{query.RequestUri}", typeof(Proxy));
                }

                throw;
            }
            catch (Exception ex)
            {
                LogError($"Exception {ex.GetType()}: {ex.Message} on DoCommand:{query.RequestUri}", typeof(Proxy));
                throw;
            }
        }

        /// <summary>
        /// Sends a query expecting a CommandResponse back.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A value</returns>
        public static CommerceOps.Sitecore.Commerce.Core.Commands.CommerceCommand DoOpsCommand<T>(DataServiceActionQuerySingle<T> query)
        {
            try
            {
                var response = query.GetValueAsync().Result;
                var commandResponse = response as CommerceOps.Sitecore.Commerce.Core.Commands.CommerceCommand;
                if (commandResponse == null)
                {
                    return null;
                }

                LogInfo($"     <.><.> DoOpsCommand<T>: {query.RequestUri}");

                if (commandResponse.ResponseCode.Equals("Ok", StringComparison.OrdinalIgnoreCase))
                {
                    return commandResponse;
                }

                foreach (var message in commandResponse.Messages.Where(m => !m.Code.Equals("Information", StringComparison.OrdinalIgnoreCase)))
                {
                    LogError($"DoOpsCommand Failed:{message.Text}", typeof(Proxy));
                }

                return commandResponse;
            }
            catch (DataServiceQueryException ex)
            {
                LogError($"Exception {ex.InnerException.Message} on DoOpsCommand:{query.RequestUri}", typeof(Proxy));
                throw;
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    LogError($"Exception {e.GetType()}: {e.Message} on DoOpsCommand:{query.RequestUri}", typeof(Proxy));
                }

                throw;
            }
            catch (Exception ex)
            {
                LogError($"Exception {ex.GetType()}: {ex.Message} on DoOpsCommand:{query.RequestUri}", typeof(Proxy));
                throw;
            }
        }

        /// <summary>
        /// Gets the entity view.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="forAction">For action.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <returns>A <see cref="Sitecore.Commerce.EntityViews.EntityView"/></returns>
        /// <exception cref="CommerceServiceQuerySingleException"></exception>
        public static EntityView GetEntityView(Container container, string entityId, string viewName, string forAction, string itemId)
        {
            var watch = new Stopwatch();
            watch.Start();

            var tries = 0;
            var maxTries = 3;
            while (tries < maxTries)
            {
                try
                {
                    var query = container.GetEntityView(entityId, viewName, forAction, itemId);
                    var entityView = query.GetValueAsync().Result;
                    watch.Stop();
                    LogInfo($"     <.><.> GetEntityView: {query.RequestUri}: {watch.ElapsedMilliseconds}ms");

                    return entityView;
                }
                catch (DataServiceQueryException ex)
                {
                    LogError($"Exception {ex.InnerException.Message} on GetEntityView:{ex.Response.Query}", typeof(Proxy));
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        if (e is DataServiceQueryException dataserviceQueryException)
                        {
                            LogError($"Query Exception - Query:{dataserviceQueryException.Response.Query} - Message:{dataserviceQueryException.Message}", typeof(Proxy));
                        }
                        else
                        {
                            LogError($"Exception {e.GetType()}: {e.Message} on GetEntityView: entityId = {entityId}, viewName = {viewName}, forAction = {forAction}, itemId = {itemId}", typeof(Proxy));
                        }
                    }

                    throw;
                }
                catch (Exception ex)
                {
                    LogError($"Exception {ex.GetType()}: {ex.Message}  GetEntityView: entityId = {entityId}, viewName = {viewName}, forAction = {forAction}, itemId = {itemId}", typeof(Proxy));
                    throw;
                }

                tries++;
                Thread.Sleep(100);
            }

            throw new CommerceServiceQuerySingleException(string.Empty);
        }

        private static void LogError(string message, Type owner)
        {
            if (Log.Enabled)
            {
                Log.Error(message, owner);
                return;
            }

            WriteColoredLine(ConsoleColor.Red, message);
        }

        private static void LogInfo(string message)
        {
            if (Log.Enabled)
            {
                return;
            }

            Console.WriteLine(message);
        }

        private static void WriteColoredLine(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = originalColor;
        }

        private static void WritePerf(IReadOnlyCollection<Model> models)
        {
            if (Log.Enabled)
            {
                return;
            }

            if (!models.OfType<ActivityPerf>().Any())
            {
                return;
            }

            Console.WriteLine("     >>-------- Performance --------");
            foreach (var activityPerf in models.OfType<ActivityPerf>())
            {
                if (activityPerf.ElapsedMs <= 0)
                {
                    continue;
                }

                switch (activityPerf.Name)
                {
                    case "GetEnvironmentCommand":

                        // Do Nothing
                        break;
                    case "ValidateContextCommand":

                        // Do Nothing
                        break;
                    default:
                        if (activityPerf.ElapsedMs < 200)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        else if (activityPerf.ElapsedMs < 500)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }

                        Console.WriteLine($"     ~Perf: {activityPerf.Name}: {activityPerf.ElapsedMs}ms");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                }
            }

            Console.WriteLine("     <<-------- Performance --------");
        }
    }
}

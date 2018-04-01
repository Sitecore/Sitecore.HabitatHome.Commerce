// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Proxy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.ServiceProxy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    using Core.Commands;

    using Microsoft.OData.Client;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Engine;
    using Sitecore.Commerce.ServiceProxy.Exceptions;

    /// <summary>
    /// Defines the proxy.
    /// </summary>
    public class Proxy
    {
        /// <summary>
        /// Writes the colored line.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="text">The text.</param>
        public static void WriteColoredLine(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = originalColor;
        }

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
                    Console.WriteLine($"     <.><.> GetValue<T>: {query.RequestUri}: {watch.ElapsedMilliseconds}ms");

                    return result;
                }
                catch (DataServiceQueryException ex)
                {
                    WriteColoredLine(ConsoleColor.Red, $"Exception {ex.InnerException.Message} on GetValue:{ex.Response.Query}");
                }
                catch (AggregateException ex)
                {
                    WriteColoredLine(ConsoleColor.Red, $"Aggregate Exception {ex.InnerException.Message}");
                    var exception = ex.InnerException as DataServiceQueryException;
                    if (exception != null)
                    {
                        var dataserviceQueryException = exception;

                        switch (dataserviceQueryException.Response.StatusCode)
                        {
                            case 404:
                                // If the item is not found, we return a null.
                                // That is easier to code for than throwing an exception
                                WriteColoredLine(ConsoleColor.Red, $"Entity Not Found Exception (404) - Query: {dataserviceQueryException.Response.Query}");
                                return default(T);
                            default:
                                WriteColoredLine(ConsoleColor.Red, $"Query Exception - Query:{dataserviceQueryException.Response.Query} - Message:{dataserviceQueryException.Message}");
                                throw dataserviceQueryException;
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteColoredLine(ConsoleColor.Red, $"Unknown Exception {ex.Message} GetValue:{query.RequestUri}");
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
                Console.WriteLine($"     <.><.> GetActionValue<T>: {query.RequestUri}: {watch.ElapsedMilliseconds}ms");

                return result;
            }
            catch (DataServiceQueryException ex)
            {
                WriteColoredLine(ConsoleColor.Red ,$"Exception {ex.InnerException.Message} on GetValue:{query.RequestUri}");
                throw;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is DataServiceQueryException)
                {
                    var queryException = ex.InnerException as DataServiceQueryException;
                    var realException = queryException.InnerException as DataServiceClientException;
                    WriteColoredLine(ConsoleColor.Red, $"Client Exception {realException.StatusCode} - {realException.Message} on GetValue:{query.RequestUri}");
                    throw realException;

                }

                WriteColoredLine(ConsoleColor.Red, $"Aggregate Exception {ex.InnerException.Message} on GetValue:{query.RequestUri}");
                throw;
            }
            catch (Exception ex)
            {
                WriteColoredLine(ConsoleColor.Red, $"Unknown Exception {ex.Message} GetValue:{query.RequestUri}");
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
                Console.WriteLine($"     <.><.> ExecuteAction<T>: {query.RequestUri}: {watch.ElapsedMilliseconds}ms");

                return result;
            }
            catch (DataServiceQueryException ex)
            {
                WriteColoredLine(ConsoleColor.Red, $"Exception {ex.InnerException.Message} on Execute:{query.RequestUri}");
                throw;
            }
            catch (AggregateException ex)
            {
                WriteColoredLine(ConsoleColor.Red, $"Exception {ex.InnerException.Message} on Execute:{query.RequestUri}");
                throw;
            }
            catch (Exception ex)
            {
                WriteColoredLine(ConsoleColor.Red, $"Unknown Exception {ex.Message} Execute:{query.RequestUri}");
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
                Console.WriteLine($"     <.><.> Execute<T>: {query.RequestUri}: {watch.ElapsedMilliseconds}ms");

                return result;
            }
            catch (DataServiceQueryException ex)
            {
                WriteColoredLine(ConsoleColor.Red, $"Exception {ex.InnerException.Message} on Execute:{query.RequestUri}");
                throw;
            }
            catch (AggregateException ex)
            {
                WriteColoredLine(ConsoleColor.Red, $"Exception {ex.InnerException.Message} on Execute:{query.RequestUri}");
                throw;
            }
            catch (Exception ex)
            {
                WriteColoredLine(ConsoleColor.Red, $"Unknown Exception {ex.Message} Execute:{query.RequestUri}");
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
            Console.WriteLine($"     ================ COMMAND =================");
            Console.WriteLine($"     {query.RequestUri}");

            try
            {
                var response = query.GetValueAsync().Result;
                var commandResponse = response as CommerceCommand;
                if (commandResponse != null)
                {
                    WritePerf(commandResponse.Models.ToList());

                    if (commandResponse.ResponseCode != "Ok")
                    {
                        WriteColoredLine(ConsoleColor.Red, $"DoCommand Failed:{commandResponse.Messages.FirstOrDefault(m => m.Code.Equals("Error") || m.Code.Equals("Warning"))?.Text}");
                    }
                }

                return commandResponse;
            }
            catch (DataServiceQueryException ex)
            {
                WriteColoredLine(ConsoleColor.Red, $"Exception {ex.InnerException.Message} on GetValue:{query.RequestUri}");
                throw;
            }
            catch (AggregateException ex)
            {
                WriteColoredLine(ConsoleColor.Red, $"Exception {ex.InnerException.Message} on GetValue:{query.RequestUri}");
                throw;
            }
            catch (Exception ex)
            {
                WriteColoredLine(ConsoleColor.Red, $"Unknown Exception {ex.Message}  GetValue:{query.RequestUri}");
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
            Console.WriteLine($"     ================ COMMAND =================");
            Console.WriteLine($"     {query.RequestUri}");

            try
            {
                var response = query.GetValueAsync().Result;
                var commandResponse = response as CommerceOps.Sitecore.Commerce.Core.Commands.CommerceCommand;
                if (commandResponse != null)
                {
                    //WritePerf(commandResponse.Models.ToList());

                    if (commandResponse.ResponseCode != "Ok")
                    {
                        WriteColoredLine(ConsoleColor.Red, $"DoCommand Failed:{commandResponse.Messages.FirstOrDefault(m => m.Code.Equals("Error") || m.Code.Equals("Warning"))?.Text}");
                    }
                }

                return commandResponse;
            }
            catch (DataServiceQueryException ex)
            {
                WriteColoredLine(ConsoleColor.Red, $"Exception {ex.InnerException.Message} on GetValue:{query.RequestUri}");
                throw;
            }
            catch (AggregateException ex)
            {
                WriteColoredLine(ConsoleColor.Red, $"Exception {ex.InnerException.Message} on GetValue:{query.RequestUri}");
                throw;
            }
            catch (Exception ex)
            {
                WriteColoredLine(ConsoleColor.Red, $"Unknown Exception {ex.Message}  GetValue:{query.RequestUri}");
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
        public static Sitecore.Commerce.EntityViews.EntityView GetEntityView(Container container, string entityId, string viewName, string forAction, string itemId)
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

                    Console.WriteLine($"     <.><.> EntityView: {entityId} ({viewName}): {watch.ElapsedMilliseconds}");
                    return entityView;
                }
                catch (DataServiceQueryException ex)
                {
                    WriteColoredLine(ConsoleColor.Red, $"Exception {ex.InnerException.Message} on GetValue:{ex.Response.Query}");
                }
                catch (AggregateException ex)
                {
                    WriteColoredLine(ConsoleColor.Red, $"Aggregate Exception {ex.InnerException.Message}");
                    var exception = ex.InnerException as DataServiceQueryException;
                    if (exception != null)
                    {
                        var dataserviceQueryException = exception;

                        switch (dataserviceQueryException.Response.StatusCode)
                        {
                            default:
                                WriteColoredLine(ConsoleColor.Red, $"Query Exception - Query:{dataserviceQueryException.Response.Query} - Message:{dataserviceQueryException.Message}");
                                throw dataserviceQueryException;
                        }
                    }
                }
                catch (Exception)
                {
                    // Console.WriteLine($"Unknown Exception {ex.Message} GetValue:{query.RequestUri}");
                }

                tries++;
                Thread.Sleep(100);
            }

            throw new CommerceServiceQuerySingleException(string.Empty);
        }

        /// <summary>
        /// Writes the performance.
        /// </summary>
        /// <param name="models">The models.</param>
        public static void WritePerf(List<Model> models)
        {
            if (models.OfType<ActivityPerf>().Any())
            {
                Console.WriteLine("     >>-------- Performance --------");
                foreach (var activityPerf in models.OfType<ActivityPerf>())
                {
                    if (activityPerf.ElapsedMs > 0)
                    {
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
                                    System.Console.ForegroundColor = ConsoleColor.Cyan;
                                }
                                else if (activityPerf.ElapsedMs < 500)
                                {
                                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                                }
                                else
                                {
                                    System.Console.ForegroundColor = ConsoleColor.Red;
                                }

                                Console.WriteLine($"     ~Perf: {activityPerf.Name}: {activityPerf.ElapsedMs}ms");
                                System.Console.ForegroundColor = ConsoleColor.Cyan;
                                break;
                        }
                    }
                }

                Console.WriteLine("     <<-------- Performance --------");
            }
        }
    }
}
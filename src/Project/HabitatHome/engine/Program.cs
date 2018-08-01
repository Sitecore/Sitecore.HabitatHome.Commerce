// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Engine
{
    using System;
    using System.IO;
    using System.Net;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    /// <summary>
    /// Defines the program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">Command line args.</param>
        public static void Main(string[] args)
        {
            try
            {
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Builds the web host.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>A <see cref="IWebHost"/></returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder
                        .SetBasePath(context.HostingEnvironment.WebRootPath)
                        .AddJsonFile("config.json", false, true)
                        .AddJsonFile($"config.{context.HostingEnvironment.EnvironmentName}.json", true, true);

                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddApplicationInsightsSettings(true);
                    }
                })
                .UseKestrel(options =>
                {
                    var configuration =
                        options.ApplicationServices.GetRequiredService<IConfiguration>();
                    options.Limits.MinResponseDataRate = null;

                    var useHttps = configuration.GetValue("AppSettings:UseHttpsInKestrel", false);
                    if (useHttps)
                    {
                        var port =
                            configuration.GetValue("AppSettings:SslPort", 5000);

                        var pfxPath =
                            configuration.GetSection("AppSettings:SslPfxPath").Value ?? string.Empty;

                        var pfxPassword =
                            configuration.GetSection("AppSettings:SslPfxPassword").Value ?? string.Empty;

                        var hostingEnvironment =
                            options.ApplicationServices.GetRequiredService<IHostingEnvironment>();

                        if (File.Exists(Path.Combine(hostingEnvironment.ContentRootPath, pfxPath)))
                        {
                            options.Listen(IPAddress.Any, port, listenOptions =>
                            {
                                listenOptions.UseHttps(pfxPath, pfxPassword);
                            });
                        }
                    }
                })
                .UseSerilog()
                .Build();
    }
}

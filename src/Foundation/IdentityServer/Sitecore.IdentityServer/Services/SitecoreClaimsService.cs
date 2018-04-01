// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SitecoreClaimsService.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.IdentityServer.Services
{
    using IdentityServer4.Services;
    using IdentityServer4.Validation;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Sitecore.IdentityServer.Configuration;

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class SitecoreClaimsService : DefaultClaimsService
    {
        private static RolesInRoles _rolesInRolesAppSetting;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitecoreClaimsService"/> class.
        /// </summary>
        /// <param name="profile">The profile service</param>
        /// <param name="logger">The logger</param>
        public SitecoreClaimsService(IProfileService profile, ILogger<DefaultClaimsService> logger) : base(profile, logger)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath($"{Directory.GetCurrentDirectory()}\\wwwroot")
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            var appSettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(appSettings);
            _rolesInRolesAppSetting = RolesInRoles.Get(appSettings);
        }

        /// <summary>
        /// Returns claims for an identity token.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="resources">The requested resources</param>
        /// <param name="request">The raw request.</param>
        /// <returns>
        /// Claims for the access token
        /// </returns>
        public override async Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, IdentityServer4.Models.Resources resources, ValidatedRequest request)
        {
            List<Claim> outputClaims = new List<Claim>();
            outputClaims.AddRange(await base.GetAccessTokenClaimsAsync(subject, resources, request));
            if (!_rolesInRolesAppSetting.Enabled)
            {
                return outputClaims;
            }

            var rolesInClaims = outputClaims.Where(c => c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value).ToList();

            var rolesToAdd = new List<string>();
            foreach (var role in rolesInClaims)
            {
               await this.GetRolesInRole(role, rolesInClaims, rolesToAdd);
            }

            foreach (var role in rolesToAdd)
            {
                outputClaims.Add(new Claim("role", role));
            }

            return outputClaims;
        }

        /// <summary>
        /// Gets the roles in role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="rolesInClaims">The roles in claims.</param>
        /// <param name="rolesToAdd">The roles to add.</param>
        /// <returns></returns>
        internal async Task GetRolesInRole(string role, List<string> rolesInClaims, List<string> rolesToAdd)
        {
            var childRoles = await this.GetRolesForRole(_rolesInRolesAppSetting.DatabaseTable, _rolesInRolesAppSetting.ApplicationName, role);
            foreach (var childRole in childRoles)
            {
                if (!rolesInClaims.Contains(childRole))
                {
                    rolesToAdd.Add(childRole); 
                }

                await this.GetRolesInRole(childRole, rolesInClaims, rolesToAdd);
            }
        }

        /// <summary>
        /// Gets the roles for role.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="memberRoleName">Name of the member role.</param>
        /// <returns>List of child roles</returns>
        internal async Task<List<string>> GetRolesForRole(string tableName, string applicationName, string memberRoleName)
        {
            using (var connection = new SqlConnection(_rolesInRolesAppSetting.ConnectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();

                command.CommandText = $"Select [MemberRoleName] from [dbo].[{tableName}] with (nolock) where [ApplicationName] = @ApplicationName AND [TargetRoleName] like @TargetRoleName";

                var listNameParameter = command.CreateParameter();
                listNameParameter.ParameterName = "@ApplicationName";
                listNameParameter.DbType = DbType.String;
                listNameParameter.Value = applicationName;
                command.Parameters.Add(listNameParameter);

                var environmentIdParameter = command.CreateParameter();
                environmentIdParameter.ParameterName = "@TargetRoleName";
                environmentIdParameter.DbType = DbType.String;
                environmentIdParameter.Value = memberRoleName;
                command.Parameters.Add(environmentIdParameter);

                var reader = await command.ExecuteReaderAsync();
                var result = new List<string>();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(reader.GetString(0));
                    }
                }

                reader.Close();
                return result;
            }
        }
    } 
}

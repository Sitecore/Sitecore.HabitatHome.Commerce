// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Microsoft.AspNetCore.Mvc;
    using Sitecore.Commerce.Core;
    using System;
    using System.Threading.Tasks;
    using System.Web.Http.OData;

    /// <summary>
    ///  Defines the commands controller for the upgrade plugin.
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.CommerceController" />
    public class CommandsController : CommerceController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandsController"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="globalEnvironment">The global environment.</param>
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {
        }

        /// <summary>
        /// Migrate environment.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 'True' if succeeded, otherwise 'false'
        /// </returns>
        [HttpPut]
        [Route("commerceops/MigrateEnvironment()")]
        public async Task<IActionResult> MigrateEnvironment([FromBody] ODataActionParameters value)
        {
            if (!this.ModelState.IsValid || value == null)
            {
                return new BadRequestObjectResult(this.ModelState);
            }

            if (!value.ContainsKey("sourceName") || string.IsNullOrEmpty(value["sourceName"]?.ToString()))
            {
                return new BadRequestObjectResult(value);
            }

            var sourceName = value["sourceName"]?.ToString();

            if (!value.ContainsKey("newName") || string.IsNullOrEmpty(value["newName"]?.ToString()))
            {
                return new BadRequestObjectResult(value);
            }

            var newName = value["newName"]?.ToString();

            var newArtifactStoreId = value["newArtifactStoreId"]?.ToString();

            Guid id;
            if (string.IsNullOrEmpty(newArtifactStoreId))
            {
                id = Guid.NewGuid();
            }
            else if (!Guid.TryParse(newArtifactStoreId, out id))
            { 
                return new BadRequestObjectResult(newArtifactStoreId);                
            }
            
            this.CurrentContext.AddModel(new ArtifactStore(id.ToString("N")));

            var command = this.Command<MigrateEnvironmentCommand>();
            await command.Process(this.CurrentContext, sourceName, newName, id);

            return new ObjectResult(command);
        }       
    }
}

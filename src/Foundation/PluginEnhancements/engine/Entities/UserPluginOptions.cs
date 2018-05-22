// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sitecore.HabitatHome.Foundation.PluginEnhancements.Engine.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   SampleEntity model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;

namespace Sitecore.HabitatHome.Foundation.PluginEnhancements.Engine.Entities
{
    /// <summary>
    /// SampleDashboardEntity model.
    /// </summary>
    public class UserPluginOptions : CommerceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserPluginOptions"/> class.
        /// </summary>
        public UserPluginOptions()
        {
            EnabledPlugins = new List<string>();
        }

        public List<string> EnabledPlugins { get; set; }

        public Task<bool> Intialize(CommerceContext commerceContext)
        {
            this.Id = $"Entity-UserPluginOptions-{commerceContext.CurrentCsrId().Replace("\\", "|")}";


            return Task.FromResult(true);
        }


        
    }
}
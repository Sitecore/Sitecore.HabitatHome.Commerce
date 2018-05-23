// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrateListArgument.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>----------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Upgrade
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;

    /// <summary>
    /// Defines the migrate list argument
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.PipelineArgument" />
    public class MigrateListArgument : PipelineArgument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrateListArgument" /> class.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        public MigrateListArgument(string listName)           
        {
            Condition.Requires(listName).IsNotNullOrEmpty("The listName can not be null or empty"); 
            this.ListName = listName;           
        }

        /// <summary>
        /// Gets or sets the name of the list.
        /// </summary>
        /// <value>
        /// The name of the list.
        /// </value>
        public string ListName { get; set; }

        /// <summary>
        /// Gets or sets the maximum count to migrate.
        /// </summary>
        /// <value>
        /// The maximum count to migrate.
        /// </value>
        public int MaxCount { get; set; }
    }
}

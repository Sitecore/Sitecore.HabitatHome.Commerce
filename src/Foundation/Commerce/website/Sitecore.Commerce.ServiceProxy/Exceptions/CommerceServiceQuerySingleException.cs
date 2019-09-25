// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommerceServiceQuerySingleException.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.ServiceProxy.Exceptions
{
    using System;

    /// <summary>
    /// Defines the commerce service query Single exception.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class CommerceServiceQuerySingleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommerceServiceQuerySingleException"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        public CommerceServiceQuerySingleException(string query)
        {
            this.Query = query;
        }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public string Query { get; set; }
    }
}

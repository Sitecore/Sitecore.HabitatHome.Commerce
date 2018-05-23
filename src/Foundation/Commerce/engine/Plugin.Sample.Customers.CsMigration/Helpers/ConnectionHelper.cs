// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionHelper.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Transactions;  
    using Sitecore.Commerce.Core;

    using Sitecore.Commerce.Core.Exceptions;

    /// <summary>
    /// A Connection Helper to access the Commerce Server Profiles system Sql context
    /// </summary>
    public class ConnectionHelper
    {
        /// <summary>
        /// Gets the profiles SQL connection policy.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A <see cref="ProfilesSqlPolicy" />
        /// </returns>     
        public static ProfilesSqlPolicy GetProfilesSqlConnectionPolicy(CommerceContext context)
        {
            if (!context.HasPolicy<ProfilesSqlPolicy>())
            {
                throw new Exception("GetProfilesSqlConnectionPolicy a ProfilesSqlPolicy to be returned for SQL Connection");
            }

            return context.GetPolicy<ProfilesSqlPolicy>();
        }

        /// <summary>
        /// The gets the SQL context in use.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The <see cref="ProfilesSqlContext" />.
        /// </returns>
        public static ProfilesSqlContext GetProfilesSqlContext(CommerceContext context)
        {
            var sqlContext = new ProfilesSqlContext(GetProfilesSqlConnectionPolicy(context).ConnectionString(context));
            var transaction = context.GetObject<Transaction>();
            
            // If we have a transaction we need to use it in the SQL Context
            if (transaction != null)
            {
                sqlContext.CurrentTransaction = transaction;
            }

            return sqlContext;
        }       

        /// <summary>
        /// Evaluates the SQL exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        public static void EvaluateSqlException(SqlException ex, string message)
        {
            switch (ex.Number)
            {
                case 1105:  // Could not allocate space in Entity Store
                    throw new EntityStoreFullException(message, ex);
                case 4060:  // Could not connect to Entity Store
                    throw new EntityStoreConnectException(message, ex);
                case 1205:  // Deadlock Victim
                    throw new EntityPersistException(message, ex);
                case 2627:  // Insert of Duplicate Entity
                    throw new EntityDuplicateException(message, ex);
                case -2:  // Timeout in accessing Entity Store
                    throw new EntityStoreTimeoutException(message, ex);
                default:
                    throw new EntityPersistException(message, ex);
            }
        }
    }
}

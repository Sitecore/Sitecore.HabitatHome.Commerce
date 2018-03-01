// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfilesSqlContext.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Sitecore.Commerce.Plugin.Customers;

    /// <summary>
    /// An SQL context to access the Commerce Server Profiles system using stored procedures
    /// </summary>
    /// <seealso cref="System.Transactions.IEnlistmentNotification" />
    [ExcludeFromCodeCoverage]
    public class ProfilesSqlContext : IEnlistmentNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilesSqlContext"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ProfilesSqlContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <value>
        /// The transaction.
        /// </value>
        public Transaction CurrentTransaction { get; internal set; }
        
        /// <summary>
        /// Notifies an enlisted object that a transaction is being committed.
        /// </summary>
        /// <param name="enlistment">An <see cref="T:System.Transactions.Enlistment" /> object used to send a response to the transaction manager.</param>
        public void Commit(Enlistment enlistment)
        {
            enlistment.Done();
        }

        /// <summary>
        /// Notifies an enlisted object that the status of a transaction is in doubt.
        /// </summary>
        /// <param name="enlistment">An <see cref="T:System.Transactions.Enlistment" /> object used to send a response to the transaction manager.</param>
        public void InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }

        /// <summary>
        /// Notifies an enlisted object that a transaction is being prepared for commitment.
        /// </summary>
        /// <param name="preparingEnlistment">A <see cref="T:System.Transactions.PreparingEnlistment" /> object used to send a response to the transaction manager.</param>
        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Done();
        }

        /// <summary>
        /// Notifies an enlisted object that a transaction is being rolled back (aborted).
        /// </summary>
        /// <param name="enlistment">A <see cref="T:System.Transactions.Enlistment" /> object used to send a response to the transaction manager.</param>
        public void Rollback(Enlistment enlistment)
        {
            enlistment.Done();
        }

        /// <summary>
        /// Gets all profiles.
        /// </summary>
        /// <returns>A list of all profile Ids and Emails</returns>
        internal async Task<DataRowCollection> GetAllProfiles()
        {
            // Hook up the SQL server context 
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM [dbo].[UserObject]";
                command.CommandType = CommandType.Text;               

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataSet ds = new DataSet();
                adapter.Fill(ds);

                connection.Close();

                DataTable propertiesTable = ds.Tables[0];
                return propertiesTable.Rows;
            }
        }

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <param name="addressId">The address identifier.</param>
        /// <returns></returns>
        internal async Task<DataRow> GetAddress(string addressId)
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM [dbo].[Addresses] where u_address_Id = @AddressId";
                command.CommandType = CommandType.Text;

                var address = command.CreateParameter();
                address.ParameterName = "@AddressId";
                address.DbType = DbType.String;
                address.Value = addressId;
                command.Parameters.Add(address);              

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataSet ds = new DataSet();
                adapter.Fill(ds);

                connection.Close();

                DataTable propertiesTable = ds.Tables[0];
                return propertiesTable.Rows[0];
            }
        }

        /// <summary>
        /// Gets all profile definitions.
        /// </summary>
        /// <returns>A list of <see cref="ProfileDefinition"/></returns>
        internal async Task<List<ProfileDefinition>> GetAllProfileDefinitions()
        {
            var definitions = await this.GetProfileInfo();

            var result = new List<ProfileDefinition>();
            foreach (var definition in definitions)
            {
                var profileDefinition = await this.GetProfileDefinition(definition);
                if (profileDefinition != null)
                {
                    result.Add(profileDefinition);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the profile information.
        /// </summary>
        /// <returns>A list of profile information</returns>
        internal async Task<List<string>> GetProfileInfo()
        {
            var result = new List<string>();

            // Hook up the SQL server context 
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "sp_GetProfileCatalogInfo";
                command.CommandType = CommandType.StoredProcedure;

                var categoryName = command.CreateParameter();
                categoryName.ParameterName = "@u_cat_name";
                categoryName.DbType = DbType.String;
                categoryName.Value = "Profile Definitions";
                command.Parameters.Add(categoryName);

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataSet ds = new DataSet();
                adapter.Fill(ds);

                connection.Close();

                DataTable propertiesTable = ds.Tables[1];
                foreach (DataRow row in propertiesTable.Rows)
                {
                    result.Add(row["ClassDefName"] as string);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the profile definition.
        /// </summary>
        /// <param name="definitionName">Name of the definition.</param>
        /// <returns>A <see cref="ProfileDefinition"/></returns>
        internal async Task<ProfileDefinition> GetProfileDefinition(string definitionName)
        {
            // Hook up the SQL server context 
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "sp_GetProfileProps";
                command.CommandType = CommandType.StoredProcedure;

                var categoryName = command.CreateParameter();
                categoryName.ParameterName = "@u_cat_name";
                categoryName.DbType = DbType.String;
                categoryName.Value = "Profile Definitions";
                command.Parameters.Add(categoryName);

                var profName = command.CreateParameter();
                profName.ParameterName = "@u_prof_name";
                profName.DbType = DbType.String;
                profName.Value = definitionName;
                command.Parameters.Add(profName);

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataSet ds = new DataSet();
                adapter.Fill(ds);

                connection.Close();

                DataTable propertiesTable = ds.Tables[0];

                // definition name not found 
                if (propertiesTable.Rows.Count == 0)
                {
                    return null;
                }

                var definition = new ProfileDefinition { Name = definitionName };

                foreach (DataRow row in propertiesTable.Rows)
                {
                    if (!(row["ClassDefName"] as string).StartsWith("ParentClass_", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var profileProperty = new ProfileProperty
                    {
                        Name = row["MemberDefName"] as string,
                        DisplayName = row["DisplayName"] as string,
                        TypeName = row["TypeName"] as string,
                        ProfileReference = row["ReferenceString"] as string,
                        IsMultiValued = Convert.ToBoolean(row["IsMultiValued"]),
                        ColumnName = string.IsNullOrEmpty(row["ColumnName"] as string) ? row["ValColumnName"] as string : row["ColumnName"] as string,
                        IsHidden = false,
                        IsReadOnly = false
                    };

                    profileProperty.OriginalType = ComponentsHelper.GetPropertyOriginalType(profileProperty.TypeName);
                    definition.Properties.Add(profileProperty);
                }

                propertiesTable = ds.Tables[1];
                foreach (DataRow row in propertiesTable.Rows)
                {
                    var id = row["GroupMemDefName"] as string;
                    var property = definition.Properties.FirstOrDefault(p => p.Name.Equals(id, StringComparison.Ordinal));
                    if (property != null)
                    {
                        property.GroupName = row["GroupName"] as string;
                    }
                }

                propertiesTable = ds.Tables[2];
                foreach (DataRow row in propertiesTable.Rows)
                {
                    var id = row["MemberName"] as string;
                    var property = definition.Properties.FirstOrDefault(p => p.Name.Equals(id, StringComparison.Ordinal));
                    if (property == null)
                    {
                        continue;
                    }

                    switch (row["AttribName"] as string)
                    {
                        case "MaxLength":
                            property.MaxLength = Convert.ToInt32(row["ValStr"]);
                            break;
                        case "MinLength":
                            property.MinLength = Convert.ToInt32(row["ValStr"]);
                            break;
                    }
                }

                return definition;
            }
        }
    }
}

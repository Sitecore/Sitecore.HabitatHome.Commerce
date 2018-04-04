// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapCustomerDetailsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a block which maps customer details
    /// </summary>
    /// <seealso cref="Sitecore.Framework.Pipelines.PipelineBlock{DataRow, Customer, CommercePipelineExecutionContext}" />
    [PipelineDisplayName(CustomersCsConstants.Pipelines.Blocks.MapCustomerDetailsBlock)]
    public class MapCustomerDetailsBlock : PipelineBlock<DataRow, Customer, CommercePipelineExecutionContext>
    {
        private readonly IGetProfileDefinitionPipeline _getProfileDefinitionPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapCustomerDetailsBlock"/> class.
        /// </summary>
        /// <param name="getProfileDefinitionPipeline">The get profile definition pipeline.</param>
        public MapCustomerDetailsBlock(IGetProfileDefinitionPipeline getProfileDefinitionPipeline)
        {
            this._getProfileDefinitionPipeline = getProfileDefinitionPipeline;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The <see cref="Customer" />.
        /// </returns>
        public override async Task<Customer> Run(DataRow arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return null;
            }

            var customer = new Customer();

            try
            {
                var profileProperties = context.GetPolicy<ProfilePropertiesPolicy>();

                // get profile definition           
                var definitionResult = (await this._getProfileDefinitionPipeline.Run(string.Empty, context)).ToList();
                if (!definitionResult.Any())
                {
                    return customer;
                }

                context.CommerceContext.AddUniqueObjectByType(definitionResult);
                var profileDefinition = definitionResult.FirstOrDefault(d => d.Name.Equals(profileProperties?.UserObjectType, StringComparison.OrdinalIgnoreCase));

                // map base properties

                var definitionProperty = profileDefinition.Properties.FirstOrDefault(p => p.Name.Equals(profileProperties?.UserIdProperty, StringComparison.OrdinalIgnoreCase));
                Guid id;
                string friendlyId = string.Empty;
                if (Guid.TryParse(arg[definitionProperty.ColumnName] as string, out id))
                {
                    friendlyId = id.ToString("D");
                }
                else
                {
                    // Azure search naming restriction
                    byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(arg[definitionProperty.ColumnName] as string);
                    friendlyId = System.Convert.ToBase64String(toEncodeAsBytes).TrimEnd('=');
                }
                 
                customer.AccountNumber = friendlyId;           
                customer.FriendlyId = friendlyId;
                customer.Id = $"{CommerceEntity.IdPrefix<Customer>()}{friendlyId}";

                // map custom properties to CustomerDetailsComponent          
                customer = ComponentsHelper.CustomerDetailsGenerator(arg, customer, profileDefinition, context);

                customer.SetComponent(new ListMembershipsComponent
                {
                    Memberships = new List<string>
                    {
                        CommerceEntity.ListName<Customer>()
                    }
                });
                
                return customer;
            }
            catch (Exception ex)
            {
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().Warning,
                    "EntityNotFound",
                    new object[] { arg["u_user_id"] as string, arg["u_email_address"] as string, ex },
                    $"Profile { arg["u_email_address"] as string } was not found.");
                return customer;
            }          
        }
    }
}

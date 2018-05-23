// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapAddressesBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which maps addresses
    /// </summary>
    /// <seealso cref="Sitecore.Framework.Pipelines.PipelineBlock{Customer, Customer, CommercePipelineExecutionContext}" />
    [PipelineDisplayName(CustomersCsConstants.Pipelines.Blocks.MapAddressesBlock)]
    public class MapAddressesBlock : PipelineBlock<Customer, Customer, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The <see cref="Customer" />.
        /// </returns>
        public override async Task<Customer> Run(Customer arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return null;
            }            

            var detailsComponent = arg.GetComponent<CustomerDetailsComponent>();
            var details = detailsComponent?.View.ChildViews.FirstOrDefault(v => v.Name.Equals("Details", StringComparison.OrdinalIgnoreCase)) as EntityView;
            if (details == null || details.Properties.Count == 0)
            {
                return arg;
            }

            // get profile definition    
            var definitionResult = context.CommerceContext.GetObject<IEnumerable<ProfileDefinition>>();
            if (definitionResult == null || !definitionResult.Any())
            {
                return arg;
            }

            var profileProperties = context.GetPolicy<ProfilePropertiesPolicy>();
            var profileDefinition = definitionResult.FirstOrDefault(d => d.Name.Equals(profileProperties?.UserObjectType, StringComparison.OrdinalIgnoreCase));
            var listProperty = profileDefinition?.Properties.FirstOrDefault(p => p.TypeName == "PROFILE" && p.ProfileReference.Equals("Profile Definitions.Address", StringComparison.Ordinal));
            var addressListProperty = details.Properties.FirstOrDefault(p => p.Name.Equals($"{listProperty?.GroupName}-{listProperty?.Name}", StringComparison.OrdinalIgnoreCase));
            var addressList = addressListProperty?.RawValue as string[];            
            if (addressList == null)
            {
                return arg;
            }

            var commerceContext = context?.CommerceContext;            

            profileDefinition = definitionResult.FirstOrDefault(d => d.Name.Equals(profileProperties?.AddressType, StringComparison.OrdinalIgnoreCase));
            foreach (var addressId in addressList)
            {
                try
                {
                    var addressComponent = await ComponentsHelper.AddressComponentGenerator(addressId.ToString(), profileDefinition, context);
                    if (addressComponent != null)
                    {
                        arg.Components.Add(addressComponent);
                    }
                }
                catch (Exception ex)
                {
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Warning,
                        "EntityNotFound",
                        new object[] { addressId, ex },
                        $"Profile address { addressId } was not found.");
                    return arg;
                }
            }

            details.Properties.Remove(addressListProperty);
            return arg;
        }
    }
}

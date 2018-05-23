// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentsHelper.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Customers.CsMigration
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Customers;
    using System.Data;

    /// <summary>
    ///  A Components Helper to generate Components for Commerce Server Profiles system
    /// </summary>
    public class ComponentsHelper
    {
        /// <summary>
        /// Customers the details generator.
        /// </summary>
        /// <param name="customerData">The customer data.</param>
        /// <param name="customer">The customer.</param>
        /// <param name="profileDefinition">The profile definition.</param>
        /// <param name="context">The context.</param>
        /// <returns> 
        /// A <see cref="Customer" />
        /// </returns>
        internal static Customer CustomerDetailsGenerator(DataRow customerData, Customer customer, ProfileDefinition profileDefinition, CommercePipelineExecutionContext context)
        {
            var profileProperties = context.GetPolicy<ProfilePropertiesPolicy>();
            var userMappingProperties = context.GetPolicy<ProfilePropertiesMappingPolicy>().UserProperties;

            var details = new EntityView { Name = "Details" };
            foreach (var property in profileDefinition?.Properties)
            {
                if (property.Name.Equals(profileProperties?.AccountNumber, StringComparison.OrdinalIgnoreCase)
                    || property.Name.Equals(profileProperties?.UserIdProperty, StringComparison.OrdinalIgnoreCase)
                    || property.Name.Equals(profileProperties?.PasswordProperty, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                TypeConverter typeConverter = TypeDescriptor.GetConverter(property.OriginalType);
                var profileValue = typeConverter.ConvertFromString(customerData[property.ColumnName] as string);

                if (property.Name.Equals(profileProperties?.AccountStatusProperty, StringComparison.OrdinalIgnoreCase))
                {
                    customer.AccountStatus = profileValue.ToString().Equals("0", StringComparison.InvariantCulture) ? context.GetPolicy<KnownCustomersStatusesPolicy>()?.InactiveAccount : context.GetPolicy<KnownCustomersStatusesPolicy>()?.RequiresApproval;
                    continue;
                }

                if (profileValue == null || string.IsNullOrEmpty(profileValue.ToString()))
                {
                    continue;
                }
               
                if (property.TypeName.Equals("PROFILE", StringComparison.OrdinalIgnoreCase))
                {
                    string[] array = profileValue.ToString().Split(';').Skip(1).ToArray();
                    details.Properties.Add(new ViewProperty { Name = $"{property.GroupName}-{property.Name}", RawValue = array });
                }
                else
                {
                    if (!userMappingProperties.ContainsKey($"{property.GroupName}.{property.Name}"))
                    {
                        continue;
                    }

                    var userProperty = userMappingProperties[$"{property.GroupName}.{property.Name}"];
                    var propertyInfo = customer.GetType().GetProperty(userProperty, BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo != null && propertyInfo.CanWrite)
                    {
                        propertyInfo.SetValue(customer, profileValue, null);
                    }
                    else
                    {

                        details.Properties.Add(new ViewProperty { Name = userProperty, RawValue = profileValue });
                    }
                }
            }

            customer.GetComponent<CustomerDetailsComponent>()?.View.ChildViews.Add(details);
            return customer;
        }

        /// <summary>
        /// Addresses the component generator.
        /// </summary>
        /// <param name="addressId">The address identifier.</param>
        /// <param name="profileDefinition">The profile definition.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A <see cref="AddressComponent" />
        /// </returns>
        internal static async Task<AddressComponent> AddressComponentGenerator(string addressId, ProfileDefinition profileDefinition, CommercePipelineExecutionContext context)
        {
            // map properties to AddressComponent
            var addressType = context.GetPolicy<ProfilePropertiesPolicy>().AddressType;
            var addressComponent = new AddressComponent { Name = addressType };
            try
            {
                var sqlContext = ConnectionHelper.GetProfilesSqlContext(context.CommerceContext);
                var address = await sqlContext.GetAddress(addressId);
                addressComponent.Id = addressId;               

                var addressProperties = context.GetPolicy<ProfilePropertiesMappingPolicy>().AddressProperties;
                var details = new EntityView { Name = "Details" };

                foreach (var property in profileDefinition.Properties)
                {
                    if (addressProperties.ContainsKey($"{property.GroupName}.{property.Name}"))
                    {
                        var rawValue = address[property.ColumnName] as string;
                        if (string.IsNullOrEmpty(rawValue) || property.Name.Equals("address_id", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        var addressProperty = addressProperties[$"{property.GroupName}.{property.Name}"];
                        var propertyInfo = addressComponent.Party.GetType().GetProperty(addressProperty, BindingFlags.Public | BindingFlags.Instance);
                        if (propertyInfo != null && propertyInfo.CanWrite)
                        {
                            propertyInfo.SetValue(addressComponent.Party, rawValue, null);
                        }
                        else
                        {
                            TypeConverter typeConverter = TypeDescriptor.GetConverter(property.OriginalType);
                            var profileValue = typeConverter.ConvertFromString(rawValue);
                            details.Properties.Add(new ViewProperty { Name = addressProperty, RawValue = profileValue });
                        }
                    }
                }

                if (details.Properties.Any())
                {
                    addressComponent.View.ChildViews.Add(details);
                }
            }
            catch (Exception ex)
            {
                await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        "EntityNotFound",
                        new object[] { addressId, ex },
                        $"Address { addressId } was not found.");
                return null;
            }

            return addressComponent;
        }        

        internal static string GetPropertyOriginalType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return string.Empty;
            }

            switch (typeName.ToLower())
            {
                case "string":
                case "password":
                case "profile":
                case "siteterm":
                    return string.Empty.GetType().FullName;
                case "bool":
                    return false.GetType().FullName;
                case "datetime":
                    return DateTime.Today.GetType().FullName;
                case "number":
                    return 0.0M.GetType().FullName;
                default:
                    return typeName;
            }
        }
    }
}

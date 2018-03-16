using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;

namespace Plugin.Demo.ImportOrders.Models
{
    public class FakeParty : Model
    {
        //
        // Summary:
        //     Gets or sets the line 2.
        public string Address2 { get; set; }
        //
        // Summary:
        //     Gets or sets the line 1.
        public string Address1 { get; set; }
        //
        // Summary:
        //     Gets or Sets the Country Code
        public string CountryCode { get; set; }
        //
        // Summary:
        //     Gets or sets the country name.
        public string Country { get; set; }
        //
        // Summary:
        //     Gets or sets the last name.
        public string LastName { get; set; }
        //
        // Summary:
        //     Gets or sets the first name.
        public string FirstName { get; set; }
        //
        // Summary:
        //     Gets or sets the postal code.
        public string ZipPostalCode { get; set; }
        //
        // Summary:
        //     Gets or sets the organization.
        public string Organization { get; set; }
        //
        // Summary:
        //     Gets or sets the state province name.
        public string State { get; set; }
        //
        // Summary:
        //     Gets or sets the email.
        public string Email { get; set; }
        //
        // Summary:
        //     Gets or sets the telephone.
        public string PhoneNumber { get; set; }
        //
        // Summary:
        //     Gets or sets the city.
        public string City { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the address.
        public string AddressName { get; set; }
        //
        // Summary:
        //     Gets or sets the party id.            
        public string ExternalId { get; set; }
        //
        // Summary:
        //     Gets or Sets the State Code
        public string StateCode { get; set; }
        //
        // Summary:
        //     Gets or sets is primary.
        public bool IsPrimary { get; set; }
    }
}

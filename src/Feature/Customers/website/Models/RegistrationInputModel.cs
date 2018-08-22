using System.ComponentModel.DataAnnotations;         
using Sitecore.Commerce.XA.Feature.Account.Models.InputModels;

namespace Sitecore.HabitatHome.Feature.Customers.Models
{
    public class RegistrationInputModel : RegistrationUserInputModel
    {
        [Display(Name = "First Name"), Required, StringLength(256, ErrorMessage = "The {0} should not exceed {1} characters.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name"), Required, StringLength(256, ErrorMessage = "The {0} should not exceed {1} characters.")]
        public string LastName { get; set; }
    }
}
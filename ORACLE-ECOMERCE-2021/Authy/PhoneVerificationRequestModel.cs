using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ORACLE_ECOMERCE_2021.Models.Authy
{
    public class PhoneVerificationRequestModel
    {
        [Required]
        //[StringLength(4, ErrorMessage = "The {0} must be at least {k2} and at max {1} characters long.", MinimumLength = 1)]
        public string CountryCode { get; set; } = "+84";

        [Required]
        //[StringLength(16, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 7)]
        public string PhoneNumber { get; set; }
    }
}
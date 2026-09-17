using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models
{
    public class Address
    {
        [Required]
        [StringLength(100)]
        public string Street { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string City { get; set; } = "";

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string State { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string Country { get; set; } = "";

        [Required]
        [RegularExpression(
            @"^\d{5}(-\d{4})?$",
            ErrorMessage = "ZIP code must be 5 digits or ZIP+4.")]
        public string ZipCode { get; set; } = "";
    }
}
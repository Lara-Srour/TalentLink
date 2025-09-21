using System.ComponentModel.DataAnnotations;

namespace TalentLink.Models
{
    public class RegisterViewModel
    {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Role { get; set; } // "Client" or "Freelancer"
        

    }
}

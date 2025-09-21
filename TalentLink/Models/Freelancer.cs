using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentLink.Models
{
    public class Freelancer
    {
            [Key]
            public int Id { get; set; }
            
            [Required]
            [MaxLength(100)]
            public string? FreelancerName { get; set; }

            [MaxLength(100)]
            public string? FreelancerProfession { get; set; }
            public string? ApplicationUserId { get; set; }
            [ForeignKey("ApplicationUserId")]
             public ApplicationUser ApplicationUser { get; set; }


    }

}

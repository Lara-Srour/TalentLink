using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentLink.Models
{
    public class JobApplication
    { 
            [Key]
            public int Id { get; set; }

            public int JobId { get; set; }
            [ForeignKey("JobId")]  
            public Job Job { get; set; }

            public int FreelancerId { get; set; }
            [ForeignKey("FreelancerId")]
            public Freelancer Freelancer { get; set; }

            public DateTime ApplicationDate { get; set; }

            public string? Message { get; set; }

           [Required(ErrorMessage = "CV is required.")]

           public string CVFilePath { get; set; } 
        }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentLink.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string? ClientName { get; set; }
        
        public string? Summary { get; set; }
        [MaxLength(100)]
        public string? Location { get; set; }
      
        ICollection<Job>? Jobs { get; set; }

        public string? ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

    }
}

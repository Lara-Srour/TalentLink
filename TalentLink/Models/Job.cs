using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TalentLink.Models
{
        public class Job
        {
            [Key]
            public int Id { get; set; }

            [Required(ErrorMessage = "Job title is required", AllowEmptyStrings = false)]
            [MaxLength(150)]
            public string? JobTitle { get; set; }

            [Required(ErrorMessage = "Description is required", AllowEmptyStrings = false)]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Location is required", AllowEmptyStrings = false)]
            public string? Location { get; set; }

            [Required]
            public int ClientId { get; set; }
            [ForeignKey("ClientId")]

            public Client? Client { get; set; }
            [Column(TypeName = "nvarchar(50)")]
            public JobType JobType { get; set; }

            public decimal? Salary { get; set; }

            public DateTime DatePosted { get; set; } = DateTime.Now;
            [Column(TypeName = "nvarchar(50)")]
            public JobStatus Status { get; set; } = JobStatus.Open;
        }

        public enum JobType
        {
            FullTime,
            PartTime,
            Contract,
            Freelance,
            Internship
        }

        public enum JobStatus
        {
            Open,
            Closed,
            Filled
        }
 
}

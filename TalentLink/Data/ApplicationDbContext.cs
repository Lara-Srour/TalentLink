using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using TalentLink.Models;

namespace TalentLink.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
            public DbSet<Client> Clients { get; set; }
            public DbSet<Freelancer> Freelancers { get; set; }
            public DbSet<JobApplication> JobApplications { get; set; }
            public DbSet<Job> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Job>()
                .Property(j => j.JobType)
                .HasConversion<string>();

            builder.Entity<Job>()
                .Property(j => j.Status)
                .HasConversion<string>();


        }



    }
}

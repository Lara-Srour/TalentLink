using System.Collections.Generic;
using TalentLink.Models;

namespace TalentLink.Models.ViewModels
{
    public class FreelancerDashboardViewModel
    {
        public string FreelancerName { get; set; } = string.Empty;
        public List<Job> RecentOpenJobs { get; set; } = new List<Job>();
    }
}

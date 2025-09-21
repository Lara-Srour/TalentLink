using System.Collections.Generic;
using TalentLink.Models;

namespace TalentLink.Models.ViewModels
{
    public class ClientDashboardViewModel
    {
        public string ClientName { get; set; } = string.Empty;
        public int JobsPostedThisMonth { get; set; }
        public int NewApplicantsThisMonth { get; set; }
        public List<Job> RecentOpenJobs { get; set; } = new List<Job>();
    }
}

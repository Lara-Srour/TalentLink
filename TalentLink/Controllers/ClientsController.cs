using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentLink.Models.ViewModels;
using TalentLink.Models;
using TalentLink.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TalentLink.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get client's name
            var client = await _context.Clients
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (client == null)
                return NotFound();

            // Jobs posted this month
            var jobsThisMonth = await _context.Jobs
                .Where(j => j.Client.ApplicationUserId == userId &&
                            j.DatePosted.Month == DateTime.Now.Month &&
                            j.DatePosted.Year == DateTime.Now.Year)
                .CountAsync();

            // New applicants for these jobs this month
            var newApplicants = await _context.JobApplications
                .Include(a => a.Job)
                .Where(a => a.Job.Client.ApplicationUserId == userId &&
                            a.ApplicationDate.Month == DateTime.Now.Month &&
                            a.ApplicationDate.Year == DateTime.Now.Year)
                .CountAsync();

            // Recent open jobs
            var recentOpenJobs = await _context.Jobs
                .Where(j => j.Client.ApplicationUserId == userId && j.Status == JobStatus.Open)
                .OrderByDescending(j => j.DatePosted)
                .Take(3)
                .ToListAsync();

            var model = new ClientDashboardViewModel
            {
                ClientName = client.ApplicationUser.FullName, 
                JobsPostedThisMonth = jobsThisMonth,
                NewApplicantsThisMonth = newApplicants,
                RecentOpenJobs = recentOpenJobs
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ClientProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (client == null) { return View("Error"); }

            return View(client);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientProfile(Client clientModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (client == null) return NotFound();

            // update fields
            client.ClientName = clientModel.ClientName;
            client.Summary = clientModel.Summary;
            client.Location = clientModel.Location;

            _context.Update(client);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(ClientProfile));
        }

        [HttpGet]
        public IActionResult JobCreate()
        {
            return View();
        }

        // POST: Jobs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JobCreate(Job job)
        {
            if (ModelState.IsValid)
            {
                // Get logged-in user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Find the client corresponding to the logged-in user
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
                if (client == null)
                {
                    return BadRequest("Logged-in client not found.");
                }

                // Assign client ID to the job
                job.ClientId = client.Id;
                job.DatePosted = DateTime.UtcNow;
                job.Status = JobStatus.Open;

                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard", "Clients");

            }
            return View(job);
        }

        private IQueryable<Job> GetFilteredJobs(string userId, string status, string search)
        {
            var jobsQuery = _context.Jobs
                .Where(j => j.Client.ApplicationUserId == userId);

            if (!string.IsNullOrEmpty(status) && status != "All" &&
                Enum.TryParse<JobStatus>(status, out var jobStatus))
            {
                jobsQuery = jobsQuery.Where(j => j.Status == jobStatus);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                jobsQuery = jobsQuery.Where(j => j.JobTitle.ToLower().Contains(searchLower) ||
                                                 j.Description.ToLower().Contains(searchLower));
            }

            return jobsQuery;
        }

        public async Task<IActionResult> ClientJobs(string status, string search)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = await GetFilteredJobs(userId, status, search).ToListAsync();

            var selectedStatus = status ?? "All";   // <--- define it
            var searchTerm = search ?? "";

            // pass to ViewBag
            ViewBag.SelectedStatus = selectedStatus;
            ViewBag.SearchTerm = searchTerm;

            ViewData["StatusList"] = new SelectList(
                new[] { "All", "Open", "Closed", "Filled" },
                 selectedStatus
            );

            return View(jobs);
        }

        public async Task<IActionResult> FilterJobs(string status = "All", string search = "")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = await GetFilteredJobs(userId, status, search).ToListAsync();
            return PartialView("_JobCards", jobs);
        }

        public async Task<IActionResult> JobDetails(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Client)
                .FirstOrDefaultAsync(j => j.Id == id);
            if (job == null)
                return NotFound();
            return View(job);

        }

        [HttpGet]
        public async Task<IActionResult> JobEdit(int id)
        {
            var job = await _context.Jobs
                .FindAsync(id);
            if (job == null)
                return NotFound();
            return View(job);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JobEdit(Job job)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Jobs.Update(job);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ClientJobs", "Clients");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Jobs.Any(e => e.Id == job.Id))
                        return NotFound();
                    else
                        throw;
                }

            }
            return View(job);
        }

        public IActionResult JobApplicants(int jobId)
        {
            var applicants = _context.JobApplications
                .Where(j => j.JobId == jobId)
                .Include(j => j.Freelancer)
                .ThenInclude(f => f.ApplicationUser)
                .OrderByDescending(j => j.ApplicationDate)
                .ToList();

            if (applicants == null)
                return NotFound();
            return View(applicants);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JobDelete(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();

            job.Status = JobStatus.Closed;
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();
            return RedirectToAction("ClientJobs", "Clients");
        }
    }

}

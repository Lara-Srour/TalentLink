using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using TalentLink.Data;
using TalentLink.Models;
using TalentLink.Models.ViewModels;

namespace TalentLink.Controllers
{
    [Authorize(Roles = "Freelancer")]
    public class FreelancersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public FreelancersController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get freelancer's name
            var freelancer = await _context.Freelancers
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (freelancer == null)
                return NotFound();

            // Recent open jobs
            var recentOpenJobs = await _context.Jobs
                .Where(j => j.Status == JobStatus.Open && (j.JobTitle.ToLower().Contains(freelancer.FreelancerProfession) ||
                                                 j.Description.ToLower().Contains(freelancer.FreelancerProfession)))
                .ToListAsync();

            var model = new FreelancerDashboardViewModel
            {
                FreelancerName = freelancer.ApplicationUser.FullName,
                RecentOpenJobs = recentOpenJobs
            };

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> FreelancerProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var freelancer = await _context.Freelancers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (freelancer == null) { return View("Error"); }

            return View(freelancer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FreelancerProfile(Freelancer freelancerModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var freelancer = await _context.Freelancers
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (freelancer == null) return NotFound();

            // update fields
            freelancer.FreelancerName = freelancerModel.FreelancerName;
            freelancer.FreelancerProfession = freelancerModel.FreelancerProfession;

            _context.Update(freelancer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(FreelancerProfile));
        }


        private IQueryable<Job> GetFilteredJobs(string userId, string type, string search)
        {
            var jobsQuery = _context.Jobs
              .Where(j => j.Status == JobStatus.Open)
              .AsQueryable();


            if (!string.IsNullOrEmpty(type) && type != "" &&
                Enum.TryParse<JobType>(type, out var jobType))
            {
                jobsQuery = jobsQuery.Where(j => j.JobType == jobType);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                jobsQuery = jobsQuery.Where(j => j.JobTitle.ToLower().Contains(searchLower) ||
                                                 j.Description.ToLower().Contains(searchLower));
            }
            return jobsQuery;
        }

        public async Task<IActionResult> JobSearch(string type, string search)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = await GetFilteredJobs(userId, type, search).ToListAsync();

            var selectedType = type ?? "";
            var searchTerm = search ?? "";

            // pass to ViewBag
            ViewBag.SelectedType = selectedType;
            ViewBag.SearchTerm = searchTerm;

            ViewData["TypeList"] = new SelectList(
                new[] { "", "FullTime", "PartTime", "Contract", "Freelance", "Internship" },
                 selectedType
            );

            return View(jobs);
        }

        public async Task<IActionResult> FilterJobs(string type = "", string search = "")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = await GetFilteredJobs(userId, type, search).ToListAsync();
            return PartialView("_JobCards", jobs);
        }

        public async Task<IActionResult> JobDetails(int id)
        {

            var job = await _context.Jobs
                .Include(j => j.Client)
                .FirstOrDefaultAsync(j => j.Id == id);
            if (job == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var freelancer = await _context.Freelancers
                .FirstOrDefaultAsync(f => f.ApplicationUserId == userId);

            bool hasApplied = false;
            if (freelancer != null)
            {
                hasApplied = await _context.JobApplications
                    .AnyAsync(a => a.JobId == id && a.FreelancerId == freelancer.Id);
            }

            ViewBag.HasApplied = hasApplied;

            return View(job);
        }

        // GET: JobApplications/Create
        public async Task <IActionResult> JobApply(int jobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var freelancer = await _context.Freelancers
                .FirstOrDefaultAsync(f => f.ApplicationUserId == userId);
            if (freelancer == null) return BadRequest("Freelancer not found.");

            // Check if already applied
            var alreadyApplied = await _context.JobApplications
                .AnyAsync(a => a.JobId == jobId && a.FreelancerId == freelancer.Id);

            if (alreadyApplied)
            {
                ViewBag.ErrorMessage = "You already applied for this job.";
                return View(new JobApplication { JobId = jobId });
            }

            return View(new JobApplication { JobId = jobId });
        }
    

        // POST: JobApplications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JobApply(JobApplication model, IFormFile cvFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var freelancer = await _context.Freelancers
           .FirstOrDefaultAsync(f => f.ApplicationUserId == userId);
            if (freelancer == null) return BadRequest("Freelancer not found.");

            model.FreelancerId = freelancer.Id;
            model.ApplicationDate = DateTime.UtcNow;

            var jobExists = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == model.JobId);
            if (jobExists == null) return BadRequest("Job not found.");

            if (cvFile == null || cvFile.Length == 0)
            {
                ModelState.AddModelError("cvFile", "Please upload your CV.");
                return View(model); // redisplay the form with validation error
            }

            if (cvFile != null && cvFile.Length > 0)
            {
                // Get file name
                var fileName = Path.GetFileName(cvFile.FileName);

                // Ensure "wwwroot/uploads" folder exists
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Full file path
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file to server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await cvFile.CopyToAsync(stream);
                }

                // Save relative path to DB
                model.CVFilePath = "/uploads/" + fileName;
            }

            // Save application to DB
            _context.JobApplications.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("JobSearch", "Freelancers", new { id = model.JobId });
        }


        [HttpGet]
        public async Task<IActionResult> FreelancerApplications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var freelancer = await _context.Freelancers
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (freelancer == null)
                return View("Error");

            var applications = await _context.JobApplications
                .Where(j => j.FreelancerId == freelancer.Id)
                .Include(j => j.Job)
                .Include(j => j.Job.Client)
                .OrderByDescending(j => j.ApplicationDate)
                .ToListAsync();

            return View(applications);
        }

    }
}

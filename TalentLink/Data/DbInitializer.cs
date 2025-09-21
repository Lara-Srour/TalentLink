using Microsoft.AspNetCore.Identity;
using TalentLink.Models;

namespace TalentLink.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            string[] roleNames = { "Admin", "Client", "Freelancer" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string adminEmail = "admin@talentlink.com";
            string adminPassword = "Admin123"; 

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createAdminResult = await userManager.CreateAsync(newAdmin, adminPassword);
                if (createAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}

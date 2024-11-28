using EduPlanManager.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace EduPlanManager.Extensions
{
    public static class AdminUserSeeder
    {
        public static IApplicationBuilder SeedAdminUser(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var adminEmail = "admin@example.com";
                var adminPassword = "Admin@123";

                var user = userManager.FindByEmailAsync(adminEmail).Result;
                if (user == null)
                {
                    user = new User
                    {
                        UserName = adminEmail,
                        Email = adminEmail
                    };

                    var result = userManager.CreateAsync(user, adminPassword).Result;
                    if (result.Succeeded)
                    {
                        var roleExists = roleManager.RoleExistsAsync("Admin").Result;
                        if (!roleExists)
                        {
                            roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                        }

                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }
            }
            return app;
        }
    }
}

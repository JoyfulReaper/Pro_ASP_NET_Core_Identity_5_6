﻿using Microsoft.AspNetCore.Identity;

namespace IdentityApp;

public static class DashboardSeed
{
    public static void SeedUserStoreForDashboard(this WebApplication app)
    {
        SeedStore(app).GetAwaiter().GetResult();
    }

    // Seed the user store with the initial user data.
    private async static Task SeedStore (this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            IConfiguration config =
                scope.ServiceProvider.GetRequiredService<IConfiguration>();
            UserManager<IdentityUser> userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager =
                scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string roleName = config["Dashboard:Role"] ?? "Dashboard";
            string userName = config["Dashboard:User"] ?? "admin@example.com";
            string password = config["Dashboard:Password"] ?? "mysecret";

            if(!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            IdentityUser dashboardUser =
                await userManager.FindByEmailAsync(userName);
            if (dashboardUser == null)
            {
                dashboardUser = new IdentityUser
                {
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(dashboardUser);
                dashboardUser = await userManager.FindByEmailAsync(userName);
                await userManager.AddPasswordAsync(dashboardUser, password);
            }
            if (!await userManager.IsInRoleAsync(dashboardUser, roleName))
            {
                await userManager.AddToRoleAsync(dashboardUser, roleName);
            }
        }
    }
}

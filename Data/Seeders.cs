using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleArchitecture.Authentication.Enums;
using SimpleArchitecture.Authentication.Types;
using SimpleArchitecture.Common;

namespace SimpleArchitecture.Data;

public static class Seeders
{
    public static async Task SeedRoles(this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

        var roles = await roleManager.Roles.ToListAsync(cancellationToken);

        var newRoles = Enum.GetNames<Roles>().Where(role => roles.All(savedRole => savedRole.Name != role)).Select(
            roleName => new Role
            {
                Name = roleName,
            }).ToList();

        foreach (var role in newRoles)
        {
            await roleManager.CreateAsync(role);
        }
    }

    public static async Task CreateAppAdmin(this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        var admin = await userManager.FindByEmailAsync(Constants.SimpleArchitectureAdmin.Email);

        if (admin is not null)
            return;

        var newAdmin = new User
        {
            Email = Constants.SimpleArchitectureAdmin.Email,
            UserName = Constants.SimpleArchitectureAdmin.UserName,
            EmailConfirmed = true,
            DisplayName = Constants.SimpleArchitectureAdmin.DisplayName
        };

        var result = await userManager.CreateAsync(newAdmin, Constants.SimpleArchitectureAdmin.Password);

        if (result.Succeeded)
        {
            var roles = new[] { nameof(Roles.SimpleArchitectureAdmin) };

            var _ = await userManager.AddToRolesAsync(newAdmin, roles);
        }
    }

    public static async Task CreateSystemSupervisor(this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        var admin = await userManager.FindByEmailAsync(Constants.SystemAdministrator.Email);

        if (admin is not null)
            return;

        var newAdmin = new User
        {
            Email = Constants.SystemAdministrator.Email,
            UserName = Constants.SystemAdministrator.UserName,
            EmailConfirmed = true,
            DisplayName = Constants.SystemAdministrator.DisplayName
        };

        var result = await userManager.CreateAsync(newAdmin, Constants.SystemAdministrator.Password);

        if (result.Succeeded)
        {
            var roles = new[] { nameof(Roles.SystemAdministrator) };

            var _ = await userManager.AddToRolesAsync(newAdmin, roles);
        }
    }
}
using Microsoft.AspNetCore.Identity;
using TrainingSystem.Auth.Model;

namespace TrainingSystem.Auth;

public class AuthSeeder
{
    private readonly UserManager<ForumUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public AuthSeeder(UserManager<ForumUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task SeedAsync()
    {
        await AddDefaultRolesAsync();
        await AddAdminUserAsync();
    }

    private async Task AddAdminUserAsync()
    {
        var newAdminUser = new ForumUser()
        {
            UserName = "admin",
            Email = "admin@admin.com"
        };
        
        var existAdminUser = await _userManager.FindByNameAsync(newAdminUser.UserName);
        if (existAdminUser != null)
        {
            var createAdminUserResult = await _userManager.CreateAsync(newAdminUser, "admin");
            if (createAdminUserResult.Succeeded)
            {
                foreach (var role in ForumRoles.All)
                {
                    await _userManager.AddToRoleAsync(newAdminUser, role);
                }
                //await _userManager.AddToRoleAsync(newAdminUser, ForumRoles.All);
            }
        }
    }

    private async Task AddDefaultRolesAsync()
    {
        throw new NotImplementedException();
    }
}
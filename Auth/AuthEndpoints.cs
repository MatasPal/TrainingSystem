using Microsoft.AspNetCore.Identity;
using TrainingSystem.Auth.Model;
using TrainingSystem.Data;

namespace TrainingSystem.Auth;

public static class AuthEndpoints
{
    public static void AddAuthApi(this WebApplication app)
    {
        //register
        app.MapPost("api/accounts", async ( UserManager<ForumUser> userManager, RegisterUserDto dto)=>
        {
            //check user
            var user = await userManager.FindByNameAsync(dto.UserName);
            if (user != null) 
                return Results.UnprocessableEntity("Username already taken");

            var newUser = new ForumUser()
            {
                Email = dto.Email,
                UserName = dto.UserName,
            };
            
            // TODO: wrap in transaction 
            var createUserResult = await userManager.CreateAsync(newUser, dto.Password);
            if(!createUserResult.Succeeded)
                return Results.UnprocessableEntity();
            
            await userManager.AddToRoleAsync(user, ForumRoles.ForumUser);

            return Results.Created();
        });
        
        //login
        
        app.MapPost("api/login", async ( UserManager<ForumUser> userManager,JwtTokenService jwtTokenService,LoginDto dto)=>
        {
            //check user
            var user = await userManager.FindByNameAsync(dto.UserName);
            if (user == null) 
                return Results.UnprocessableEntity("Username does not exist");

            var isPassword = await userManager.CheckPasswordAsync(user, dto.Password);
            if(!isPassword)
                return Results.UnprocessableEntity("Username or password is incorrect");

            var roles = await userManager.GetRolesAsync(user);
            
            var accessToken = jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
            

            return Results.Ok(new SuccessfulLoginDto(accessToken));
        });
    }
    
    public record RegisterUserDto(string UserName, string Email, string Password);
    public record LoginDto(string UserName, string Password);
    public record SuccessfulLoginDto(string AccessToken);
}
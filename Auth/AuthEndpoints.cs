using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            
            await userManager.AddToRoleAsync(newUser, ForumRoles.ForumUser);

            return Results.Created();
        });
        
        //login
        
        app.MapPost("api/login", async ( UserManager<ForumUser> userManager,JwtTokenService jwtTokenService, HttpContext httpContext, LoginDto dto)=>
        {
            //check user
            var user = await userManager.FindByNameAsync(dto.UserName);
            if (user == null) 
                return Results.UnprocessableEntity("Username does not exist");

            var isPassword = await userManager.CheckPasswordAsync(user, dto.Password);
            if(!isPassword)
                return Results.UnprocessableEntity("Username or password is incorrect");

            var roles = await userManager.GetRolesAsync(user);
            
            var expiresAt = DateTime.UtcNow.AddDays(3);
            var accessToken = jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
            var refreshToken = jwtTokenService.CreateRefreshToken(user.Id, expiresAt);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = expiresAt,
                //Secure = false => should be true
            };
            
            httpContext.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);

            return Results.Ok(new SuccessfulLoginDto(accessToken));
        });

        app.MapPost("api/acessToken", async (UserManager<ForumUser> userManager,JwtTokenService jwtTokenService, HttpContext httpContext) =>
        {
            if (!httpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
            {
                return Results.UnprocessableEntity();
            }

            if (!jwtTokenService.TryParseRefreshToken(refreshToken, out var claims))
            {
                return Results.UnprocessableEntity();
            }

            var userId = claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.UnprocessableEntity();
            }
            
            var roles = await userManager.GetRolesAsync(user);
            
            var expiresAt = DateTime.UtcNow.AddDays(3);
            var accessToken = jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
            var newrefreshToken = jwtTokenService.CreateRefreshToken(user.Id, expiresAt);
            
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = expiresAt,
                //Secure = false => should be true
            };
            
            httpContext.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
            
            return Results.Ok(new SuccessfulLoginDto(accessToken));
        });
    }
    
    public record RegisterUserDto(string UserName, string Email, string Password);
    public record LoginDto(string UserName, string Password);
    public record SuccessfulLoginDto(string AccessToken);
}
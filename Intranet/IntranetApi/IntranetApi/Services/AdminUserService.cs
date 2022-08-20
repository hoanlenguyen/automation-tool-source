using IntranetApi.DbContext;
using IntranetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IntranetApi.Services
{
    public static class AdminUserService
    {
        public static void AddAdminUserService(this WebApplication app)
        {
            app.MapPost("auth/register", [AllowAnonymous] async (UserManager<User> userManager, UserCreateOrUpdateDto input) =>
            {
                var user = new User
                {
                    UserName = input.UserName ?? input.Email,
                    Name = input.Name,
                    Email = input.Email,
                    IsSuperAdmin =true,
                    IsFirstTimeLogin = true
                };

                var result = await userManager.CreateAsync(user, input.Password);

                if (result.Succeeded)
                    return Results.Ok();

                return Results.BadRequest();
            });

            app.MapPost("auth/login", 
                [AllowAnonymous] async (
                [FromServices] IConfiguration config,
                [FromServices] UserManager<User> userManager,
                [FromServices] ApplicationDbContext db, 
                [FromBody] UserLoginDto input) => 
            {
                var user = await userManager.FindByNameAsync(input.UserName);
                if (user is null)
                    user = await userManager.FindByEmailAsync(input.UserName);
                
                if (user is null)
                    return Results.Unauthorized();

                if (await userManager.CheckPasswordAsync(user, input.Password))
                {
                    var issuer = config["Jwt:Issuer"];
                    var audience = config["Jwt:Audience"];
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    var query = from s in db.UserRoles.AsNoTracking()
                                join sa in db.RoleClaims.AsNoTracking() on s.RoleId equals sa.RoleId
                                where s.UserId == user.Id && sa.ClaimType == Permissions.Type
                                select sa.ClaimValue;

                    var permissions = query.ToList();

                    query = from s in db.UserRoles
                            join sa in db.Roles on s.RoleId equals sa.Id
                            where s.UserId == user.Id
                            select sa.Name;

                    var roleName= query.FirstOrDefault();

                    var token = new JwtSecurityToken(
                        issuer: issuer,
                        audience: audience,
                        signingCredentials: credentials,
                        claims: claims);

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var accessToken = tokenHandler.WriteToken(token);
                    return Results.Ok(new
                    {
                        accessToken,
                        user.Email,
                        user.Name,
                        roleName,
                        permissions,
                        user.IsFirstTimeLogin
                    });
                }

                return Results.Unauthorized();
            });

            app.MapGet("auth/checkConfig", [Authorize] async (IConfiguration config, IHttpContextAccessor httpContextAccessor) =>
            {
                return Results.Ok(new
                {
                    //conn = connectionString,
                    CheckConfigEnv = config["CheckConfigEnv"],
                    serverDB = config["Mysql:Server"],
                    database = config["Mysql:Database"],
                    EMAIL_IS_SEND_SPECIFIC_MAIL = config["EMAIL_IS_SEND_SPECIFIC_MAIL"],
                    //user = config["Mysql:User"],
                    //password = config["Mysql:Password"],
                    userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    email = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email)
                });
            });

            app.MapGet("auth/getProfile", [Authorize] async (IHttpContextAccessor httpContextAccessor, UserManager<User> userManager) =>
            {
                var email = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                    return Results.Unauthorized();

                var user = await userManager.FindByEmailAsync(email);
                if (user is null)
                    return Results.Unauthorized();

                return Results.Ok(new
                {
                    Name = user.Name,
                    Email = user.Email
                });
            });

            app.MapPost("auth/changePassword", [Authorize] async ([FromServices] IHttpContextAccessor httpContextAccessor, [FromServices] UserManager<User> userManager, [FromBody] UserChangePassword input) =>
            {
                var email = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                    return Results.Unauthorized();

                var user = await userManager.FindByEmailAsync(email);
                if (user is null)
                    return Results.Unauthorized();

                if (await userManager.CheckPasswordAsync(user, input.CurrentPassword))
                {
                    var result = await userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
                    if (result.Succeeded)
                    {
                        user.IsFirstTimeLogin = false;
                        return Results.Ok();
                    }
                    return Results.BadRequest("Can not change this password!");
                }

                return Results.Unauthorized();
            });
        }
    }
}

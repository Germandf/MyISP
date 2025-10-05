using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MyISP.Identity;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/me", async (HttpContext httpContext, UserManager<ApplicationUser> userManager) =>
        {
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Results.Unauthorized();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Results.NotFound();

            return Results.Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber
            });
        })
        .WithName("GetMyInformation")
        .RequireAuthorization();
    }
}

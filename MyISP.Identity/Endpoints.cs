namespace MyISP.Identity;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/me", (HttpContext httpContext) =>
        {
            var claims = httpContext.User.Claims.Select(c => new { c.Type, c.Value });
            return Results.Ok(claims);
        })
        .WithName("GetMyClaims")
        .RequireAuthorization();
    }
}

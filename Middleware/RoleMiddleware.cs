using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class RoleMiddleware
{
    private readonly RequestDelegate _next;

    public RoleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userType = context.Session.GetString("UserType");
        if (userType == "admin")
        {
            // Przekierowanie na dashboard admina
            context.Response.Redirect("/Account/AdminDashboard");
        }
        else if (userType == "user")
        {
            // Przekierowanie na dashboard użytkownika
            context.Response.Redirect("/Account/UserDashboard");
        }
        else
        {
            // Jeśli brak roli, przekieruj do logowania
            context.Response.Redirect("/Account/Login");
        }

        await _next(context);
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("[controller]")]
public class AccountController : Controller
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] string rfc, [FromForm] string rol)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, rfc),
            new Claim(ClaimTypes.Role, rol)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return Redirect("/");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/login");
    }
}
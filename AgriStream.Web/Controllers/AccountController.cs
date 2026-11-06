using AgriStream.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriStream.Web.Controllers;

public class AccountController : Controller
{
    private readonly AgriStreamDbContext _db;

    public AccountController(AgriStreamDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // NOTE: In production use ASP.NET Identity + proper password hashing
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View();
        }

        // Demo only – replace with proper verification
        // For the practicum a simple check is acceptable if documented
        TempData["User"] = user.Username;
        TempData["Role"] = user.Role;
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        TempData.Clear();
        return RedirectToAction("Index", "Home");
    }
}

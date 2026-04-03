using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using WebApplication1.Models;
using System.Security.Cryptography;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyDbContext _context;

        public AccountController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var admin = await _context.Administrators   
                .FirstOrDefaultAsync(a => a.Login == login);

            if (admin != null)
            {
                byte[] hashedPassword = HashPassword(password);

                if (CompareByteArrays(hashedPassword, admin.Password))
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, admin.Login),
                        new("ADMIN_ID", admin.AdminId.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }

        private static byte[] HashPassword(string password)
        {
            const string salt = "MedicalClinicSalt";
            string saltedPassword = password + salt;
            return SHA256.HashData(Encoding.Unicode.GetBytes(saltedPassword));
        }

        private static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return false;

            return array1.AsSpan().SequenceEqual(array2);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
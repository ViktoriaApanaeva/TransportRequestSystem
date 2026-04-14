using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TransportRequestSystem.Data;

namespace TransportRequestSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Ищем пользователя через сырой SQL
            string userId = null;
            string userEmail = null;
            string role = "User";

            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                if (cmd.Connection.State != System.Data.ConnectionState.Open)
                    await cmd.Connection.OpenAsync();

                cmd.CommandText = "SELECT \"Id\", \"Email\", COALESCE(\"Role\", 'User') FROM \"AspNetUsers\" WHERE \"Email\" = @email";
                cmd.Parameters.Add(new Npgsql.NpgsqlParameter("@email", email));

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        userId = reader.GetString(0);
                        userEmail = reader.GetString(1);
                        role = reader.GetString(2);
                    }
                }
            }

            // Если пользователь не найден - создаем
            if (userId == null)
            {
                userId = Guid.NewGuid().ToString();

                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"AspNetUsers\" (\"Id\", \"UserName\", \"Email\", \"EmailConfirmed\", \"Role\", \"LockoutEnabled\", \"AccessFailedCount\") VALUES (@id, @email, @email, true, 'User', false, 0)";
                    cmd.Parameters.Add(new Npgsql.NpgsqlParameter("@id", userId));
                    cmd.Parameters.Add(new Npgsql.NpgsqlParameter("@email", email));
                    await cmd.ExecuteNonQueryAsync();
                }
                role = "User";
                userEmail = email;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userEmail),
                new Claim(ClaimTypes.Email, userEmail),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Applications");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
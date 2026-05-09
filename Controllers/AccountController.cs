using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Npgsql;
using TransportRequestSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TransportRequestSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Введите логин и пароль";
                return View();
            }

            // Ищем пользователя в таблице AspNetUsers
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                // Создаём нового пользователя
                var hashedPassword = HashPassword(password);

                user = new IdentityUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = username,
                    Email = $"{username}@local.com",
                    PasswordHash = hashedPassword
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Добавляем роль по умолчанию через ExecuteSqlRaw
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE \"AspNetUsers\" SET \"Role\" = 'User' WHERE \"Id\" = {0}", user.Id);
            }
            else
            {
                var hashedInputPassword = HashPassword(password);
                if (user.PasswordHash != hashedInputPassword)
                {
                    ViewBag.Error = "Неверный логин или пароль";
                    return View();
                }
            }

            // Получаем роль пользователя через ExecuteSqlRaw
            string role = "User";
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT \"Role\" FROM \"AspNetUsers\" WHERE \"Id\" = @id";
                command.Parameters.Add(new Npgsql.NpgsqlParameter("@id", user.Id));

                _context.Database.OpenConnection();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        role = reader.GetString(0);
                    }
                }
                _context.Database.CloseConnection();
            }

            // Создаём claims для входа
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            return RedirectToAction("Index", "Applications");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Account");
        }
    }
}
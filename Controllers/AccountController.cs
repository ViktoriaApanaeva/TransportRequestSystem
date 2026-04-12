using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TransportRequestSystem.Data;

namespace TransportRequestSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login (MVC - для HTML формы)
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Ищем или создаем пользователя
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Создаем claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Вход
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Applications");
        }

        // POST: /api/account/login (API для фронтенда)
        [HttpPost("api/account/login")]
        public async Task<IActionResult> ApiLogin([FromBody] LoginRequest request)
        {
            // Ищем или создаем пользователя
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = request.Email,
                    Email = request.Email,
                    EmailConfirmed = true
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Генерируем JWT токен
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                success = true,
                token = token,
                user = new
                {
                    email = user.Email,
                    role = "user"
                }
            });
        }

        // Метод для генерации JWT токена
        private string GenerateJwtToken(IdentityUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key-minimum-32-characters-long!"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: "transport-system",
                audience: "transport-system",
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }

    // Класс для данных запроса
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
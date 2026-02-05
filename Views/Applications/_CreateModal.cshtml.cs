using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TransportRequestSystem.Models;
using TransportRequestSystem.Data;

namespace TransportRequestSystem.Pages.Applications
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Application Application { get; set; } = new Application();

        public IActionResult OnGet()
        {
            
            Application.Number = Application.GenerateNumber();
            Application.ApplicationDate = DateTime.Today;
            Application.Status = ApplicationStatus.CreatedOrModified;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                Request.Query["partial"] == "true")
            {
                return Partial("Create", this);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Partial("Create", this);
            }

            Application.CreatedAt = DateTime.Now;
            Application.Status = ApplicationStatus.CreatedOrModified;

            if (string.IsNullOrEmpty(Application.Number))
            {
                Application.Number = Application.GenerateNumber();
            }

            // Сохраняем в БД
            _context.Applications.Add(Application);
            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                Request.Query["partial"] == "true")
            {
                return Content("<script>window.location.reload();</script>", "text/html");
            }
            TempData["Success"] = "Заявка успешно создана!";
            return RedirectToPage("./Index");
        }
    }
}
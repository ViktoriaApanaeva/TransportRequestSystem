using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TransportRequestSystem.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        // GET: /Calendar
        public IActionResult Index()
        {
            return View();
        }
    }
}
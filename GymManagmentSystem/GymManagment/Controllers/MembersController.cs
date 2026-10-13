using Microsoft.AspNetCore.Mvc;

namespace GymManagment.PL.Controllers
{
    public class MembersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

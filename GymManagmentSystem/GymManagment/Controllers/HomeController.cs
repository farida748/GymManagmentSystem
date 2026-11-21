using System.Diagnostics;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymManagment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAnalyticsService _analyticsService;

        public HomeController(ILogger<HomeController> logger,IAnalyticsService analyticsService)
        {
            _logger = logger;
            _analyticsService = analyticsService;
        }

        public async Task<IActionResult> IndexAsync(CancellationToken cancellationToken)
        {
            var data =await _analyticsService.GetAnalyticsDataAsync(cancellationToken);
            return View(data);
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}

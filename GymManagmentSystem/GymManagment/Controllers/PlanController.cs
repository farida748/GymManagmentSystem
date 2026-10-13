using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagment.PL.Controllers
{
    public class PlanController : Controller
    {
        private readonly IGenericRepository<Plan> _planRepository; 
        
        public PlanController(IGenericRepository<Plan> planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planRepository.GetAllAsync(false, ct);
            return View(plans);
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await _planRepository.GetByIdAsync(id, ct);

            if (plan is null)
                return RedirectToAction(nameof(Index));

            return View(plan);
        }

    }
}

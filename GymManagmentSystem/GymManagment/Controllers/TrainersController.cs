using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.TrainerViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace GymManagment.PL.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
           _trainerService = trainerService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var trainers = await _trainerService.GetAllTrainersAsync(ct);
            return View(trainers);
        }
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {

                return View(nameof(Create), model);
            }
          
            var result = await _trainerService.CreateTrainerAsync(model, ct);
            if (result.Success) { 
                TempData["SuccessMessage"] = "Trainer Created Sucessfully";
                return RedirectToAction(nameof(Index));

            }
            else
            {
               TempData["ErrorMessage"] = result.Error;
                return View( model);

            }
           
            
        }
        public async Task<IActionResult> Details(int id ,CancellationToken ct)
        {
            var result = await _trainerService.GetTrainersDetailsAsync(id, ct);
            if (!result.Success)
            {
                return NotFound();
            }
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id,CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerForUpdateAsync(id, ct);
            if (!trainer.Success)
            {
                return NotFound();
            }  
            return View(trainer.Data );
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateTrainerViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid)
            {
                return View( model);
            } 
            var result = await _trainerService.UpdateTrainerAsync(id, model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Trainer Created Sucessfully";
                return RedirectToAction(nameof(Index));

            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return View(model);

            }

        }
       

        public async Task<IActionResult> Delete(int id ,CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerByIdAsync(id, ct);
            return View();

        }
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _trainerService.DeleteTrainerAsync(id, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Trainer Deleted Sucessfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

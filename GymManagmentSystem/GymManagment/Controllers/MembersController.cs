using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.MemberViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GymManagment.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _memberservice;

        //Get BaseUrl/Members/Index
        //Index list all members
        public MembersController(IMemberService memberService)
        {
            _memberservice = memberService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberservice.GetAllMembersAsync(ct);
            return View(members);
        }
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
       
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);

            var result = await _memberservice.CreateMemberAsync(model, ct);
            if (result)
            {
                TempData["SucessMessage"] = "Member Created Sucessfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Member Failed to Create";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

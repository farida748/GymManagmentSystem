using GymManagment.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagment.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _memberservice;

        //Get BaseUrl/Members/Index
        //Index list all members
        public   MembersController(IMemberService memberService) {
             _memberservice =memberService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members= await _memberservice.GetAllMembersAsync(ct);
            return View(members);
        }

    }
}

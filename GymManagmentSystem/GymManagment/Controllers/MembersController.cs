using GymManagment.BLL.Services.Attachment;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.MemberViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GymManagment.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _memberservice;
        private readonly IAttachmentService _attachmentService;

        //Get BaseUrl/Members/Index
        //Index list all members
        public MembersController(IMemberService memberService,IAttachmentService attachmentService)
        {
            _memberservice = memberService;
            _attachmentService = attachmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Picture(int id)
        {
            var member = await _memberservice.GetMembersDetailsByIdAsync(id, CancellationToken.None);
            if(member is null || string.IsNullOrWhiteSpace(member.Photo))
            {
                return NotFound();
            }
            var result = _attachmentService.GetFile(member.Photo, "MembersPhoto");
            if(result is null)
            {
                return NotFound();
            }
            return File(result.Value.stream, result.Value.ContentType);

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
            if (result.Success)
            {
                TempData["SucessMessage"] = "Member Created Sucessfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Member Failed to Create";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _memberservice.GetMembersDetailsByIdAsync(id, ct);

            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var result =await _memberservice.GetMemberHealthRecordAsync(id, ct);
            if (result is null)
            {

                TempData["ErrorMessage"] = "Health Record Is Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(result);

        }
        [HttpGet]
        public async Task<IActionResult> EditMember(int id,CancellationToken cancellationToken)
        {
            var member = await _memberservice.GetMembersToUpdateAsync(id, cancellationToken);

            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);

        }

        [HttpPost]
        public async Task<IActionResult> EditMemberAsync([FromRoute]int id,MemberToUpdateViewModel model,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _memberservice.UpdateMemberDetailsAsync(id, model, cancellationToken);
            if (result.Success)
                TempData["SucessMessage"] = "Member Updated Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Update Member";

            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id,CancellationToken cancellationToken)
        {
            var member=await _memberservice.GetMembersDetailsByIdAsync(id, cancellationToken);
            if(member is null)
            {
                TempData["ErrorMessage"] = "Member is not Found";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedAsync([FromRoute]int id,CancellationToken cancellationToken)
        {
            var result= await _memberservice.RemoveMemberAsync(id, cancellationToken);
            if (result.Success)
                TempData["SucessMessage"] = "Member deleted successfully";
            else
                TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }


    }
}

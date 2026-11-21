using AutoMapper;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Attachment;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.MemberViewModel;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GymManagment.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitofWork unitofWork , IMapper mapper,IAttachmentService attachmentService) {
           _unitofWork = unitofWork;
           _mapper = mapper;
           _attachmentService = attachmentService;
        }

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel member, CancellationToken cancellationToken = default)
        {
            var emailExist = await _unitofWork.GetRepository<Member>().AnyAsync(x => x.Email == member.Email, cancellationToken);
            var phoneExist = await _unitofWork.GetRepository<Member>().AnyAsync(x => x.Phone == member.Phone, cancellationToken);
            if (emailExist || phoneExist) return Result.Fail("Email or phone number already exists");

            if (member.PhotoFile is null || member.PhotoFile.Length == 0)
                return Result.Fail("Photo is required");

            await using var photoStream = member.PhotoFile.OpenReadStream();
            var uploadResult = await _attachmentService.UploadAsync(
                photoStream,
                member.PhotoFile.FileName,
                "MembersPhoto",
                cancellationToken);

            if (!uploadResult.Success || string.IsNullOrWhiteSpace(uploadResult.Data))
                return Result.Fail(uploadResult.Error ?? "Failed to upload photo");

            var storedPhotoName = uploadResult.Data;

            var newMember = _mapper.Map<CreateMemberViewModel, Member>(member);
            newMember.Photo = storedPhotoName;

            _unitofWork.GetRepository<Member>().Add(newMember);
            var result = await _unitofWork.SaveChangesAsync(cancellationToken);

            if (result > 0)
                return Result.Ok();

            _attachmentService.Delete(storedPhotoName, "MembersPhoto");
            return Result.Fail("Failed to create member");
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitofWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any())
            {
                return [];
            }
            var memberViewModels =_mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);

            return memberViewModels;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int MemberId, CancellationToken cancellationToken = default)
        {
            var record = await _unitofWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(m => m.Id == MemberId, cancellationToken: cancellationToken);
            if (record == null) return null;
            return _mapper.Map<HealthRecord, HealthRecordViewModel>(record);


        }

        public async Task<MemberViewModel?> GetMembersDetailsByIdAsync(int MemberId, CancellationToken cancellationToken = default)
        {
            var member = await _unitofWork.GetRepository<Member>().GetByIdAsync(MemberId, cancellationToken);
            if (member == null)
                return null;

            var model =_mapper.Map<Member, MemberViewModel>(member);
            var activeMemberShip = await _unitofWork.GetRepository<MemberShip>().FirstOrDefaultAsync(x => x.MemberId == MemberId && x.EndDate > DateTime.Now);
            if (activeMemberShip is not null)
            {
                var activePlan = await _unitofWork.GetRepository<Plan>().GetByIdAsync(activeMemberShip.PlanId);
                model.PlanName = activePlan?.Name;
                model.MemberShipStartDate = activeMemberShip.CreatedAt.ToString();
                model.MemberShipEndDate = activeMemberShip.EndDate.ToString();
            }
            return model;
        }

        public async Task<MemberToUpdateViewModel?> GetMembersToUpdateAsync(int memberId, CancellationToken cancellationToken = default)
        {
            var member = await _unitofWork.GetRepository<Member>().GetByIdAsync(memberId, cancellationToken);
            if (member == null) return null;
            return _mapper.Map<Member, MemberToUpdateViewModel>(member);
        }

        public async Task<Result> RemoveMemberAsync(int memberid, CancellationToken cancellationToken = default)
        {
            var member= await _unitofWork.GetRepository<Member>().GetByIdAsync(memberid, cancellationToken);
            if (member == null) return Result.Fail("Member not found");

            var hasfutureBooking = await _unitofWork.GetRepository<Booking>().AnyAsync(x => x.MemberId == memberid && x.Session.StartDate > DateTime.Now,cancellationToken);
            
             _unitofWork.GetRepository<Member>().Delete(member);
            var result= await _unitofWork.SaveChangesAsync(cancellationToken);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to remove member");
        }

        public async Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken cancellationToken = default)
        {
            var member = await _unitofWork.GetRepository<Member>().GetByIdAsync(id, cancellationToken);

            if (member == null) return Result.Fail("Member not found");

            var emailExist = await _unitofWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email && x.Id != id);
            var phoneExist = await _unitofWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone && x.Id != id);

            if (emailExist || phoneExist) return Result.Fail("Email or phone number already exists");

            _mapper.Map(model, member);
            member.UpdatedAt= DateTime.Now;

             _unitofWork.GetRepository<Member>().Update(member);
            var result = await _unitofWork.SaveChangesAsync(cancellationToken);

            return result > 0 ? Result.Ok() : Result.Fail("Failed to update member");

        }
    }
}
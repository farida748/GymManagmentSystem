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

namespace GymManagment.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitofWork _unitofWork;

        public MemberService(IUnitofWork unitofWork) {
           _unitofWork = unitofWork;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel member, CancellationToken cancellationToken = default)
        {
            var emailExist = await _unitofWork.GetRepository<Member>().AnyAsync(x => x.Email == member.Email, cancellationToken);
            var phoneExist = await _unitofWork.GetRepository<Member>().AnyAsync(x => x.Phone == member.Phone, cancellationToken);
            if (emailExist || phoneExist) return false;

            var members = new Member()
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Gender = member.Gender,
                DateOfBirth = member.DateOfBirth,
                Adress = new Adress()
                {
                    BuildingNumber = member.BuildingNumber,
                    Street = member.Street,
                    City = member.City,

                },
                HealthRecord = new HealthRecord()
                {
                    BloodType = member.HealthRecordViewModel.BloodType,
                    Weight = member.HealthRecordViewModel.Weight,
                    Height = member.HealthRecordViewModel.Height,
                    Note = member.HealthRecordViewModel.Note,
                }



            };
              _unitofWork.GetRepository<Member>().Add(members);
            var result=await _unitofWork.SaveChangesAsync(cancellationToken);
            return result > 0;

        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitofWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any())
            {
                return [];
            }
            var memberViewModels = members.Select(m => new MemberViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Photo = m.Photo,
                Email = m.Email,
                Phone = m.Phone,
                Gender = m.Gender.ToString()
            });

            return memberViewModels;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int MemberId, CancellationToken cancellationToken = default)
        {
            var record = await _unitofWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(m => m.Id == MemberId, cancellationToken: cancellationToken);
            if (record == null) return null;
            return new HealthRecordViewModel()
            {
                BloodType = record.BloodType,
                Note = record.Note,
                Height = record.Height,
                Weight = record.Weight,

            };


        }

        public async Task<MemberViewModel?> GetMembersDetailsByIdAsync(int MemberId, CancellationToken cancellationToken = default)
        {
            var member = await _unitofWork.GetRepository<Member>().GetByIdAsync(MemberId, cancellationToken);
            if (member == null)
                return null;

            var model = new MemberViewModel()
            {
                Name = member.Name,
                Phone = member.Phone,
                DateOfBirth = member.DateOfBirth.ToString(),
                Gender = member.Gender.ToString(),
                Address = $"{member.Adress.BuildingNumber}-{member.Adress.Street}-{member.Adress.City}",
                Email = member.Email,

            };
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
            return new MemberToUpdateViewModel()
            {

                Name = member.Name,
                Phone = member.Phone,
                Photo = member.Photo,
                Email = member.Email,
                BuildingNumber = member.Adress.BuildingNumber,
                Street = member.Adress.Street,
                City = member.Adress.City,



            };
        }

        public async Task<bool> RemoveMemberAsync(int memberid, CancellationToken cancellationToken = default)
        {
            var member= await _unitofWork.GetRepository<Member>().GetByIdAsync(memberid, cancellationToken);
            if (member == null) return false;

            var hasfutureBooking = await _unitofWork.GetRepository<Booking>().AnyAsync(x => x.MemberId == memberid && x.Session.StartDate > DateTime.Now,cancellationToken);
            
             _unitofWork.GetRepository<Member>().Delete(member);
            var result= await _unitofWork.SaveChangesAsync(cancellationToken);
            return result > 0;

        }

        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken cancellationToken = default)
        {
            var member = await _unitofWork.GetRepository<Member>().GetByIdAsync(id, cancellationToken);

            if (member == null) return false;

            var emailExist = await _unitofWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email && x.Id != id);
            var phoneExist = await _unitofWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone && x.Id != id);

            if (emailExist || phoneExist) return false;

            member.Email = model.Email;
            member.Phone = model.Phone;

            member.Adress.BuildingNumber = model.BuildingNumber;
            member.Adress.Street = model.Street;
            member.Adress.City = model.City;
            member.UpdatedAt= DateTime.Now;

             _unitofWork.GetRepository<Member>().Update(member);
            var result = await _unitofWork.SaveChangesAsync(cancellationToken);

            return result > 0;

        }
    }
}
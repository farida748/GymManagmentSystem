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
        private readonly IGenericRepository<Member> _memberRepository;

        public MemberService(IGenericRepository<Member> memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel member, CancellationToken cancellationToken = default)
        {
           var emailExist=await _memberRepository.AnyAsync(x=> x.Email == member.Email,cancellationToken);
            var phoneExist = await _memberRepository.AnyAsync(x => x.Phone == member.Phone, cancellationToken);
            if(emailExist|| phoneExist) return false;

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
               HealthRecord=new HealthRecord()
               {
                   BloodType=member.HealthRecordViewModel.BloodType,
                   Weight=member.HealthRecordViewModel.Weight,
                   Height=member.HealthRecordViewModel.Height,
                   Note=member.HealthRecordViewModel.Note,
               }



            };
          var result= await _memberRepository.AddAsync(members);
            return result > 0;

        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _memberRepository.GetAllAsync(ct:ct);
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
    }
}

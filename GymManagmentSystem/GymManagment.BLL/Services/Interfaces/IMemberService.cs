using GymManagment.BLL.ViewModels.MemberViewModel;
using GymManagment.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task <IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken cancellationToken=default);
        Task<bool> CreateMemberAsync(CreateMemberViewModel member, CancellationToken cancellationToken = default);
        Task<MemberViewModel?> GetMembersDetailsByIdAsync(int MemberId,CancellationToken cancellationToken=default);
        Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int MemberId,CancellationToken cancellationToken=default);
        Task<MemberToUpdateViewModel?> GetMembersToUpdateAsync(int memberId, CancellationToken cancellationToken = default);
        Task<bool> UpdateMemberDetailsAsync(int id,MemberToUpdateViewModel model, CancellationToken cancellationToken = default);
        Task<bool> RemoveMemberAsync(int memberid,CancellationToken cancellationToken=default);

    }
}

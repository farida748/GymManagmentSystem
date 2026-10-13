using GymManagment.BLL.ViewModels.MemberViewModel;
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
    }
}

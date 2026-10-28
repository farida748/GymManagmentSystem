using GymManagment.BLL.Common;
using GymManagment.BLL.ViewModels.SessionViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default);
        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken cancellationToken = default);

        Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDownAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDownAsync(CancellationToken cancellationToken = default);

        Task<Result<SessionViewModel>> GetSessionByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken cancellationToken = default);
        Task<Result<UpdateSessionViewModel>> GetSessionForUpdateByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Result> RemoveSessionAsync(int id, CancellationToken cancellationToken = default);

    }
}

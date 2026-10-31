using GymManagment.BLL.Common;
using GymManagment.BLL.ViewModels.PlanViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Interfaces
{
   public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct);
        Task<Result<PlanViewModel>>GetPlanDetailsByIdAsync(int id, CancellationToken ct);
        Task<Result<UpdatePlanViewModel>> GetPlanToUpdateAsync(int id, CancellationToken ct);

        Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct);

        Task<Result> TogglePlanStatusAsync(int id, CancellationToken ct);
    }
}

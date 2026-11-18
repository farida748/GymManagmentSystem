using GymManagment.BLL.Common;
using GymManagment.BLL.ViewModels.TrainerViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        public Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct);
        public Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct);

        public Task<Result<TrainerDetailsViewModel>> GetTrainersDetailsAsync(int id, CancellationToken ct);

        public Task<Result<UpdateTrainerViewModel>> GetTrainerForUpdateAsync(int id, CancellationToken ct);

        public Task<Result> UpdateTrainerAsync(int id, UpdateTrainerViewModel model, CancellationToken ct);

        public Task<Result> DeleteTrainerAsync(int id, CancellationToken ct);

        public Task<Result<TrainerViewModel>> GetTrainerByIdAsync(int id, CancellationToken ct);
    }
}

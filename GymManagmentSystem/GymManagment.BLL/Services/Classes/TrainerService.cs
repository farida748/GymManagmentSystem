using AutoMapper;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.TrainerViewModel;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Classes;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Classes
{
    
    public class TrainerService : ITrainerService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IMapper _mapper;

        public TrainerService(IUnitofWork unitofWork,IMapper mapper)
        {
          _unitofWork = unitofWork;
          _mapper = mapper;
        }

        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct)
        {
          var trainer = _mapper.Map<Trainer>(model);
          _unitofWork.GetRepository<Trainer>().Add(trainer);
            var EmailExists = await _unitofWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email, ct);
            if(EmailExists) return Result.Fail("Trainer with the same email already exists");
            var PhoneExists = await _unitofWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone, ct);
            if (PhoneExists) return Result.Fail("Trainer with the same phone number already exists");
            var result = await _unitofWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to create trainer");

        }

        public async Task<Result> DeleteTrainerAsync(int id, CancellationToken ct)
        {
          var trainer = await _unitofWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
          if (trainer == null) return Result.NotFound();
            var hasActiveSessions = await _unitofWork.GetRepository<Session>()
          .AnyAsync(s => s.TrainerId == id
                      && s.StartDate > DateTime.Now
                     );

            if (hasActiveSessions)
                return Result.Fail("Cannot delete a trainer who has scheduled sessions. Reassign or cancel them first.");

           _unitofWork.GetRepository<Trainer>().Delete(trainer);
            var saved = await _unitofWork.SaveChangesAsync(ct);
            return saved > 0 ? Result.Ok() : Result.Fail("Failed to delete trainer");
           
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct)
        {
         
            var trainers = await _unitofWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            var mappedTrainers = _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);
            return mappedTrainers;

        }

        public async Task<Result<TrainerViewModel>> GetTrainerByIdAsync(int id, CancellationToken ct)
        {
           var trainer = await _unitofWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null) return Result<TrainerViewModel>.NotFound();
            var mappedTrainer = _mapper.Map<TrainerViewModel>(trainer);
            return Result<TrainerViewModel>.Ok(mappedTrainer);
        }

        public async Task<Result<UpdateTrainerViewModel>> GetTrainerForUpdateAsync(int id, CancellationToken ct)
        { 
        
            var trainer = await _unitofWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
          if(trainer== null) return Result<UpdateTrainerViewModel>.NotFound();
            var mappedTrainer = _mapper.Map<UpdateTrainerViewModel>(trainer);
            return Result<UpdateTrainerViewModel>.Ok(mappedTrainer);

        }

        public async Task<Result<TrainerDetailsViewModel>> GetTrainersDetailsAsync(int id, CancellationToken ct)
        {
            var trainer =await _unitofWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null) return Result<TrainerDetailsViewModel>.NotFound();
            var maappedTrainers = _mapper.Map<TrainerDetailsViewModel>(trainer);
            return Result<TrainerDetailsViewModel>.Ok(maappedTrainers);
        }

        public async Task<Result> UpdateTrainerAsync(int id, UpdateTrainerViewModel model, CancellationToken ct)
        {
           var trainer =await  _unitofWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if(trainer==null) return Result.NotFound();
            var mappedTrainer = _mapper.Map(model, trainer);
            _unitofWork.GetRepository<Trainer>().Update(mappedTrainer);
            var result = await _unitofWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update trainer");
        }
    }
}

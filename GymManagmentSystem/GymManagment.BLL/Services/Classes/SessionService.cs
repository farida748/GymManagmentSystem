using AutoMapper;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.SessionViewModel;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Data.Models.Enums;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitofWork unitofWork,IMapper mapper) {
            _unitofWork = unitofWork;
            _mapper = mapper;   
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken cancellationToken = default)
        {
            if (model.EndDate <= model.StartDate) return Result.ValidationError("End date must be after start date");
            if(model.StartDate < DateTime.Now) return Result.ValidationError("Start date must be in the future");
            if(model.Capacity <1 || model.Capacity>25) return Result.ValidationError("Capacity must be between 1 and 25");

            var trainer=await _unitofWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, cancellationToken);
            if(trainer == null) return Result.NotFound("Trainer not found");

            var category = await _unitofWork.GetRepository<Category>().GetByIdAsync(model.CategoryId, cancellationToken);
            if(category == null) return Result.NotFound("Category not found");

            var isvalid=Enum.TryParse<Speciality>(category.CategoryName,true, out var Categoryspeciality);
            if(!isvalid|| trainer.Speciality != Categoryspeciality) return Result.ValidationError
                    ("Incompatible trainer and category");

            var session=_mapper.Map<Session>(model);
            _unitofWork.GetRepository<Session>().Add(session);
            var result=await _unitofWork.SaveChangesAsync(cancellationToken);

            return result>0 ? Result.Ok() : Result.Fail("Failed to create session");
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _unitofWork.SessionRepository.GetAllSessionsWithTrainerAndCategory(ct);
            var mappedSession= _mapper.Map<IEnumerable<SessionViewModel>>(sessions);

            // sessions.Select(s => new SessionViewModel
            //{
            //    Id = s.Id,
            //    Capacity = s.Capacity,
            //    TrainerName = s.Trainer.Name,
            //    CategoryName = s.Category.CategoryName,
            //    Description = s.Description,
            //    StartDate = s.StartDate,
            //    EndDate = s.EndDate,
            //});

            foreach (var session in mappedSession)
            {
                var availableSlots = await _unitofWork.SessionRepository.GetCountOfBookingSlotsAsync(session.Id, ct);
                session.AvailableSlots = availableSlots;
            }

            return mappedSession;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDownAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _unitofWork.GetRepository<Category>().GetAllAsync(ct: cancellationToken);
            var mappedCategories = _mapper.Map<IEnumerable<CategorySelectViewModel>>(categories);
            return mappedCategories;

        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDownAsync(CancellationToken cancellationToken = default)
        {
            var trainers = await _unitofWork.GetRepository<Trainer>().GetAllAsync(ct: cancellationToken);
            var mappedTrainers = _mapper.Map<IEnumerable<TrainerSelectViewModel>>(trainers);
            return mappedTrainers;
        }
    }
}

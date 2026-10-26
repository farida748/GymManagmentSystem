using AutoMapper;
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

        public async Task<bool> CreateSessionAsync(CreateSessionViewModel model, CancellationToken cancellationToken = default)
        {
            if (model.EndDate <= model.StartDate) return false;
            if(model.StartDate < DateTime.Now) return false;
            if(model.Capacity <1 || model.Capacity>25) return false;

            var trainer=await _unitofWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, cancellationToken);
            if(trainer == null) return false;

            var category = await _unitofWork.GetRepository<Category>().GetByIdAsync(model.CategoryId, cancellationToken);
            if(category == null) return false;

            var isvalid=Enum.TryParse<Speciality>(category.CategoryName,true, out var Categoryspeciality);
            if(!isvalid|| trainer.Speciality != Categoryspeciality) return false;

            var session=_mapper.Map<Session>(model);
            _unitofWork.GetRepository<Session>().Add(session);
            var result=await _unitofWork.SaveChangesAsync(cancellationToken);

            return result>0;
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

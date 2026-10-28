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

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await _unitofWork.SessionRepository.GetSessionWithTrainerAndCategoryByIdAsync(id, cancellationToken);
            if (session == null) return Result<SessionViewModel>.NotFound("Session not found");

            var mappedSession = _mapper.Map<SessionViewModel>(session);
            var availableSlots =  mappedSession.Capacity- await _unitofWork.SessionRepository.GetCountOfBookingSlotsAsync(session.Id, cancellationToken);
            mappedSession.AvailableSlots = availableSlots;

            return Result<SessionViewModel>.Ok(mappedSession);
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionForUpdateByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var session= await _unitofWork.GetRepository<Session>().GetByIdAsync(id, cancellationToken);
            if (session == null) return Result<UpdateSessionViewModel>.NotFound("Session not found");
            if(session.StartDate < DateTime.Now) return Result<UpdateSessionViewModel>.Fail("Cannot update session that has already started");
            var bookingslots = await _unitofWork.SessionRepository.GetCountOfBookingSlotsAsync(session.Id, cancellationToken);
            if(bookingslots > 0) return Result<UpdateSessionViewModel>.Fail("Cannot update session that has bookings");

            var mappedSession = _mapper.Map<UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(mappedSession);
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDownAsync(CancellationToken cancellationToken = default)
        {
            var trainers = await _unitofWork.GetRepository<Trainer>().GetAllAsync(ct: cancellationToken);
            var mappedTrainers = _mapper.Map<IEnumerable<TrainerSelectViewModel>>(trainers);
            return mappedTrainers;
        }

        public async Task<Result> RemoveSessionAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await _unitofWork.GetRepository<Session>().GetByIdAsync(id, cancellationToken);
            if (session == null) return Result.NotFound("Session is Not Found");
            if (session.EndDate >= DateTime.Now) return Result.ValidationError("Can Not Delete Session That has not Ended Yet");
            var bookingslots = await _unitofWork.SessionRepository.GetCountOfBookingSlotsAsync(session.Id, cancellationToken);
            if (bookingslots > 0) return Result.Fail("Can Not Delete Session That Has Bookings");
            _unitofWork.GetRepository<Session>().Delete(session);
            var result = await _unitofWork.SaveChangesAsync(cancellationToken);
            return result > 0 ? Result.Ok() : Result.Fail("Session cannot be Deleted");
        }

        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken cancellationToken = default)
        {
            var session = await  _unitofWork.GetRepository<Session>().GetByIdAsync(id, cancellationToken);
            if (session == null) return Result.NotFound("Not Found");

            if (session.StartDate < DateTime.Now) return Result.Fail("Cannot update session that has already started");
            if (session.EndDate < session.StartDate) return Result.ValidationError("End Date Must Be After Start Date ");

            var bookingslots = await _unitofWork.SessionRepository.GetCountOfBookingSlotsAsync(session.Id, cancellationToken);
            if (bookingslots > 0) return Result.Fail("Cannot update session that has bookings");

            if (session.StartDate < DateTime.Now) return Result.Fail("Cannot update session that has already started");

            var trainer = await _unitofWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, cancellationToken);
            if (trainer == null) return Result.NotFound("Trainer not found");

            var category = await _unitofWork.GetRepository<Category>().GetByIdAsync(session.CategoryId, cancellationToken);
           
            var isvalid = Enum.TryParse<Speciality>(category?.CategoryName, true, out var Categoryspeciality);
            if (!isvalid || trainer.Speciality != Categoryspeciality) return Result.ValidationError
                    ("Incompatible trainer and category");

            _mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;
            _unitofWork.GetRepository<Session>().Update(session);
            var result = await _unitofWork.SaveChangesAsync(cancellationToken);

            return result > 0 ? Result.Ok() : Result.Fail("Session cannot be Updated");
        }
    }
}

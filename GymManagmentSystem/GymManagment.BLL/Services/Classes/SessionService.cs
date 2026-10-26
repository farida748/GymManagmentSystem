using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.SessionViewModel;
using GymManagment.DAL.Data.Models;
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

        public SessionService(IUnitofWork unitofWork) {
            _unitofWork = unitofWork;
        }
        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _unitofWork.SessionRepository.GetAllSessionsWithTrainerAndCategory(ct);
            var mappedSession= sessions.Select(s => new SessionViewModel
            {
                Id = s.Id,
                Capacity = s.Capacity,
                TrainerName = s.Trainer.Name,
                CategoryName = s.Category.CategoryName,
                Description = s.Description,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
            });

            foreach (var session in mappedSession)
            {
                var availableSlots = await _unitofWork.SessionRepository.GetCountOfBookingSlotsAsync(session.Id, ct);
                session.AvailableSlots = availableSlots;
            }

            return mappedSession;
        }
    }
}

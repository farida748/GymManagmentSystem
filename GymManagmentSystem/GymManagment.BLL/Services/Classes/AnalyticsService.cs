using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.AnalyticsViewModel;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitofWork _unitofWork;

        public AnalyticsService(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        public async Task<AnalyticsViewModel> GetAnalyticsDataAsync(CancellationToken cancellationToken = default)
        {
            var Now = DateTime.Now;
            var upcomingSessions = await _unitofWork.GetRepository<Session>().GetCountAsync(s => s.StartDate > Now, cancellationToken);   
           var ongoingSessions = await _unitofWork.GetRepository<Session>().GetCountAsync(s => s.StartDate <= Now && s.EndDate >= Now, cancellationToken);
           var completedSessions = await _unitofWork.GetRepository<Session>().GetCountAsync(s => s.EndDate < Now, cancellationToken);
            var totalMembers = await _unitofWork.GetRepository<Member>().GetCountAsync(cancellationToken: cancellationToken);
            var totalTrainers = await _unitofWork.GetRepository<Trainer>().GetCountAsync(cancellationToken: cancellationToken);
            var activeMembers = await _unitofWork.GetRepository<MemberShip>().GetCountAsync(m => m.EndDate > Now, cancellationToken);

            return new AnalyticsViewModel
            {
                TotalMembers = totalMembers,
                TotalTrainers = totalTrainers,
                ActiveMembers = activeMembers,
                UpcomingSessions = upcomingSessions,
                OngoingSessions = ongoingSessions,
                CompletedSessions = completedSessions
            };


        }
    }
}

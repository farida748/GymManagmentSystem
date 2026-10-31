using AutoMapper;
using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.BLL.ViewModels.PlanViewModel;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IMapper _mapper;

        public PlanService(IUnitofWork unitofWork, IMapper mapper)
        {
            _unitofWork = unitofWork;
            _mapper = mapper;

        }



        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct)
        {
            var plans = await _unitofWork.GetRepository<Plan>().GetAllAsync(ct: ct);

            var mappedplans = _mapper.Map<IEnumerable<PlanViewModel>>(plans);

            return mappedplans;

        }

        public async Task<Result<PlanViewModel>> GetPlanDetailsByIdAsync(int id, CancellationToken ct)
        {
            var plan = await _unitofWork.GetRepository<Plan>().GetByIdAsync(id, ct);

            if (plan == null)
            {
                return Result<PlanViewModel>.NotFound("Plan not found.");
            }

            var mappedPlan = _mapper.Map<PlanViewModel>(plan);
            return Result<PlanViewModel>.Ok(mappedPlan);
        }

        public async Task<Result<UpdatePlanViewModel>> GetPlanToUpdateAsync(int id, CancellationToken ct)
        {
            var plan = await _unitofWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null) return Result<UpdatePlanViewModel>.NotFound("Plan Not Found");
            var mappedPlan = _mapper.Map<UpdatePlanViewModel>(plan);
            return Result<UpdatePlanViewModel>.Ok(mappedPlan);
        }

        public async Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct)
        {
            var plan = await _unitofWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if(plan is null) return Result.NotFound("Plan Not Found");
            else if(plan.IsActive)
            {
                return Result.Fail("Cannot update an active plan.");
            }
            _mapper.Map(model, plan);
            plan.UpdatedAt = DateTime.Now;

            _unitofWork.GetRepository<Plan>().Update(plan);
            var result = await _unitofWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update plan.");


        }

        public async Task<Result> TogglePlanStatusAsync(int id, CancellationToken ct)
        {
            var plan = await _unitofWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null) return Result.NotFound("Plan Not Found");

            // Cannot deactivate a plan that still has active memberships
            if (plan.IsActive)
            {
                var hasActiveMemberships = await _unitofWork.GetRepository<MemberShip>()
                    .AnyAsync(m => m.PlanId == id && m.EndDate > DateTime.Now, ct);

                if (hasActiveMemberships)
                    return Result.Fail("Cannot deactivate a plan with active memberships.");
            }

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitofWork.GetRepository<Plan>().Update(plan);
            var result = await _unitofWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to change plan status.");
        }
    }
}
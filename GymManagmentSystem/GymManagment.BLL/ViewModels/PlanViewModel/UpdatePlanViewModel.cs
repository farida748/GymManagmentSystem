using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.ViewModels.PlanViewModel
{
    public class UpdatePlanViewModel
    {
        public int Id { get; set; }
        public string PlanName { get; set; } = default!;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; } = default!;
        [Required]
        public int DurationDays { get; set; }
    }
}

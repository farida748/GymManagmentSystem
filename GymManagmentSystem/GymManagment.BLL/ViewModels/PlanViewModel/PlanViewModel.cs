using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.ViewModels.PlanViewModel
{
    public class PlanViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }

        public string Description { get; set; } = default!;

        public int DurationDays { get; set; }

        public bool IsActive { get; set; }

    }
}

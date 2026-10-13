using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Models
{
   public class Member : GymUser
    {
        public string? Photo { get; set; }

        #region Navigation Properties
        public HealthRecord HealthRecord { get; set; }= default!;
        public ICollection<MemberShip> MemberShips { get; set; } = default!;

        public ICollection<Booking> MemberSession { get; set; } = default!;
        #endregion
    }
}

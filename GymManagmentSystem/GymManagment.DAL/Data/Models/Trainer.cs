using GymManagment.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Models
{
  public class Trainer : GymUser
    {
        //HireDate =CreatedAt of the BaseEntity class
        public Speciality Speciality { get; set; } = default!;

        public ICollection<Session> Sessions { get; set; } = default!;

    }
}

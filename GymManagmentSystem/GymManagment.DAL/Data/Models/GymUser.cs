using GymManagment.DAL.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Models
{
   public abstract class GymUser : BaseEntity
    {
        public string  Name { get; set; } =default!;
        public string  Email { get; set; } = default!;

        public string Phone{ get; set; } = default!;
        public DateOnly DateOfBirth { get; set; }
        public Adress Adress { get; set; } = default!;

        public Gender Gender { get; set; }

    }
    [Owned]
    public class Adress
    {
        public string Street { get; set; } = default!;
        public string City { get; set; } = default!;
        public int BuildingNumber{ get; set; } = default!;
    }
}

using GymManagment.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace GymManagment.DAL.Configurations
{
    internal class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(x => x.Name)
                 .HasColumnType("varchar")
                 .HasMaxLength(50);

            builder.Property(X => X.Price)
                .HasPrecision(10, 2);

            builder.Property(X => X.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.ToTable(Tb =>
            {
                Tb.HasCheckConstraint("PlanDurationCheck",
                    "DurationDays Between 1 and 365");
            });

        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AssessmentCriteriaAggregate;

namespace Nova.DeptServiceAssesment.Infrastructure.EntityConfigurations
{
    class PeriodSelectedDepartmentConfiguration
        : IEntityTypeConfiguration<PeriodSelectedDepartment>
    {
        public void Configure(EntityTypeBuilder<PeriodSelectedDepartment> configuration)
        {
            configuration.ToTable("PeriodSelectedDepartments");

            configuration.HasKey(b => b.Id);

            configuration.Ignore(b => b.DomainEvents);

            configuration.Property(b => b.AssessmentPeriodId)
                .IsRequired();

            configuration.Property(b => b.DepartmentFrom)
                .IsRequired();

            configuration.Property(b => b.DepartmentTo)
                .IsRequired();

            configuration.Property(b => b.UserId)
                .IsRequired();

            configuration.HasMany(b => b.PeriodAssessmentResults)
               .WithOne()
               .HasForeignKey(x => x.PeriodSelectedDepartmentId)
               .OnDelete(DeleteBehavior.Restrict);

            var navigation = configuration.Metadata.FindNavigation(nameof(PeriodSelectedDepartment.PeriodAssessmentResults));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}

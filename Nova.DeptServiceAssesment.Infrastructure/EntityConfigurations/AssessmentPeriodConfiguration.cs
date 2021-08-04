using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AssessmentCriteriaAggregate;

namespace Nova.DeptServiceAssesment.Infrastructure.EntityConfigurations
{
    class AssessmentPeriodsConfiguration
        : IEntityTypeConfiguration<AssessmentPeriod>
    {
        public void Configure(EntityTypeBuilder<AssessmentPeriod> configuration)
        {
            configuration.ToTable("AssessmentPeriods");

            configuration.HasKey(b => b.Id);

            configuration.Ignore(b => b.DomainEvents);

            configuration.Property(b => b.PeriodName)
                .HasMaxLength(1000)
                .IsRequired();

            configuration.Property(b => b.PeriodFrom)
                .IsRequired();

            configuration.Property(b => b.PeriodTo)
                .IsRequired();

            configuration.HasMany(b => b.PeriodQuetions)
               .WithOne()
               .HasForeignKey(x => x.AssessmentPeriodId)
               .OnDelete(DeleteBehavior.Restrict);

            configuration.HasMany(b => b.PeriodSelectedDepartments)
               .WithOne()
               .HasForeignKey(x => x.AssessmentPeriodId)
               .OnDelete(DeleteBehavior.Restrict);

            var navigation = configuration.Metadata.FindNavigation(nameof(AssessmentPeriod.PeriodQuetions));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Property);

            var navigation2 = configuration.Metadata.FindNavigation(nameof(AssessmentPeriod.PeriodSelectedDepartments));
            navigation2.SetPropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}

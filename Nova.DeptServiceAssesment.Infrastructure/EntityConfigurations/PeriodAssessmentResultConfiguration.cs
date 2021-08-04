using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate;

namespace Nova.DeptServiceAssesment.Infrastructure.EntityConfigurations
{
    class PeriodAssessmentResultConfiguration
        : IEntityTypeConfiguration<PeriodAssessmentResult>
    {
        public void Configure(EntityTypeBuilder<PeriodAssessmentResult> configuration)
        {
            configuration.ToTable("PeriodAssessmentResults");

            configuration.HasKey(b => b.Id);

            configuration.Ignore(b => b.DomainEvents);

            configuration.Property(b => b.PeriodSelectedDepartmentId)
                .IsRequired();

            configuration.Property(b => b.PeriodQuestionId)
                .IsRequired();


            configuration.Property(b => b.ResultComment)
                .HasMaxLength(1000);
        }
    }
}

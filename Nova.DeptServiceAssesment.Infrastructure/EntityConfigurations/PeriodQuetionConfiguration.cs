using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AssessmentCriteriaAggregate;

namespace Nova.DeptServiceAssesment.Infrastructure.EntityConfigurations
{
    class PeriodQuetionConfiguration
        : IEntityTypeConfiguration<PeriodQuetion>
    {
        public void Configure(EntityTypeBuilder<PeriodQuetion> configuration)
        {
            configuration.ToTable("PeriodQuetions");

            configuration.HasKey(b => b.Id);

            configuration.Ignore(b => b.DomainEvents);

            configuration.Property(b => b.AssessmentPeriodId)
                .IsRequired();

            configuration.Property(b => b.AssessmentQuestionId)
                .IsRequired();

            configuration.HasOne(b => b.AssessmentQuestion)
               .WithMany()
               .HasForeignKey(x => x.AssessmentQuestionId)
               .OnDelete(DeleteBehavior.Restrict);

            var navigation = configuration.Metadata.FindNavigation(nameof(PeriodQuetion.AssessmentQuestion));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}

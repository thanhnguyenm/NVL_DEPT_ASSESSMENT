using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AssessmentCriteriaAggregate;

namespace Nova.DeptServiceAssesment.Infrastructure.EntityConfigurations
{
    class AssessmentQuestionConfiguration
        : IEntityTypeConfiguration<AssessmentQuestion>
    {
        public void Configure(EntityTypeBuilder<AssessmentQuestion> configuration)
        {
            configuration.ToTable("AssessmentQuestions");

            configuration.HasKey(b => b.Id);

            configuration.Ignore(b => b.DomainEvents);

            configuration.Property(b => b.Content)
                .IsRequired();
        }
    }
}

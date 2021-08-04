using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AssessmentCriteriaAggregate;

namespace Nova.DeptServiceAssesment.Infrastructure.EntityConfigurations
{
    class AssessmentCriteriaConfiguration
        : IEntityTypeConfiguration<AssessmentCriteria>
    {
        public void Configure(EntityTypeBuilder<AssessmentCriteria> configuration)
        {
            configuration.ToTable("AssessmentCriteria");

            configuration.HasKey(b => b.Id);

            configuration.Ignore(b => b.DomainEvents);

            configuration.Property(b => b.CriteriaName)
                .HasMaxLength(1000)
                .IsRequired();


            configuration.HasMany(b => b.AssessmentQuestions)
               .WithOne()
               .HasForeignKey(x => x.CriteriaId)
               .OnDelete(DeleteBehavior.Restrict);

            var navigation = configuration.Metadata.FindNavigation(nameof(AssessmentCriteria.AssessmentQuestions));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}

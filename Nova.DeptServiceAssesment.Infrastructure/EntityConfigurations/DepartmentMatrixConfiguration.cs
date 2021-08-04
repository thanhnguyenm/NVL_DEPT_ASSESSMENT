using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AssessmentCriteriaAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.DepartmentMatrixAggregate;

namespace Nova.DeptServiceAssesment.Infrastructure.EntityConfigurations
{
    class DepartmentMatrixConfiguration
        : IEntityTypeConfiguration<DepartmentMatrix>
    {
        public void Configure(EntityTypeBuilder<DepartmentMatrix> configuration)
        {
            configuration.ToTable("DepartmentMatrix");

            configuration.HasKey(b => b.Id);

            configuration.Ignore(b => b.DomainEvents);

            configuration.Property(b => b.DepartmentFrom)
                .IsRequired();

            configuration.Property(b => b.DepartmentTo)
                .IsRequired();

            configuration.Property(b => b.Interact)
                .IsRequired();

            configuration.Property(b => b.Note);
        }
    }
}

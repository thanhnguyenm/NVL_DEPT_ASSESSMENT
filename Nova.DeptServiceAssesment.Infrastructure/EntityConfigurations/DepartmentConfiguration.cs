using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;

namespace Nova.DeptServiceAssesment.Infrastructure.EntityConfigurations
{
    class DepartmentConfiguration
        : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> configuration)
        {
            configuration.ToTable("Departments");

            configuration.HasKey(b => b.Id);

            configuration.Ignore(b => b.DomainEvents);

            configuration.Property(b => b.Code)
                .IsRequired();

            configuration.HasMany(b => b.Users)
               .WithOne()
               .HasForeignKey(x => x.DepartmentCode)
               .OnDelete(DeleteBehavior.Restrict);

            var navigation = configuration.Metadata.FindNavigation(nameof(Department.Users));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}

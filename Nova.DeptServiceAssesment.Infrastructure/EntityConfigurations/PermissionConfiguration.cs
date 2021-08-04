using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;

namespace Nova.DeptServiceAssesment.Infrastructure.EntityConfigurations
{
    public class PermissionConfiguration
        : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> configuration)
        {
            configuration.ToTable("Permissions");

            configuration.HasKey(b => b.Id);

            configuration.Property(b => b.Email).IsRequired();

        }
    }
}

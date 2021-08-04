using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nova.DeptServiceAssesment.Infrastructure.EntityConfigurations
{
    public class EmailConfiguration
        : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> configuration)
        {
            configuration.ToTable("Emails");

            configuration.HasKey(b => b.Id);

            configuration.Property(b => b.To).IsRequired();
            configuration.Property(b => b.Subject).IsRequired();
            configuration.Property(b => b.Body).IsRequired();

        }
    }
}

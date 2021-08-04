using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate
{
    public class Permission : IAggregateRoot
    {
        public int Id { get; set; }
        public string Email { get; set; }
    }
}

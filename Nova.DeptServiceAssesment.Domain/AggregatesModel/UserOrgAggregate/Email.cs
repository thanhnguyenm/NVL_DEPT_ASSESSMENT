using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate
{
    public class Email : Entity
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.ExternalModel
{
    [DataContract]
    public class OrgDepartmentModel
    {
        [DataMember(Name = "Id")]
        public string Id { get; set; }
        [DataMember(Name = "ShortCode")]
        public string ShortCode { get; set; }
        [DataMember(Name = "Code")]
        public string Code { get; set; }
        [DataMember(Name = "Name")]
        public string Name { get; set; }
        [DataMember(Name = "Type")]
        public string Type { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.ExternalModel
{
    [DataContract]
    public class OrgUserModel
    {
        [DataMember(Name = "OrgUserCode")]
        public string OrgUserCode { get; set; }

        [DataMember(Name = "FullName")]
        public string FullName { get; set; }

        [DataMember(Name = "OrgUserName")]
        public string OrgUserName { get; set; }

        [DataMember(Name = "DepartmentCode")]
        public string DepartmentCode { get; set; }

        [DataMember(Name = "JobTitle")]
        public string JobTitle { get; set; }

        [DataMember(Name = "Gender")]
        public string Gender { get; set; }

        [DataMember(Name = "Email")]
        public string Email { get; set; }

        [DataMember(Name = "JobLevel")]
        public string JobLevel { get; set; }

        [DataMember(Name = "DateOfBirth")]
        public string DateOfBirth { get; set; }

        [DataMember(Name = "CompanyCode")]
        public string CompanyCode { get; set; }

        [DataMember(Name = "LocationCode")]
        public string LocationCode { get; set; }

        [DataMember(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [DataMember(Name = "IsManager")]
        public string IsManager { get; set; }

    }
}

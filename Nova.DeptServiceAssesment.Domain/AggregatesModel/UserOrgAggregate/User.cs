using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate
{
    public class User : Entity
    {
        public string OrgUserCode { get; private set; }
        public string FullName { get; private set; }
        public string OrgUserName { get; private set; }
        public int DepartmentCode { get; private set; }
        public string JobTitle { get; private set; }
        public string Gender { get; private set; }
        public string Email { get; private set; }
        public int JobLevel { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string CompanyCode { get; private set; }
        public string LocationCode { get; private set; }
        public string PhoneNumber { get; private set; }
        public string IsManager { get; private set; }

        protected User()
        {
            SetCreateInfo(0);
        }

        public User(string orgUserCode, string fullname, string orgName, string departmentCode, string jobTitle,
                            string gender, string email, string joblevel, DateTime dateOfBirth, string companyCode, string locationCode,
                            string phoneNumber, string isManager, int userid) : this()
        {
            OrgUserCode = !string.IsNullOrEmpty(orgUserCode) ? orgUserCode : throw new Exception(nameof(OrgUserCode));
            FullName = !string.IsNullOrEmpty(fullname) ? fullname : throw new Exception(nameof(FullName));
            OrgUserName = !string.IsNullOrEmpty(orgName) ? orgName : throw new Exception(nameof(OrgUserName));
            DepartmentCode = int.Parse(departmentCode);
            JobTitle = jobTitle;
            Gender = gender;
            Email = !string.IsNullOrEmpty(email) ? email : throw new Exception(nameof(Email));

            int level = 0;
            JobLevel = int.TryParse(joblevel, out level)? level : 0;
            DateOfBirth = dateOfBirth;
            CompanyCode = companyCode;
            LocationCode = locationCode ;
            PhoneNumber = phoneNumber;
            IsManager = isManager;
            
            SetCreateInfo(userid);
        }

        public void Update(string orgUserCode, string fullname, string orgName, int departmentCode, string jobTitle,
                            string gender, string email, int joblevel, DateTime dateOfBirth, string companyCode, string locationCode,
                            string phoneNumber, string isManager, int userid)
        {
            OrgUserCode = !string.IsNullOrEmpty(orgUserCode) ? orgUserCode : throw new Exception(nameof(OrgUserCode));
            FullName = !string.IsNullOrEmpty(fullname) ? fullname : throw new Exception(nameof(FullName));
            OrgUserName = !string.IsNullOrEmpty(orgName) ? orgName : throw new Exception(nameof(orgName));
            DepartmentCode = departmentCode;
            JobTitle = jobTitle;
            Gender = gender;
            Email = !string.IsNullOrEmpty(email) ? email : throw new Exception(nameof(Email));

            JobLevel = joblevel;
            DateOfBirth = dateOfBirth;
            CompanyCode = companyCode;
            LocationCode = locationCode;
            PhoneNumber = phoneNumber;
            IsManager = isManager;

            SetModifiedInfo(userid);
        }

        public bool IsEqualTo(string email)
        {
            return email == Email;
        }
    }
}

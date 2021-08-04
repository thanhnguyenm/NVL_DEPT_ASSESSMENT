using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate
{
    public class Department : Entity, IAggregateRoot
    {
        public string ShortCode { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public string DivCode { get; private set; }
        public string DivName { get; private set; }
        public string EmailHead { get; private set; }

        private List<User> _users;

        public IEnumerable<User> Users => _users;

        protected Department()
        {
            _users = new List<User>();

            SetCreateInfo(0);
        }

        public Department(int id, string code, string name, string type, string divCode, string divName, string emailhead, int userid) : this()
        {
            Id = id;
            Code = !string.IsNullOrEmpty(code) ? code : throw new Exception(nameof(Code));
            Name = code;
            Type = type;
            DivCode = FormatDepartmentCode(divCode);
            DivName = divName;
            EmailHead = emailhead;

            ShortCode = FormatDepartmentCode(Code);
            SetCreateInfo(userid);
        }

        public void Update(string code, string name, string type, string divCode, string divName, string emailhead, int userid)
        {
            Code = !string.IsNullOrEmpty(code) ? code : throw new Exception(nameof(Code));
            Name = code;
            Type = type;
            DivCode = !string.IsNullOrEmpty(divCode) ? FormatDepartmentCode(divCode) : DivCode;
            DivName = !string.IsNullOrEmpty(divName) ? divName : DivName;
            EmailHead = !string.IsNullOrEmpty(emailhead) ? emailhead : EmailHead;

            ShortCode = FormatDepartmentCode(Code);
            SetModifiedInfo(userid);
        }


        public User AddUser(string orgCode, string fullname, string orgName, string departmentCode, string jobTitle,
                            string gender, string email, string joblevel, DateTime dateOfBirth, string companyCode, string locationCode,
                            string phoneNumber, string isManager, int userid)
        {
            var existingUser = _users.SingleOrDefault(p => p.IsEqualTo(email));

            if (existingUser != null)
            {
                //AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

                return existingUser;
            }

            var user = new User(orgCode, fullname, orgName, departmentCode, jobTitle,
                            gender, email, joblevel, dateOfBirth, companyCode, locationCode,
                            phoneNumber, isManager, userid);

            _users.Add(user);

            //AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

            SetModifiedInfo(userid);

            return user;
        }


        private string FormatDepartmentCode(string unFormatedCode)
        {
            if (unFormatedCode.IndexOf("(") != -1 || unFormatedCode.IndexOf(")") != -1)
            {
                int start = unFormatedCode.LastIndexOf('(');
                int end = unFormatedCode.LastIndexOf(')');
                unFormatedCode = unFormatedCode.Substring(start, end - start + 1).Replace("(", "").Replace(")", "").Replace(" - ", "-");
            }

            return unFormatedCode;
        }

    }
}

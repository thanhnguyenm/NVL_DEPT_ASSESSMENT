using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models
{
    public class AdminUserModel
    {
        public string Id { get; set; }
        public string OrgUserCode { get; set; }
        public string FullName { get; set; }
        public string OrgUserName { get; set; }
        public int DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string Email { get; set; }
    }
}

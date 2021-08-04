using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models
{
    public class UserPeriodDepartmentModel
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool Finished { get; set; }
        public string Status { get; set; }
    }

    public class AdminPeriodDepartmentModel
    {
        public int Id { get; set; }
        public int DepartmentFromId { get; set; }
        public string DepartmentFromName { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int DepartmentToId { get; set; }
        public string DepartmentToName { get; set; }
        public string Status { get; set; }
    }
}

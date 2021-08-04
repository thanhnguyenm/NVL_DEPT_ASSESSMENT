using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models
{
    public class AdminDepartmentModel
    {
        public int Id{ get; set; }
        public string ShortCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DivCode { get; set; }
        public string DivName { get; set; }
        public string EmailHead { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models
{
    public class IdentityModel
    {
        public IdentityModel()
        {
            Email = string.Empty;
            Name = string.Empty;
            Issuer = string.Empty;
            Audiance = string.Empty;
            Tenantid = string.Empty;
        }

        public string Email { get; set; }
        public string Name { get; set; }
        public string Issuer { get; set; }
        public string Audiance { get; set; }
        public string Tenantid { get; set; }
    }
}

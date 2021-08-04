using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Configuration
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public string SeedData { get; set; }
        public AzureAdOptions AzureAd { get; set; }
        public OrgConfigOptions OrgConfig { get; set; }
        public Smtp Smtp { get; set; }

        public string NumOfDepartmentsSelected { get; set; }
        public string NumOfUserSelected { get; set; }
        public string HostUrl { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnectionString { get; set; }
    }

    public class AzureAdOptions
    {
        public string Instance { get; set; }
        public string Authority { get; set; }
        public string Domain { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string CallbackPath { get; set; }
        public string ClientSecret { get; set; }
    }

    public class Smtp
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool IsTest { get; set; }
        public string EmailTest { get; set; }
    }

    public class OrgConfigOptions
    {
        public string Api { get; set; }
        public string ApiMethod { get; set; }
        public string ApiCode { get; set; }
        public string TypeNV { get; set; }
        public string TypePB { get; set; }
        public string TypeCT { get; set; }
        public string TypeDD { get; set; }
        
    }
}

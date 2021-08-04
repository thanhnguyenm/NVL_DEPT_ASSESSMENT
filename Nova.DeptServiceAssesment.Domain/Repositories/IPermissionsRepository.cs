using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.Domain.Repositories
{
    public interface IPermissionsRepository : IRepository<Permission>
    {
        Task<List<Permission>> GetPermissionsAsync();
        Task<Permission> GetPermissionAsync(string email);
        Task<List<Permission>> GetPermissionsAsync(List<string> emails);
        Permission AddPermission(Permission p);
        void DeletePermission(Permission p);
        Task<List<Email>> GetNewEmailsAsync();
        Email AddEmail(Email p);
        void UpdateEmail(Email result);
    }
}

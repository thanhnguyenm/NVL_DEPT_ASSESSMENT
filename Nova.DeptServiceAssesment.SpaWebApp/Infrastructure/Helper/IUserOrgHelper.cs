using Nova.DeptServiceAssesment.Domain.ExternalModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper
{
    public interface IUserOrgHelper
    {
        string FormatDepartmentCode(string unFormatedCode);
        List<OrgDepartmentModel> GetDepartment();
        Task<List<OrgDepartmentModel>> GetDepartmentAsync();
        List<OrgUserModel> GetUsers();
        Task<List<OrgUserModel>> GetUsersAsync();

    }
}

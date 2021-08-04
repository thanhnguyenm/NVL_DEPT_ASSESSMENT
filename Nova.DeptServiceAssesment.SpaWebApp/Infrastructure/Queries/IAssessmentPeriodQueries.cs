using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Queries
{
    public interface IAssessmentPeriodQueries
    {
        Task<PagedList<UserPeriodModel>> GetUserAssessmentPeriodsAsync(int userid, int activePage, int itemsPerPage);
        Task<PagedList<UserPeriodDepartmentModel>> GetUserAssessmentPeriodDepartment(int userid, int periodid);
        Task<PagedList<UserAssessmentQuestionModel>> GetUserAssessmentQuestions(int userid, int periodid, int departmentId);
    }
}

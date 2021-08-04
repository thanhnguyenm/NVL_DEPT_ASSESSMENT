using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Queries
{
    public interface IAdminSetupQueries
    {
        Task<PagedList<AdminQuestionModel>> GetAdminQuestionsAsync(int activePage, int itemsPerPage);
        Task<AdminQuestionModel> GetAdminQuestionByIdAsync(int Id);
        Task<PagedList<QuestionCriteriaModel>> GetQuestionCriteriaAsync();
        Task<DataTable> GetDepartmentMatrixAsync();
        Task<PagedList<AdminPeriodModel>> GetAdminAssessmentPeriodsAsync(int activePage, int itemsPerPage);
        Task<AdminPeriodModel> GetAdminPeriodByIdAsync(int Id);
        Task<List<int>> GetAdminQuestionsPeriod(int Id);
        Task<PagedList<AdminPeriodDepartmentModel>> GetAdminPeriodDepartmentsAsync(int periodid, int activePage, int itemsPerPage);
        Task<DataTable> GetReports(int reportType, int periodId, int deptid);
        Task<PagedList<AdminDepartmentModel>> GetAdminDepartmentsAsync(string q, int activePage, int itemsPerPage);
        Task<PagedList<AdminUserModel>> GetAdminUsersAsync(string q, int activePage, int itemsPerPage);
        Task<PagedList<PermissionModel>> GetPermissions(int activePage, int itemsPerPage);
        Task<bool> CheckPermission(string email);
        Task<List<ReminderModel>> GetUserPending();
        Task<List<ReminderModel>> GetUserPendingByPeriod(int Id);
        Task<DataTable> GetResultByDepartmentTo(int periodId, int deptToId);
    }
}

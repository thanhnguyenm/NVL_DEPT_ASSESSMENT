using Nova.DeptServiceAssesment.Domain.AggregatesModel.AssessmentCriteriaAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.DepartmentMatrixAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.Domain.Repositories
{
    public interface IAssessmentCriteriaRepository : IRepository<AssessmentCriteria>
    {
        Task<AssessmentCriteria> GetCriteriaAsync(int id);

        AssessmentCriteria AddAssessmentCriteria(AssessmentCriteria result);

        void UpdateAssessmentCriteria(AssessmentCriteria result);

        Task SaveDepartmentMatrix(List<DepartmentMatrix> matrix);

        Task<List<DepartmentMatrix>> GetDepartmentMatrix();


        void UpdateDepartmentMatrix(List<DepartmentMatrix> maxtrix);

        Task<List<AssessmentQuestion>> GetQuestionsAsync();

        Task<List<Department>> GetDepartmentsFromDB();

        Task<Department> GetDepartmentsByIdFromDB(int id);

        Task<List<User>> GetUsersFromDB();

        Task<List<User>> GetActiveUsersFromDB();

        Task<List<User>> GetActiveUsersFromDB(int level);

        Task<User> GetActiveUsersFromDB(string email);

        Task<List<User>> GetUsersFromDB(List<int> userids);

        Task AddDepartments(List<Department> departments);

        void UpdateDepartments(List<Department> departments);

        Task AddUsers(List<User> users);

        void UpdateUsers(List<User> users);
    }
}

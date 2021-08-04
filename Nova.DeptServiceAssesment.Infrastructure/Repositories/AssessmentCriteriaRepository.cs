using Microsoft.EntityFrameworkCore;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AssessmentCriteriaAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.DepartmentMatrixAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.Infrastructure.Repositories
{
    public class AssessmentCriteriaRepository : IAssessmentCriteriaRepository
    {
        private readonly AssessmentContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }


        public AssessmentCriteriaRepository(AssessmentContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<AssessmentCriteria> GetCriteriaAsync(int id)
        {
            var period = await _context
                                .AssessmentCriteria
                                .Include(x => x.AssessmentQuestions)
                                .FirstOrDefaultAsync(o => o.Id == id);


            return period;
        }

        public async Task<List<AssessmentQuestion>> GetQuestionsAsync()
        {
            var questions = await _context
                                .AssessmentQuestions.ToListAsync();

            return questions;
        }

        public async Task<AssessmentCriteria> SaveCriteriaAsync(int id)
        {
            var period = await _context
                                .AssessmentCriteria
                                .Include(x => x.AssessmentQuestions)
                                .FirstOrDefaultAsync(o => o.Id == id);


            return period;
        }

        public AssessmentCriteria AddAssessmentCriteria(AssessmentCriteria result)
        {
            return _context.AssessmentCriteria.Add(result).Entity;
        }

        public void UpdateAssessmentCriteria(AssessmentCriteria result)
        {
            _context.Entry(result).State = EntityState.Modified;
        }


        public async Task<List<DepartmentMatrix>> GetDepartmentMatrix()
        {
            var matrices = await _context
                                .DepartmentMatrix.ToListAsync();
                                


            return matrices;
        }

        public void UpdateDepartmentMatrix(List<DepartmentMatrix> maxtrix)
        {
            foreach(var m in maxtrix)
            {
                _context.Entry(m).State = EntityState.Modified;
            }
        }

        public async Task SaveDepartmentMatrix(List<DepartmentMatrix> matrix)
        {
            await _context.DepartmentMatrix.AddRangeAsync(matrix);

        }

        public async Task<List<Department>> GetDepartmentsFromDB()
        {
            var depts = await _context
                                .Departments.ToListAsync();

            return depts;
        }

        public async Task<Department> GetDepartmentsByIdFromDB(int id)
        {
            var dept = await _context
                                .Departments.SingleOrDefaultAsync(x=>x.Id == id);

            return dept;
        }

        public async Task<List<User>> GetUsersFromDB()
        {
            var users = await _context
                                .Users.ToListAsync();

            return users;
        }

        public async Task<List<User>> GetActiveUsersFromDB()
        {
            var users = await _context
                                .Users.Where(x => !x.Deleted).ToListAsync();

            return users;
        }

        public async Task<List<User>> GetActiveUsersFromDB(int level)
        {
            var users = await _context
                                .Users.Where(x => x.JobLevel >= level && !x.Deleted).ToListAsync();

            return users;
        }

        public async Task<User> GetActiveUsersFromDB(string email)
        {
            var user = await _context
                                .Users.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower() && !x.Deleted);

            return user;
        }

        public async Task<List<User>> GetUsersFromDB(List<int> userids)
        {
            var users = await _context
                                .Users.Where(x => userids.Contains(x.Id)).ToListAsync();

            return users;
        }

        public async Task AddDepartments(List<Department> departments)
        {
            await _context.Departments.AddRangeAsync(departments);
        }

        public void UpdateDepartments(List<Department> departments)
        {
            foreach(var dept in departments)
            {
                _context.Entry(dept).State = EntityState.Modified;
            }
        }

        public async Task AddUsers(List<User> users)
        {
            await _context.Users.AddRangeAsync(users);
        }

        public void UpdateUsers(List<User> users)
        {
            foreach (var user in users)
            {
                _context.Entry(user).State = EntityState.Modified;
            }
        }
    }
}

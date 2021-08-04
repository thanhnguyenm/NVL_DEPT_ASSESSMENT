using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models;
using System.Linq;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Queries
{
    public class AssessmentPeriodQueries : IAssessmentPeriodQueries
    {
        private string _connectionString = string.Empty;

        public AssessmentPeriodQueries(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }

        public async Task<PagedList<UserPeriodModel>> GetUserAssessmentPeriodsAsync(int userid, int activePage, int itemsPerPage)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //count first
                var totalRecords = await connection.ExecuteScalarAsync<int>(
                        @"select count(*) 
                        FROM (select distinct p.Id
                            FROM AssessmentPeriods p 
                            INNER JOIN PeriodSelectedDepartments dp ON p.Id = dp.AssessmentPeriodId
                            WHERE p.Published=1 AND p.Deleted=0 AND dp.Deleted=0 AND dp.UserId=@userid) as a"
                        , new { userid }
                    );

                //then get data
                var pager = new Pager(activePage, itemsPerPage);
                var result = await connection.QueryAsync<UserPeriodModel>(
                        @"select distinct p.Id, p.PeriodName, p.PeriodFrom, p.PeriodTo, 
                                    IIF(getdate() < p.PeriodFrom,'New',IIF(p.PeriodFrom <= getdate() and getdate() <= p.PeriodTo, 'In-progress', 'End')) as Status,
                                    p.Note
                            FROM AssessmentPeriods p 
                            INNER JOIN PeriodSelectedDepartments dp ON p.Id = dp.AssessmentPeriodId
                            WHERE p.Published=1 AND p.Deleted=0 AND dp.Deleted=0 AND dp.UserId=@userid
                            ORDER BY p.Id DESC
                            OFFSET @Offset ROWS 
                            FETCH NEXT @Next ROWS ONLY
                        "
                        , new { userid, pager.Offset, pager.Next }
                    );

                if (result.AsList().Count == 0)
                    new PagedList<UserPeriodModel>(new List<UserPeriodModel>(), totalRecords, activePage, itemsPerPage);

                return new PagedList<UserPeriodModel>(result, totalRecords, activePage, itemsPerPage);
            }
        }


        public async Task<PagedList<UserPeriodDepartmentModel>> GetUserAssessmentPeriodDepartment(int userid, int periodid)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

             
                //then get data
                var result = await connection.QueryAsync<UserPeriodDepartmentModel>(
                        @"select dp.Id, dp.DepartmentTo as DepartmentId, dp.Finished, d.Code as DepartmentName,
						    IIF((SELECT COUNT(IIF(Result is not null, 1, null)) FROM PeriodAssessmentResults rs WHERE rs.Deleted=0 AND rs.PeriodSelectedDepartmentId=dp.Id)=0,'NOTSTART',
						    IIF((SELECT COUNT(IIF(Result is not null, 1, null)) FROM PeriodAssessmentResults rs WHERE rs.Deleted=0 AND rs.PeriodSelectedDepartmentId=dp.Id)=
								    (SELECT COUNT(*) FROM PeriodQuetions qs WHERE qs.AssessmentPeriodId=p.Id AND qs.Deleted=0),'COMPLETE', 'PENDING')) As Status
                            FROM AssessmentPeriods p 
                            INNER JOIN PeriodSelectedDepartments dp ON p.Id = dp.AssessmentPeriodId
                            INNER JOIN Departments d ON d.Id = dp.DepartmentTo
                            WHERE p.Published=1 AND p.Deleted=0 AND dp.Deleted=0 AND dp.UserId=@userid AND p.Id=@periodid
                            ORDER BY p.Id DESC, dp.Id ASC
                        "
                        , new { userid, periodid }
                    );

                if (result.AsList().Count == 0)
                    new PagedList<UserPeriodModel>(new List<UserPeriodModel>(), 0, 0, 0);

                return new PagedList<UserPeriodDepartmentModel>(result.AsEnumerable(), 0, 0, 0);
            }
        }

        public async Task<PagedList<UserAssessmentQuestionModel>> GetUserAssessmentQuestions(int userid, int periodid, int departmentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //then get data
                var result = await connection.QueryAsync<UserAssessmentQuestionModel>(
                        @"select A.PeriodFrom, A.PeriodTo, B.ResultId, A.PeriodSelectedDepartmentId, A.PeriodQuetionId, A.QuestionContent, A.CriteriaId, A.CriteriaName, B.Result, B.ResultComment, IIF(B.Finished is null, 0, B.Finished) as Finished
                            FROM 
                            (SELECT distinct p1.PeriodFrom, p1.PeriodTo, pq1.Id as PeriodQuetionId, qs1.Content as QuestionContent, qs1.CriteriaId, ca.CriteriaName, dp1.Id as PeriodSelectedDepartmentId
                            FROM AssessmentPeriods p1 
                            INNER JOIN PeriodQuetions pq1 ON p1.Id = pq1.AssessmentPeriodId
                            INNER JOIN AssessmentQuestions qs1 ON qs1.Id = pq1.AssessmentQuestionId
                            INNER JOIN AssessmentCriteria ca ON ca.Id = qs1.CriteriaId
                            INNER JOIN PeriodSelectedDepartments dp1 ON p1.Id = dp1.AssessmentPeriodId
                            WHERE p1.Published=1 AND p1.Deleted=0 AND pq1.Deleted=0 AND qs1.Deleted=0 AND dp1.UserId=@userid AND p1.Id=@periodid AND dp1.Id=@departmentId) AS A
                            LEFT JOIN 
                            (SELECT distinct rs.Id as ResultId, dp.Id as PeriodSelectedDepartmentId, rs.PeriodQuestionId, rs.Result, rs.ResultComment, dp.Finished
                            FROM AssessmentPeriods p 
                            INNER JOIN PeriodSelectedDepartments dp ON p.Id = dp.AssessmentPeriodId
                            INNER JOIN PeriodAssessmentResults rs ON dp.Id = rs.PeriodSelectedDepartmentId
                            WHERE p.Published=1 AND p.Deleted=0 AND dp.Deleted=0 AND rs.Deleted=0 AND dp.UserId=@userid AND p.Id=@periodid AND dp.Id=@departmentId) AS B
                            ON A.PeriodQuetionId = B.PeriodQuestionId
                            ORDER BY A.CriteriaId, A.PeriodQuetionId
                        "
                        , new { userid, periodid, departmentId }
                    );

                if (result.AsList().Count == 0)
                    new PagedList<UserAssessmentQuestionModel>(new List<UserAssessmentQuestionModel>(), 0, 0, 0);

                foreach(var rs in result)
                {
                    rs.CanEdit = rs.PeriodFrom.Date <= DateTime.Today && DateTime.Today <= rs.PeriodTo.Date && !rs.Finished;
                }

                return new PagedList<UserAssessmentQuestionModel>(result.AsEnumerable(), 0, 0, 0);
            }
        }

    }
}

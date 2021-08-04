using Dapper;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Queries
{
    public class AdminSetupQueries : IAdminSetupQueries
    {
        private string _connectionString = string.Empty;

        public AdminSetupQueries(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }


        public async Task<PagedList<AdminQuestionModel>> GetAdminQuestionsAsync(int activePage, int itemsPerPage)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //count first
                var totalRecords = await connection.ExecuteScalarAsync<int>(@"select count(*) FROM AssessmentQuestions q");

                //then get data
                var pager = new Pager(activePage, itemsPerPage);
                var result = await connection.QueryAsync<AdminQuestionModel>(
                        @"select q.Id, q.CriteriaId, c.CriteriaName, q.Content
                            FROM AssessmentQuestions q 
                            INNER JOIN AssessmentCriteria c ON c.Id = q.CriteriaId
                            ORDER BY q.Id DESC
                            OFFSET @Offset ROWS 
                            FETCH NEXT @Next ROWS ONLY
                        "
                        , new { pager.Offset, pager.Next }
                    );

                if (result.AsList().Count == 0)
                    new PagedList<AdminQuestionModel>(new List<AdminQuestionModel>(), totalRecords, activePage, itemsPerPage);

                return new PagedList<AdminQuestionModel>(result, totalRecords, activePage, itemsPerPage);
            }
        }

        public async Task<AdminQuestionModel> GetAdminQuestionByIdAsync(int Id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //then get data
                var result = await connection.QueryFirstOrDefaultAsync<AdminQuestionModel>(
                        @"select q.Id, q.CriteriaId, c.CriteriaName, q.Content
                            FROM AssessmentQuestions q 
                            INNER JOIN AssessmentCriteria c ON c.Id = q.CriteriaId
                            WHERE q.Id=@Id
                        "
                        , new { Id }
                    );

                if (result == null)
                    throw new ArgumentNullException();

                return result;
            }
        }

        public async Task<PagedList<QuestionCriteriaModel>> GetQuestionCriteriaAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //then get data
                var result = await connection.QueryAsync<QuestionCriteriaModel>(
                        @"select Id, CriteriaName
                            FROM AssessmentCriteria 
                            WHERE deleted=0
                            ORDER BY Id ASC
                        "
                    );

                if (result.AsList().Count == 0)
                    new PagedList<QuestionCriteriaModel>(new List<QuestionCriteriaModel>(), 0, 0, 0);

                return new PagedList<QuestionCriteriaModel>(result, 0, 0, 0);
            }
        }

        public DataTable ToDataTable(IEnumerable<dynamic> items)
        {
            if (items == null) return null;
            var data = items.ToArray();
            if (data.Length == 0) return null;

            var dt = new DataTable();
            foreach (var pair in ((IDictionary<string, object>)data[0]))
            {
                dt.Columns.Add(pair.Key, (pair.Value ?? string.Empty).GetType());
            }
            foreach (var d in data)
            {
                dt.Rows.Add(((IDictionary<string, object>)d).Values.ToArray());
            }
            return dt;
        }

        public async Task<DataTable> GetDepartmentMatrixAsync()
        {
            using (var cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                var obs = await cnn.QueryAsync(sql: "SP_DepartmentMatrix", commandType: CommandType.StoredProcedure);

                var dt = ToDataTable(obs);
                return dt;
            }
        }

        public async Task<PagedList<AdminPeriodModel>> GetAdminAssessmentPeriodsAsync(int activePage, int itemsPerPage)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //count first
                var totalRecords = await connection.ExecuteScalarAsync<int>(
                        @"select count(*) 
                        FROM (select distinct p.Id
                            FROM AssessmentPeriods p 
                            WHERE p.Deleted=0) as a"
                    );

                //then get data
                var pager = new Pager(activePage, itemsPerPage);
                var result = await connection.QueryAsync<AdminPeriodModel>(
                        @"select distinct p.Id, p.PeriodName, p.PeriodFrom, p.PeriodTo, p.Published
                            FROM AssessmentPeriods p 
                            WHERE p.Deleted=0
                            ORDER BY p.Id DESC
                            OFFSET @Offset ROWS 
                            FETCH NEXT @Next ROWS ONLY
                        "
                        , new { pager.Offset, pager.Next }
                    );

                if (result.AsList().Count == 0)
                    new PagedList<AdminPeriodModel>(new List<AdminPeriodModel>(), totalRecords, activePage, itemsPerPage);

                return new PagedList<AdminPeriodModel>(result, totalRecords, activePage, itemsPerPage);
            }
        }

        public async Task<AdminPeriodModel> GetAdminPeriodByIdAsync(int Id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //then get data
                var result = await connection.QueryFirstOrDefaultAsync<AdminPeriodModel>(
                        @"select q.Id, q.PeriodName, q.PeriodFrom, q.PeriodTo, q.Published
                            FROM AssessmentPeriods q
                            WHERE q.Deleted=0 and q.Id = @Id
                        "
                        , new { Id }
                    );

                if (result == null)
                    throw new ArgumentNullException();

                return result;
            }
        }

        public async Task<List<int>> GetAdminQuestionsPeriod(int Id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //then get data
                var result = await connection.QueryAsync<int>(
                        @"select q.Id
                            FROM AssessmentQuestions q
                            INNER JOIN PeriodQuetions pq ON q.Id = pq.AssessmentQuestionId
                            WHERE q.Deleted=0 and pq.Deleted=0 and pq.AssessmentPeriodId = @Id
                        "
                        , new { Id }
                    );

                if (result == null)
                    return new List<int>();

                return result.ToList();
            }
        }


        public async Task<PagedList<AdminPeriodDepartmentModel>> GetAdminPeriodDepartmentsAsync(int Id, int activePage, int itemsPerPage)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //count first
                var totalRecords = await connection.ExecuteScalarAsync<int>(
                        @"select count(dp.Id)
                            FROM AssessmentPeriods p 
                            INNER JOIN PeriodSelectedDepartments dp ON p.Id = dp.AssessmentPeriodId
                            WHERE p.Id=@Id
                        "
                        , new { Id }
                    );

                //then get data
                var pager = new Pager(activePage, itemsPerPage);
                var result = await connection.QueryAsync<AdminPeriodDepartmentModel>(
                        @"select distinct dp.Id, d.Name as DepartmentFromName, u.FullName, d1.Name as DepartmentToName
                            FROM AssessmentPeriods p 
                            INNER JOIN PeriodSelectedDepartments dp ON p.Id = dp.AssessmentPeriodId
                            INNER JOIN Departments d ON d.Id = dp.DepartmentFrom
                            INNER JOIN Users u ON u.Id = dp.UserId
                            INNER JOIN Departments d1 ON d1.Id = dp.DepartmentTo
                            WHERE p.Deleted=0 AND dp.Deleted=0 AND p.Id=@Id
                            ORDER BY dp.Id ASC
                            OFFSET @Offset ROWS 
                            FETCH NEXT @Next ROWS ONLY
                        "
                        , new { Id, pager.Offset, pager.Next }
                    );

                if (result.AsList().Count == 0)
                    new PagedList<AdminPeriodDepartmentModel>(new List<AdminPeriodDepartmentModel>(), totalRecords, activePage, itemsPerPage);

                return new PagedList<AdminPeriodDepartmentModel>(result, totalRecords, activePage, itemsPerPage);
            }
        }

        public async Task<DataTable> GetReports(int reportType, int periodId, int deptid)
        {
            if(reportType == 1)
                return await GetReport1(periodId);
            else if (reportType == 2)
                return await GetReportCommon(periodId, reportType);
            else if (reportType == 3)
                return await GetReport3(periodId);
            else if (reportType == 5)
                return await GetReport5(periodId, deptid);
            else if (reportType == 6)
                return await GetReportCommon(periodId, reportType);
            else
                return await GetReportCommon(periodId, reportType);
        }

        private async Task<DataTable> GetReport1(int periodId)
        {
            using (var cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                var p = new DynamicParameters();
                p.Add("@PeriodId", periodId);

                var items = await cnn.QueryAsync(sql: "SP_REPORT_1", param: p, commandType: CommandType.StoredProcedure);

                if (items == null) return null;
                var data = items.ToArray();
                if (data.Length == 0) return null;

                var dt = new DataTable();
                foreach (var pair in ((IDictionary<string, object>)data[0]))
                {
                    dt.Columns.Add(pair.Key, (pair.Value ?? string.Empty).GetType());
                }
                foreach (var d in data)
                {
                    var dictionary = (IDictionary<string, object>)d;
                    float sum = 0;
                    int count = 0;
                    foreach(var dicItem in dictionary)
                    {
                        float score = 0;
                        if (dicItem.Key.Contains("QUES_") && dicItem.Value != null && float.TryParse(dicItem.Value.ToString(), out score))
                        {
                            sum += score;
                            count++;
                        }
                    }
                    dictionary["Tong Diem"] = sum;
                    dictionary["Tong Binh Quan"] = count == 0 ? 0 : sum / (float)count;

                    var arrayItems = dictionary.Values.ToArray();
                    dt.Rows.Add(arrayItems);
                }

                int colNums = dt.Columns.Count;
                dt.Columns["DepartmentId"].SetOrdinal(0);
                dt.Columns["DepartmentName"].SetOrdinal(1);
                dt.Columns["DivCode"].SetOrdinal(2);
                dt.Columns["DivName"].SetOrdinal(3);
                dt.Columns["Tong Binh Quan"].SetOrdinal(colNums - 1);
                dt.Columns["Tong Diem"].SetOrdinal(colNums - 2);

                int firstColNums = 5;
                List<int> lstQuestions = new List<int>();
                for (int colIndex = 0;  colIndex < colNums; colIndex++)
                {
                    if (dt.Columns[colIndex].ColumnName.Contains("QUES_"))
                    {
                        int numOfQuestion = int.Parse(dt.Columns[colIndex].ColumnName.Replace("QUES_", ""));
                        if (lstQuestions.Count == 0)
                        {
                            lstQuestions.Add(numOfQuestion);
                        }
                        else
                        {
                            int insertPos = -1;
                            for (int j = 0; j < lstQuestions.Count; j++)
                            {
                                if (lstQuestions[j] > numOfQuestion)
                                {
                                    insertPos = j;
                                    break;
                                }
                            }
                            if (insertPos != -1) lstQuestions.Insert(insertPos, numOfQuestion);
                            else lstQuestions.Add(numOfQuestion);
                        }

                    }
                }

                for (int j = 0; j < lstQuestions.Count; j++)
                {
                    dt.Columns[$"QUES_{lstQuestions[j]}"].SetOrdinal(firstColNums + j);
                    dt.Columns[$"QUES_{lstQuestions[j]}"].ColumnName = $"Câu hỏi {j + 1}";
                }

                return dt;

            }
        }

        private async Task<DataTable> GetReport2(int periodId)
        {
            using (var cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                var p = new DynamicParameters();
                p.Add("@PeriodId", periodId);

                var items = await cnn.QueryAsync(sql: "SP_REPORT_2", param: p, commandType: CommandType.StoredProcedure);

                var dt = ToDataTable(items);
                return dt;

            }
        }

        private async Task<DataTable> GetReport3(int periodId)
        {
            using (var cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                var p = new DynamicParameters();
                p.Add("@PeriodId", periodId);

                var items = await cnn.QueryAsync(sql: "SP_REPORT_3", param: p, commandType: CommandType.StoredProcedure);

                if (items == null) return null;
                var data = items.ToArray();
                if (data.Length == 0) return null;

                var dt = new DataTable();
                foreach (var pair in ((IDictionary<string, object>)data[0]))
                {
                    dt.Columns.Add(pair.Key, (pair.Value ?? string.Empty).GetType());
                }
                foreach (var d in data)
                {
                    var dictionary = (IDictionary<string, object>)d;
                    float sum = 0;
                    int count = 0;
                    foreach (var dicItem in dictionary)
                    {
                        float score = 0;
                        if (dicItem.Key.Contains("QUES_") && dicItem.Value != null && float.TryParse(dicItem.Value.ToString(), out score))
                        {
                            sum += score;
                            count++;
                        }
                    }
                    dictionary["Tong Diem"] = sum;
                    dictionary["Tong Binh Quan"] = count == 0 ? 0 : (float)sum / (float)count;

                    var arrayItems = dictionary.Values.ToArray();
                    dt.Rows.Add(arrayItems);
                }

                int colNums = dt.Columns.Count;
                dt.Columns["DepartmentId1"].SetOrdinal(0);
                dt.Columns["DepartmentName1"].SetOrdinal(1);
                dt.Columns["DepartmentId2"].SetOrdinal(2);
                dt.Columns["DepartmentName2"].SetOrdinal(3);
                dt.Columns["Tong Binh Quan"].SetOrdinal(colNums - 1);
                dt.Columns["Tong Diem"].SetOrdinal(colNums - 2);

                int firstColNums = 4;
                List<int> lstQuestions = new List<int>();
                for (int colIndex = 0; colIndex < colNums; colIndex++)
                {
                    if (dt.Columns[colIndex].ColumnName.Contains("QUES_"))
                    {
                        int numOfQuestion = int.Parse(dt.Columns[colIndex].ColumnName.Replace("QUES_", ""));
                        if (lstQuestions.Count == 0)
                        {
                            lstQuestions.Add(numOfQuestion);
                        }
                        else
                        {
                            int insertPos = -1;
                            for (int j = 0; j < lstQuestions.Count; j++)
                            {
                                if (lstQuestions[j] > numOfQuestion)
                                {
                                    insertPos = j;
                                    break;
                                }
                            }
                            if (insertPos != -1) lstQuestions.Insert(insertPos, numOfQuestion);
                            else lstQuestions.Add(numOfQuestion);
                        }

                    }
                }

                for (int j = 0; j < lstQuestions.Count; j++)
                {
                    dt.Columns[$"QUES_{lstQuestions[j]}"].SetOrdinal(firstColNums + j);
                    dt.Columns[$"QUES_{lstQuestions[j]}"].ColumnName = $"Câu hỏi {j + 1}";
                }

                return dt;

            }
        }

        private async Task<DataTable> GetReport5(int periodId, int deptid)
        {
            using (var cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                var p = new DynamicParameters();
                p.Add("@PeriodId", periodId);
                p.Add("@DepartmentId", deptid);

                var items = await cnn.QueryAsync(sql: "SP_REPORT_5", param: p, commandType: CommandType.StoredProcedure);

                if (items == null) return null;
                var data = items.ToArray();
                if (data.Length == 0) return null;

                var dt = new DataTable();
                foreach (var pair in ((IDictionary<string, object>)data[0]))
                {
                    dt.Columns.Add(pair.Key, (pair.Value ?? string.Empty).GetType());
                }
                foreach (var d in data)
                {
                    var dictionary = (IDictionary<string, object>)d;
                    int sum = 0;
                    int count = 0;
                    foreach (var dicItem in dictionary)
                    {
                        int score = 0;
                        if (dicItem.Key != "Tong Binh Quan" && dicItem.Key != "Question" && dicItem.Value != null && int.TryParse(dicItem.Value.ToString(), out score))
                        {
                            sum += score;
                            count++;
                        }
                    }
                    dictionary["Tong Binh Quan"] = count == 0 ? 0 : (float)sum / count;

                    var arrayItems = dictionary.Values.ToArray();
                    dt.Rows.Add(arrayItems);
                }

                int colNums = dt.Columns.Count;
                dt.Columns["Question"].SetOrdinal(0);
                dt.Columns["Tong Binh Quan"].SetOrdinal(colNums - 1);

                DataView dv = dt.DefaultView;
                dv.Sort = "Question asc";
                DataTable sortedDT = dv.ToTable();
                var newCols = sortedDT.Columns.Add("Câu hỏi/Tiêu chí");
                newCols.ColumnName = "Câu hỏi/Tiêu chí";
                newCols.DataType = typeof(string);
                newCols.SetOrdinal(0);
                sortedDT.Columns.Remove("Question");

                int i = 1;
                foreach(DataRow r in sortedDT.Rows)
                {
                    r[0] = $"Câu hỏi {i++}";
                }

                return sortedDT;

            }
        }

        private async Task<DataTable> GetReportCommon(int periodId, int number)
        {
            using (var cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                var p = new DynamicParameters();
                p.Add("@PeriodId", periodId);

                var items = await cnn.QueryAsync(sql: $"SP_REPORT_{number}", param: p, commandType: CommandType.StoredProcedure);

                var dt = ToDataTable(items);
                return dt;

            }
        }

        public async Task<PagedList<AdminDepartmentModel>> GetAdminDepartmentsAsync(string q, int activePage, int itemsPerPage)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                q = q == null ? string.Empty : q;
                //count first
                var totalRecords = await connection.ExecuteScalarAsync<int>(
                        q == string.Empty ?
                        @"select count(*) 
                        FROM (select distinct Id
                            FROM Departments
                            WHERE Deleted=0) as a" :

                        @"select count(*) 
                        FROM (select distinct Id
                            FROM Departments
                            WHERE Deleted=0 AND (Name like N'%" + q + "%' OR Code like N'%" + q + "%')) as a"

                    );

                //then get data
                var pager = new Pager(activePage, itemsPerPage);
                var result = await connection.QueryAsync<AdminDepartmentModel>(
                        q == string.Empty ?
                        @"select Id, ShortCode, Code, Name, Type, DivCode, DivName, EmailHead
                            FROM Departments
                            WHERE Deleted=0
                            ORDER BY Id ASC
                            OFFSET @Offset ROWS 
                            FETCH NEXT @Next ROWS ONLY
                        " :
                        @"select Id, ShortCode, Code, Name, Type, DivCode, DivName, EmailHead
                            FROM Departments
                            WHERE Deleted=0 AND (Name like N'%" + q + "%' OR Code like N'%" + q + @"%')
                            ORDER BY Id ASC
                            OFFSET @Offset ROWS 
                            FETCH NEXT @Next ROWS ONLY
                        "
                        , new { pager.Offset, pager.Next }
                    );

                if (result.AsList().Count == 0)
                    new PagedList<AdminDepartmentModel>(new List<AdminDepartmentModel>(), totalRecords, activePage, itemsPerPage);

                return new PagedList<AdminDepartmentModel>(result, totalRecords, activePage, itemsPerPage);
            }
        }

        public async Task<PagedList<AdminUserModel>> GetAdminUsersAsync(string q, int activePage, int itemsPerPage)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                q = q == null ? string.Empty : q;

                //count first
                var totalRecords = await connection.ExecuteScalarAsync<int>(
                        q == string.Empty ?
                        @"select count(*) 
                        FROM (select distinct Id
                            FROM Users
                            WHERE Deleted=0) as a" :

                        @"select count(*) 
                        FROM (select distinct Id
                            FROM Users
                            WHERE Deleted=0 AND (FullName like N'%"+q+ "%' OR OrgUserName like N'%" + q + "%' OR Email like N'%" + q + "%' )) as a"
                    );

                //then get data
                var pager = new Pager(activePage, itemsPerPage);
                var result = await connection.QueryAsync<AdminUserModel>(
                        q == string.Empty ?
                        @"select Id, OrgUserCode, FullName, OrgUserName, Email
                            FROM Users
                            WHERE  Deleted=0
                            ORDER BY Id ASC
                            OFFSET @Offset ROWS 
                            FETCH NEXT @Next ROWS ONLY
                        " :
                        @"select Id, OrgUserCode, FullName, OrgUserName, Email
                            FROM Users
                            WHERE  Deleted=0 AND (FullName like N'%" + q + "%' OR OrgUserName like N'%" + q + "%' OR Email like N'%" + q + @"%' )
                            ORDER BY Id ASC 
                            OFFSET @Offset ROWS 
                            FETCH NEXT @Next ROWS ONLY
                        "
                        , new { pager.Offset, pager.Next }
                    );

                if (result.AsList().Count == 0)
                    new PagedList<AdminUserModel>(new List<AdminUserModel>(), totalRecords, activePage, itemsPerPage);

                return new PagedList<AdminUserModel>(result, totalRecords, activePage, itemsPerPage);
            }
        }

        public async Task<PagedList<PermissionModel>> GetPermissions(int activePage, int itemsPerPage)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //count first
                var totalRecords = await connection.ExecuteScalarAsync<int>(
                        @"select count(*) 
                        FROM (select distinct Id
                            FROM Permissions ) as a"
                    );

                //then get data
                var pager = new Pager(activePage, itemsPerPage);
                var result = await connection.QueryAsync<PermissionModel>(
                        @"select Id, Email
                            FROM Permissions
                            ORDER BY Email ASC
                            OFFSET @Offset ROWS 
                            FETCH NEXT @Next ROWS ONLY
                        "
                        , new { pager.Offset, pager.Next }
                    );

                if (result.AsList().Count == 0)
                    new PagedList<PermissionModel>(new List<PermissionModel>(), totalRecords, activePage, itemsPerPage);

                return new PagedList<PermissionModel>(result, totalRecords, activePage, itemsPerPage);
            }
        }

        public async Task<bool> CheckPermission(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //then get data
                var result = await connection.QueryAsync<PermissionModel>(
                        @"select Id, Email
                            FROM Permissions
                            ORDER BY Email ASC
                        "
                    );

                if (result != null &&  result.Any(x => x.Email.ToLower() == email.ToLower())) return true;

                return false;
            }
        }

        public async Task<List<ReminderModel>> GetUserPending()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //then get data
                var result = await connection.QueryAsync<ReminderModel>(
                        @"select p.Id, p.PeriodName, p.PeriodFrom, p.PeriodTo, ur.Id as UserId, ur.Email
                            FROM AssessmentPeriods p
                            INNER JOIN PeriodSelectedDepartments dp ON p.Id = dp.AssessmentPeriodId
                            INNER JOIN Users ur ON dp.UserId = ur.Id
                            WHERE p.Deleted=0 AND p.Published=1 AND 
                                    dateadd(DD,1,Cast(GetDate() AS date)) = Cast(PeriodTo AS date)
                                    AND dp.Deleted=0 AND dp.Finished=0 

                        "
                    );

                
                return result.ToList();
            }
        }

        public async Task<DataTable> GetResultByDepartmentTo(int periodId, int deptToId)
        {

            using (var cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                var p = new DynamicParameters();
                p.Add("@PeriodId", periodId);
                p.Add("@DeptId", deptToId);

                var items = await cnn.QueryAsync(sql: "SP_REPORT_4", param: p, commandType: CommandType.StoredProcedure);

                var dt = ToDataTable(items);
                return dt;

            }

        }

        public async Task<List<ReminderModel>> GetUserPendingByPeriod(int Id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //then get data
                var result = await connection.QueryAsync<ReminderModel>(
                        @"select p.Id, p.PeriodName, p.PeriodFrom, p.PeriodTo, ur.Id as UserId, ur.Email
                            FROM AssessmentPeriods p
                            INNER JOIN PeriodSelectedDepartments dp ON p.Id = dp.AssessmentPeriodId
                            INNER JOIN Users ur ON dp.UserId = ur.Id
                            WHERE p.Deleted=0 AND p.Published=1 AND 
                                    dateadd(DD,1,Cast(GetDate() AS date)) = Cast(PeriodTo AS date)
                                    AND dp.Deleted=0 AND dp.Finished=0 
                                    AND p.Id=@Id

                        ",
                        new { Id }
                    );


                return result.ToList();
            }
        }

    }
}

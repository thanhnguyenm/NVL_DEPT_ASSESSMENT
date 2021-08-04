using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AssessmentCriteriaAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.DepartmentMatrixAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.Domain.ExternalModel;
using Nova.DeptServiceAssesment.Infrastructure;
using Nova.DeptServiceAssesment.SpaWebApp.Configuration;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Extensions;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure
{
    public class AssessmentContextSeed
    {
        public async Task SeedAsync(AssessmentContext context, IHostingEnvironment env, ILogger<AssessmentContextSeed> logger, IUserOrgHelper userService, AppSettings setting)
        {
            var contentRootPath = env.ContentRootPath;
            var picturePath = env.WebRootPath;

            if (!context.AssessmentCriteria.Any())
            {
                await context.AssessmentCriteria.AddRangeAsync(GetAssessmentCriteriaFromFile(contentRootPath, logger));

                await context.SaveChangesAsync();
            }

            if (!context.AssessmentQuestions.Any())
            {
                await context.AssessmentQuestions.AddRangeAsync(GetAssessmentQuestionFromFile(contentRootPath, logger));

                await context.SaveChangesAsync();
            }

            if (!context.DepartmentMatrix.Any())
            {
                await context.DepartmentMatrix.AddRangeAsync(GetDepartmentMatrixFromFile(contentRootPath, logger, userService));

                await context.SaveChangesAsync();
            }

            if (!context.Departments.Any())
            {
                await context.Departments.AddRangeAsync(GetDepartmentsFromServices(logger, userService));
                await context.SaveChangesAsync();

                await GetDepartmentsFromFile(context, contentRootPath, logger);
            }

            if (!context.Users.Any())
            {
                await context.Users.AddRangeAsync(GetUsersFromServices(logger, userService));

                await context.SaveChangesAsync();
            }

            if (!context.Permissions.Any())
            {
                await context.Permissions.AddAsync(new Permission { Email = "thanh.nguyenminh@novaland.com.vn" });
                await context.Permissions.AddAsync(new Permission { Email = "app.admin@novaland.com.vn" });

                await context.SaveChangesAsync();
            }

            //if (!context.AssessmentPeriods.Any())
            //{
            //    await GeneratePeriods(context, contentRootPath, logger, userService, setting);

            //    await context.SaveChangesAsync();
            //}


        }

        private IEnumerable<AssessmentCriteria> GetAssessmentCriteriaFromFile(string contentRootPath, ILogger<AssessmentContextSeed> logger)
        {
            string csvFileQuestions = Path.Combine(contentRootPath, "Setup", "criteria.csv");

            if (File.Exists(csvFileQuestions))
            {
                return File.ReadAllLines(csvFileQuestions)
                            .SelectTry(x => CreateCriteria(x))
                            .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                        .Where(x => x != null);
            }

            return new List<AssessmentCriteria>();
        }

        private AssessmentCriteria CreateCriteria(string s)
        {
            s = s.Trim('"').Trim();

            if (String.IsNullOrEmpty(s))
            {
                throw new Exception("Criteria is invalid");
            }

            string[] arrStr = s.Split(',', StringSplitOptions.None);
            return new AssessmentCriteria(arrStr[1], 0);
        }

        private IEnumerable<AssessmentQuestion> GetAssessmentQuestionFromFile(string contentRootPath, ILogger<AssessmentContextSeed> logger)
        {
            string csvFileQuestions = Path.Combine(contentRootPath, "Setup", "questions.csv");

            if (File.Exists(csvFileQuestions))
            {
                return File.ReadAllLines(csvFileQuestions)
                            .SelectTry(x => CreateQuestion(x))
                            .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                        .Where(x => x != null);
            }

            return new List<AssessmentQuestion>();
        }

        private AssessmentQuestion CreateQuestion(string s)
        {
            s = s.Trim('"').Trim();

            if (String.IsNullOrEmpty(s))
            {
                throw new Exception("question is invalid");
            }

            string[] arrStr = s.Split(',', StringSplitOptions.None);
            return new AssessmentQuestion(arrStr[2], int.Parse(arrStr[1]), 0);
        }

        private IEnumerable<DepartmentMatrix> GetDepartmentMatrixFromFile(string contentRootPath, ILogger<AssessmentContextSeed> logger, IUserOrgHelper userService)
        {
            string csvFileQuestions = Path.Combine(contentRootPath, "Setup", "dpmatrix.xlsx");
            var lst = new List<DepartmentMatrix>();

            if (File.Exists(csvFileQuestions))
            {
                using (var stream = File.Open(csvFileQuestions, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Use the AsDataSet extension method
                        var result = reader.AsDataSet();

                        // The result of each spreadsheet is in result.Tables

                        var firstRowArray = result.Tables[0].Rows[0].ItemArray;
                        if (result.Tables[0] != null)
                        {
                            for(int i = 1; i < result.Tables[0].Rows.Count; i++)
                            {
                                var arrItems = result.Tables[0].Rows[i].ItemArray;
                                var rowHeader = userService.FormatDepartmentCode(arrItems[0].ToString());
                                for (int j = 1; j < result.Tables[0].Columns.Count; j++)
                                {
                                    var colHeader = userService.FormatDepartmentCode(firstRowArray[j].ToString());
                                    if(!lst.Any(x=>x.DepartmentFrom == rowHeader && x.DepartmentTo == colHeader)
                                        && arrItems[j] != null && !string.IsNullOrEmpty(arrItems[j].ToString()) 
                                        && int.TryParse(arrItems[j].ToString(), out int interact))
                                    {
                                        lst.Add(new DepartmentMatrix(rowHeader, colHeader, interact, string.Empty, 0));
                                    }

                                }
                            }
                        }

                    }
                }
            }
            return lst;
        }

        private IEnumerable<AssessmentPeriod> GetPeriodsFromFile(string contentRootPath, ILogger<AssessmentContextSeed> logger)
        {
            string csvFileQuestions = Path.Combine(contentRootPath, "Setup", "periods.csv");

            if (File.Exists(csvFileQuestions))
            {
                return File.ReadAllLines(csvFileQuestions)
                            .SelectTry(x => CreatePeriod(x))
                            .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                        .Where(x => x != null);
            }

            return new List<AssessmentPeriod>();
        }

        private AssessmentPeriod CreatePeriod(string s)
        {
            s = s.Trim('"').Trim();

            if (String.IsNullOrEmpty(s))
            {
                throw new Exception("Period is invalid");
            }

            string[] arrStr = s.Split(',', StringSplitOptions.None);
            return new AssessmentPeriod(arrStr[0], DateTime.Parse(arrStr[1]), DateTime.Parse(arrStr[2]), bool.Parse(arrStr[3]) ,string.Empty,0);
        }

        //private async Task GeneratePeriods(AssessmentContext context, string contentRootPath, ILogger<AssessmentContextSeed> logger, IUserOrgHelper userService, AppSettings setting)
        //{
        //    //Randomize maxtrix departments
        //    var matrix = await context.DepartmentMatrix.ToListAsync();

        //    //select 20 Dep
        //    int numOfDept = 20;
        //    if (!int.TryParse(setting.NumOfDepartmentsSelected, out numOfDept)) numOfDept = 20;
        //    List<string> selectedDepartments = new List<string>();

        //    //get interact = 3
        //    selectedDepartments.AddRange(RandomAssessmentDepartments(matrix, selectedDepartments, 3, numOfDept));
        //    selectedDepartments.AddRange(RandomAssessmentDepartments(matrix, selectedDepartments, 2, numOfDept));
        //    selectedDepartments.AddRange(RandomAssessmentDepartments(matrix, selectedDepartments, 1, numOfDept));
            
        //    //read period from file
        //    var periods = GetPeriodsFromFile(contentRootPath, logger);

        //    //generate departments
        //    int numOfUser = 10;
        //    if (!int.TryParse(setting.NumOfUserSelected, out numOfUser)) numOfUser = 10;
        //    var fullDepartments = await context.Departments.ToListAsync();
        //    var fullUsers = await context.Users.ToListAsync();
        //    foreach (var period in periods)
        //    {
        //        //add questions
        //        await context.AssessmentQuestions.ForEachAsync(x => period.AddQuestion(x.Id, 0));

        //        //add departments 
        //        List<string> departmentFroms = matrix.Select(x => x.DepartmentFrom).Distinct().ToList();
        //        Dictionary<string, List<int>> selectedUsers = GenerateUsers(departmentFroms, numOfUser, logger, userService, fullDepartments, fullUsers);
        //        foreach (var selectedToDept  in selectedDepartments)
        //        {
        //            GenerateSelectedDepartments(period, selectedToDept, selectedUsers, logger, userService, fullDepartments);
        //        }

        //        await context.AssessmentPeriods.AddRangeAsync(period);
        //    }

        //}

        //private List<string> RandomAssessmentDepartments(List<DepartmentMatrix> matrix, List<string> selectedDepts, 
        //                                                        int interact, int numDepts)
        //{
        //    Random rd = new Random();
        //    var templist = matrix.Where(x => x.Interact == interact && !selectedDepts.Contains(x.DepartmentTo)).ToList();
        //    while(templist.Any() && selectedDepts.Count < numDepts)
        //    {
        //        int rdNum = rd.Next(templist.Count);
        //        selectedDepts.Add(templist[rdNum].DepartmentTo);
        //        templist = matrix.Where(x => x.Interact == interact && !selectedDepts.Contains(x.DepartmentTo)).ToList();
        //    }
               
        //    return selectedDepts;
        //}

        //private Dictionary<string, List<int>> GenerateUsers(List<string> departmentFroms, int numOfUser,
        //                        ILogger<AssessmentContextSeed> logger, IUserOrgHelper userService, List<Department> fullDetps, List<User> fullUsers)
        //{
        //    Dictionary<string, List<int>> users = new Dictionary<string, List<int>>();
        //    Random rd = new Random();

        //    foreach (var dept in departmentFroms)
        //    {
        //        var deparmentFromCode = userService.FormatDepartmentCode(dept);
        //        var fromDepartment = fullDetps.FirstOrDefault(x => x.ShortCode == deparmentFromCode);

        //        List<int> userIds = new List<int>();
        //        if (fromDepartment!=null)
        //        {
        //            var userInDept = fullUsers.Where(x => x.DepartmentCode == fromDepartment.Id && x.JobLevel >= 7
        //                                                        && !string.IsNullOrEmpty(x.Email)).ToList();
        //            for (int i = 1; i <= 100; i++)
        //            {
        //                try
        //                {
        //                    int rdNumber = rd.Next(userInDept.Count);
        //                    var selectedUser = userInDept[rdNumber];

        //                    if (!string.IsNullOrEmpty(selectedUser.Email))
        //                    {
        //                        userIds.Add(selectedUser.Id);
        //                    }
        //                    userInDept.RemoveAt(rdNumber);
        //                    if (userIds.Count == numOfUser) break;
        //                }
        //                catch (Exception ex1)
        //                {
        //                    logger.LogError(ex1, "EXCEPTION ERROR: {Message}", ex1.Message);
        //                }
        //            }
        //        }

        //        users.Add(dept, userIds);
        //    }

        //    return users;
        //}

        //private void GenerateSelectedDepartments(AssessmentPeriod period, string selectedToDept, Dictionary<string, List<int>> selectedUsers, 
        //                        ILogger<AssessmentContextSeed> logger, IUserOrgHelper userService, List<Department> fullDetps)
        //{

        //    var deparmentToCode = userService.FormatDepartmentCode(selectedToDept);
        //    var toDepartment = fullDetps.FirstOrDefault(x => x.ShortCode == deparmentToCode);

        //    foreach (var mxItem in selectedUsers)
        //    {
        //        var deparmentFromCode = userService.FormatDepartmentCode(mxItem.Key);
        //        var fromDepartment = fullDetps.FirstOrDefault(x => x.ShortCode == deparmentFromCode);

        //        if (fromDepartment != null && toDepartment != null)
        //        {
        //            try
        //            {
        //                var selected = new PeriodSelectedDepartment(0, fromDepartment.Id, toDepartment.Id, 0);

        //                var userInDept = mxItem.Value;
        //                if (userInDept.Any())
        //                {
        //                    foreach(int uid in userInDept)
        //                    {
        //                        selected.AddSelectedUser(uid, 0);
        //                    }
        //                }
                        
        //                period.AddSelectedDepartment(selected, 0);
                        
        //            }
        //            catch (Exception ex)
        //            {
        //                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
        //            }

        //        }
        //    }
        //}
    
        private List<Department> GetDepartmentsFromServices(ILogger<AssessmentContextSeed> logger, IUserOrgHelper userService)
        {
            var fullDepartments = userService.GetDepartment();
            List<Department> listDeptEntities = new List<Department>();
            foreach(var dept in fullDepartments)
            {
                try
                {
                    var deptEntity = new Department(int.Parse(dept.Id), dept.Code, dept.Name, dept.Type, string.Empty, string.Empty, string.Empty, 0);
                    listDeptEntities.Add(deptEntity);
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                }
            }

            return listDeptEntities;
        }

        private List<User> GetUsersFromServices(ILogger<AssessmentContextSeed> logger, IUserOrgHelper userService)
        {
            var fullUsers = userService.GetUsers();
            List<User> listUserEntities = new List<User>();
            foreach (var user in fullUsers)
            {
                try
                {
                    if (listUserEntities.All(x => x.OrgUserCode != user.OrgUserCode))
                    {
                        var userEntity = new User(user.OrgUserCode, user.FullName, user.OrgUserName, user.DepartmentCode,
                                                   user.JobTitle, user.Gender, user.Email, user.JobLevel, DateTime.Parse(user.DateOfBirth), user.CompanyCode,
                                                   user.LocationCode, user.PhoneNumber, user.IsManager, 0);
                        listUserEntities.Add(userEntity);
                    }
                    
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                }
            }

            return listUserEntities;
        }

        private async Task GetDepartmentsFromFile(AssessmentContext context, string contentRootPath, ILogger<AssessmentContextSeed> logger)
        {
            string csvFileQuestions = Path.Combine(contentRootPath, "Setup", "departments.xlsx");

            if (File.Exists(csvFileQuestions))
            {
                using (var stream = File.Open(csvFileQuestions, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Use the AsDataSet extension method
                        var result = reader.AsDataSet();

                        // The result of each spreadsheet is in result.Tables
                        if (result.Tables[0] != null)
                        {
                            for (int i = 1; i < result.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    var arrItems = result.Tables[0].Rows[i].ItemArray;

                                    var tempdept = new Department(0, arrItems[2].ToString(), arrItems[2].ToString(), "PB", arrItems[1].ToString(), arrItems[1].ToString(), arrItems[3].ToString(), 0);
                                    var existing = await context.Departments.SingleOrDefaultAsync(x => x.ShortCode.ToUpper().TrimEnd().Replace(" ", "") == tempdept.ShortCode.ToUpper().TrimEnd().Replace(" ", ""));
                                    if (existing != null)
                                    {
                                        existing.Update(arrItems[2].ToString(), arrItems[2].ToString(), "PB", arrItems[1].ToString(), arrItems[1].ToString(), arrItems[3].ToString(), 0);
                                        context.Entry(existing).State = EntityState.Modified;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);

                                }
                            }
                        }

                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}


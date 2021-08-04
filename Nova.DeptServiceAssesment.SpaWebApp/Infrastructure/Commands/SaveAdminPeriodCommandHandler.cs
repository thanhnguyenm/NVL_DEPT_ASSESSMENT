using MediatR;
using Microsoft.Extensions.Logging;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.DepartmentMatrixAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.Domain.ExternalModel;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.SpaWebApp.Configuration;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Extensions;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class SaveAdminPeriodCommandHandler : IRequestHandler<SaveAdminPeriodCommand, int>
    {
        private readonly IAssessmentPeriodRepository _assessmentPeriodRepository;
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly IIdentityClaimsHelper _identityClaimsHelper;
        private readonly IUserOrgHelper _userService;
        private readonly ILogger<SaveAdminPeriodCommandHandler> _logger;
        private readonly AppSettings _setting;

        public SaveAdminPeriodCommandHandler(IAssessmentPeriodRepository assessmentPeriodRepository,
                                                IAssessmentCriteriaRepository assessmentCriteriaRepository,
                                                 IIdentityClaimsHelper identityClaimsHelper,
                                                 IUserOrgHelper userService,
                                                 ILogger<SaveAdminPeriodCommandHandler> logger,
                                                 AppSettings setting)
        {
            _assessmentPeriodRepository = assessmentPeriodRepository;
            _identityClaimsHelper = identityClaimsHelper;
            _setting = setting;
            _userService = userService;
            _logger = logger;
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
        }

        public async Task<int> Handle(SaveAdminPeriodCommand request, CancellationToken cancellationToken)
        {
            try
            {
                AssessmentPeriod period;
                if (request.Id == 0)
                {
                    if (request.SelectedQuestions == null || request.SelectedQuestions.Count == 0)
                        throw new DomainException("Thiếu câu hỏi cho đợt đánh giá");
                    
                    period = new AssessmentPeriod(request.PeriodName, request.PeriodFrom.AddDays(1), request.PeriodTo.AddDays(1), request.Published, string.Empty, _identityClaimsHelper.CurrentUser.Id);

                    //get departments and users
                    var fullDepartments = await _assessmentCriteriaRepository.GetDepartmentsFromDB();
                    var fullUsers = await _assessmentCriteriaRepository.GetActiveUsersFromDB(7);

                    //add questions
                    var questions = await _assessmentCriteriaRepository.GetQuestionsAsync();
                    request.SelectedQuestions.ForEach(x => period.AddQuestion(x, 0));

                    //Randomize maxtrix departments
                    var matrix = await _assessmentCriteriaRepository.GetDepartmentMatrix();

                    //random users
                    int numOfUser = 10;
                    if (!int.TryParse(_setting.NumOfUserSelected, out numOfUser)) numOfUser = 10;
                    Dictionary<string, List<int>> userSelectted = new Dictionary<string, List<int>>();
                    RandomUsers(matrix, userSelectted, fullDepartments, fullUsers, numOfUser, _userService);

                    //select 20 Dep
                    int numOfDept = 10;
                    if (!int.TryParse(_setting.NumOfDepartmentsSelected, out numOfDept)) numOfDept = 10;
                    GenerateAssessmentDepartments(period, matrix, userSelectted, fullDepartments, fullUsers, numOfDept, _userService);

                    period = _assessmentPeriodRepository.AddPeriod(period);
                }
                else
                {
                    period = await _assessmentPeriodRepository.GetPeriodAsync(request.Id);
                    period.Update(request.PeriodName, request.PeriodFrom.AddDays(1), request.PeriodTo.AddDays(1), request.Published, string.Empty, _identityClaimsHelper.CurrentUser.Id);

                    _assessmentPeriodRepository.UpdatePeriod(period);
                }

                await _assessmentPeriodRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                return period.Id;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void RandomUsers(List<DepartmentMatrix> matrix, Dictionary<string, List<int>> selectedUsers, List<Department> fullDetps, List<User> fullUsers, 
                                    int numOfUser, IUserOrgHelper userService)
        {
            Random rd = new Random();
            List<string> assignedDepartments = matrix.Select(x => x.DepartmentFrom).Distinct().ToList();
            foreach(var dept in assignedDepartments)
            {
                var deparmentFromCode = userService.FormatDepartmentCode(dept);
                var fromDepartment = fullDetps.FirstOrDefault(x => x.ShortCode.ToUpper().TrimEnd().Replace(" ", "") == deparmentFromCode.ToUpper().TrimEnd().Replace(" ", ""));

                List<int> userIds = new List<int>();
                if (fromDepartment != null)
                {
                    var userInDept = fullUsers.Where(x => x.DepartmentCode == fromDepartment.Id && !string.IsNullOrEmpty(x.Email)).ToList();
                    for (int i = 1; i <= 100; i++)
                    {
                        try
                        {
                            int rdNumber = rd.Next(userInDept.Count);
                            var selectedUser = userInDept[rdNumber];

                            if (!string.IsNullOrEmpty(selectedUser.Email))
                            {
                                userIds.Add(selectedUser.Id);
                            }
                            userInDept.RemoveAt(rdNumber);
                            if (userIds.Count == numOfUser) break;
                        }
                        catch (Exception ex1)
                        {
                            _logger.LogError(ex1, "EXCEPTION ERROR: {Message}", ex1.Message);
                        }
                    }
                }

                selectedUsers.Add(dept, userIds);
            }

        }

        private void GenerateAssessmentDepartments(AssessmentPeriod period, List<DepartmentMatrix> matrix, Dictionary<string, List<int>> selectedUsers,
                                                                                List<Department> fullDetps, List<User> fullUsers, int numDepts, IUserOrgHelper userService)
        {
            List<PeriodSelectedDepartment> periodSelectedDepartments = new List<PeriodSelectedDepartment>();

            foreach (var user in selectedUsers)
            {
                var deparmentFromCode = userService.FormatDepartmentCode(user.Key);
                var fromDepartment = fullDetps.FirstOrDefault(x => x.ShortCode.ToUpper().TrimEnd().Replace(" ", "") == deparmentFromCode.ToUpper().TrimEnd().Replace(" ", ""));
                foreach(var uid in user.Value)
                {
                    List<string> assessmentDepts = new List<string>();
                    RandomAssessmentDepartments(matrix, assessmentDepts, fullDetps, user.Key, 1, (int)(numDepts * 0.7), userService);
                    RandomAssessmentDepartments(matrix, assessmentDepts, fullDetps, user.Key, 2, (int)(numDepts * 0.2), userService);
                    RandomAssessmentDepartments(matrix, assessmentDepts, fullDetps, user.Key, 3, (int)(numDepts * 0.1), userService);

                    if (assessmentDepts.Count < numDepts)
                    {
                        RandomAssessmentDepartments(matrix, assessmentDepts, fullDetps, user.Key, null, numDepts - assessmentDepts.Count, userService);
                    }

                    foreach(var assDept in assessmentDepts)
                    {
                        var deparmentToCode = userService.FormatDepartmentCode(assDept);
                        var toDepartment = fullDetps.FirstOrDefault(x => x.ShortCode.ToUpper().TrimEnd().Replace(" ","") == deparmentToCode.ToUpper().TrimEnd().Replace(" ", ""));
                        if (toDepartment != null)
                        {
                            period.AddSelectedDepartment(fromDepartment.Id, toDepartment.Id, uid, 0);
                        }
                        
                    }
                }

            }

        }

        private void RandomAssessmentDepartments(List<DepartmentMatrix> matrix, List<string> selectedDepts, List<Department> fullDetps,
                                                               string deptFrom, int? interact, int numDepts, IUserOrgHelper userService)
        {
            Random rd = new Random();
            var templist = matrix.Where(x => ((interact.HasValue && x.Interact == interact) || !interact.HasValue) && !selectedDepts.Contains(x.DepartmentTo) && x.DepartmentTo != deptFrom).ToList();
            while (templist.Any() && selectedDepts.Count < numDepts)
            {
                int rdNum = rd.Next(templist.Count);

                var deparmentToCode = userService.FormatDepartmentCode(templist[rdNum].DepartmentTo);
                var toDepartment = fullDetps.FirstOrDefault(x => x.ShortCode.ToUpper().TrimEnd().Replace(" ", "") == deparmentToCode.ToUpper().TrimEnd().Replace(" ", ""));
                if (toDepartment != null)
                {
                    selectedDepts.Add(templist[rdNum].DepartmentTo);
                }
                templist = matrix.Where(x => ((interact.HasValue && x.Interact == interact) || !interact.HasValue) && !selectedDepts.Contains(x.DepartmentTo)).ToList();
            }

        }
    }
}

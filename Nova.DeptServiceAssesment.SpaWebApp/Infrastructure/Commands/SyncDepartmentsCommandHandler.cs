using MediatR;
using Microsoft.Extensions.Logging;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.SpaWebApp.Configuration;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class SyncDepartmentsCommandHandler : IRequestHandler<SyncDepartmentsCommand, bool>
    {
        private readonly IAssessmentPeriodRepository _assessmentPeriodRepository;
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly IIdentityClaimsHelper _identityClaimsHelper;
        private readonly IUserOrgHelper _userService;
        private readonly ILogger<SyncDepartmentsCommandHandler> _logger;
        private readonly AppSettings _setting;

        public SyncDepartmentsCommandHandler(IAssessmentPeriodRepository assessmentPeriodRepository,
                                                IAssessmentCriteriaRepository assessmentCriteriaRepository,
                                                 IIdentityClaimsHelper identityClaimsHelper,
                                                 IUserOrgHelper userService,
                                                 ILogger<SyncDepartmentsCommandHandler> logger,
                                                 AppSettings setting)
        {
            _assessmentPeriodRepository = assessmentPeriodRepository;
            _identityClaimsHelper = identityClaimsHelper;
            _setting = setting;
            _userService = userService;
            _logger = logger;
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
        }

        public async Task<bool> Handle(SyncDepartmentsCommand request, CancellationToken cancellationToken)
        {
            var departments = await GetDepartmentsFromServices(_logger, _userService);
            var oldDepartments = await _assessmentCriteriaRepository.GetDepartmentsFromDB();
            var needUpdate = new List<Department>();
            var needAdd = new List<Department>();

            foreach(var dept in departments)
            {
                var oldDept = oldDepartments.FirstOrDefault(x => x.Id == dept.Id);
                if(oldDept == null)
                {
                    needAdd.Add(dept);
                }
                else if(oldDept.Code != dept.Code ||
                    oldDept.Name != dept.Name ||
                    oldDept.Type != dept.Type ||
                    oldDept.DivCode != dept.DivCode ||
                    oldDept.DivName != dept.DivName)
                {
                    oldDept.Update(dept.Code, dept.Name, dept.Type, dept.DivCode, dept.DivName, string.Empty, _identityClaimsHelper.CurrentUser.Id);
                    needUpdate.Add(oldDept);
                }
            }


            await _assessmentCriteriaRepository.AddDepartments(needAdd);
            _assessmentCriteriaRepository.UpdateDepartments(needUpdate);

            return await _assessmentCriteriaRepository.UnitOfWork.SaveEntitiesAsync();
        }

        private async Task<List<Department>> GetDepartmentsFromServices(ILogger<SyncDepartmentsCommandHandler> logger, IUserOrgHelper userService)
        {
            var fullDepartments = await userService.GetDepartmentAsync();
            List<Department> listDeptEntities = new List<Department>();
            foreach (var dept in fullDepartments)
            {
                try
                {
                    var deptEntity = new Department(int.Parse(dept.Id), dept.Code, dept.Name, dept.Type, string.Empty, string.Empty, string.Empty, 0);
                    listDeptEntities.Add(deptEntity);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                }
            }

            return listDeptEntities;
        }
    }
}
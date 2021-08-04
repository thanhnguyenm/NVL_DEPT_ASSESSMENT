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
    public class SyncUsersCommandHandler : IRequestHandler<SyncUsersCommand, bool>
    {
        private readonly IAssessmentPeriodRepository _assessmentPeriodRepository;
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly IIdentityClaimsHelper _identityClaimsHelper;
        private readonly IUserOrgHelper _userService;
        private readonly ILogger<SyncUsersCommandHandler> _logger;
        private readonly AppSettings _setting;

        public SyncUsersCommandHandler(IAssessmentPeriodRepository assessmentPeriodRepository,
                                                IAssessmentCriteriaRepository assessmentCriteriaRepository,
                                                 IIdentityClaimsHelper identityClaimsHelper,
                                                 IUserOrgHelper userService,
                                                 ILogger<SyncUsersCommandHandler> logger,
                                                 AppSettings setting)
        {
            _assessmentPeriodRepository = assessmentPeriodRepository;
            _identityClaimsHelper = identityClaimsHelper;
            _setting = setting;
            _userService = userService;
            _logger = logger;
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
        }

        public async Task<bool> Handle(SyncUsersCommand request, CancellationToken cancellationToken)
        {
            var users = await GetUsersFromServices(_logger, _userService);
            var oldUsers = await _assessmentCriteriaRepository.GetUsersFromDB();
            var skeepList = new List<User>();
            var needUpdate = new List<User>();
            var needAdd = new List<User>();

            int currentuserid = _identityClaimsHelper.CurrentUser.Id;
            foreach (var user in users)
            {
                var oldUser = oldUsers.FirstOrDefault(x => x.OrgUserCode == user.OrgUserCode);
                if (oldUser == null)
                {
                    if (!needAdd.Any(x => x.OrgUserCode == user.OrgUserCode))
                    {
                        needAdd.Add(user);
                    }
                    
                }
                else if (oldUser.OrgUserName != user.OrgUserName ||
                    oldUser.FullName != user.FullName ||
                    oldUser.DepartmentCode != user.DepartmentCode ||
                    oldUser.JobTitle != user.JobTitle ||
                    oldUser.Email != user.Email ||
                    oldUser.JobLevel != user.JobLevel ||
                    oldUser.DateOfBirth != user.DateOfBirth ||
                    oldUser.CompanyCode != user.CompanyCode ||
                    oldUser.LocationCode != user.LocationCode ||
                    oldUser.PhoneNumber != user.PhoneNumber
                    )
                {
                    oldUser.Update(user.OrgUserCode, user.FullName, user.OrgUserName, user.DepartmentCode,
                                                   user.JobTitle, user.Gender, user.Email, user.JobLevel, user.DateOfBirth, user.CompanyCode,
                                                   user.LocationCode, user.PhoneNumber, user.IsManager,
                                                   currentuserid);
                    oldUser.SetDelete(false, currentuserid);
                    needUpdate.Add(oldUser);
                }
                else
                {
                    skeepList.Add(oldUser);
                }
            }

            var needDeleted = oldUsers.Where(x => !needUpdate.Contains(x) && !skeepList.Contains(x)).ToList();
            needDeleted.ForEach(x =>
            {
                x.SetDelete(true, currentuserid);
            });

            await _assessmentCriteriaRepository.AddUsers(needAdd);
            _assessmentCriteriaRepository.UpdateUsers(needUpdate);
            _assessmentCriteriaRepository.UpdateUsers(needDeleted);

            return await _assessmentCriteriaRepository.UnitOfWork.SaveEntitiesAsync();
        }

        private async Task<List<User>> GetUsersFromServices(ILogger<SyncUsersCommandHandler> logger, IUserOrgHelper userService)
        {
            var fullUsers = await userService.GetUsersAsync();
            List<User> listUserEntities = new List<User>();
            foreach (var user in fullUsers)
            {
                try
                {
                    DateTime tempDate;
                    if(!DateTime.TryParse(user.DateOfBirth, out tempDate))
                    {
                        tempDate = DateTime.MaxValue;
                    }
                    var userEntity = new User(user.OrgUserCode, user.FullName, user.OrgUserName, user.DepartmentCode,
                                                   user.JobTitle, user.Gender, user.Email, user.JobLevel, tempDate, user.CompanyCode,
                                                   user.LocationCode, user.PhoneNumber, user.IsManager, 0);
                    listUserEntities.Add(userEntity);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                }
            }

            return listUserEntities;
        }
    }
}

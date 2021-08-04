using MediatR;
using Microsoft.Extensions.Logging;
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
    public class DeleteAdminPermisionCommandHandler : IRequestHandler<DeleteAdminPermisionCommand, bool>
    {
        private readonly IAssessmentPeriodRepository _assessmentPeriodRepository;
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly IIdentityClaimsHelper _identityClaimsHelper;
        private readonly IUserOrgHelper _userService;
        private readonly ILogger<DeleteAdminPermisionCommandHandler> _logger;
        private readonly AppSettings _setting;

        public DeleteAdminPermisionCommandHandler(IAssessmentPeriodRepository assessmentPeriodRepository,
                                                IAssessmentCriteriaRepository assessmentCriteriaRepository,
                                                 IIdentityClaimsHelper identityClaimsHelper,
                                                 IPermissionsRepository permissionsRepository,
                                                 IUserOrgHelper userService,
                                                 ILogger<DeleteAdminPermisionCommandHandler> logger,
                                                 AppSettings setting)
        {
            _assessmentPeriodRepository = assessmentPeriodRepository;
            _identityClaimsHelper = identityClaimsHelper;
            _setting = setting;
            _userService = userService;
            _logger = logger;
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
            _permissionsRepository = permissionsRepository;
        }

        public async Task<bool> Handle(DeleteAdminPermisionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var perrmision = await _permissionsRepository.GetPermissionAsync(request.Email);
                if (perrmision != null)
                {
                    _permissionsRepository.DeletePermission(perrmision);
                }

                return await _permissionsRepository.UnitOfWork.SaveEntitiesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}

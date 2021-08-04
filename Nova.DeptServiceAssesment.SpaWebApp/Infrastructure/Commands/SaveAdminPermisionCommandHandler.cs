using MediatR;
using Microsoft.Extensions.Logging;
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
    public class SaveAdminPermisionCommandHandler : IRequestHandler<SaveAdminPermisionCommand, bool>
    {
        private readonly IAssessmentPeriodRepository _assessmentPeriodRepository;
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly IIdentityClaimsHelper _identityClaimsHelper;
        private readonly IUserOrgHelper _userService;
        private readonly ILogger<SaveAdminPermisionCommandHandler> _logger;
        private readonly AppSettings _setting;

        public SaveAdminPermisionCommandHandler(IAssessmentPeriodRepository assessmentPeriodRepository,
                                                IAssessmentCriteriaRepository assessmentCriteriaRepository,
                                                 IIdentityClaimsHelper identityClaimsHelper,
                                                 IPermissionsRepository permissionsRepository,
                                                 IUserOrgHelper userService,
                                                 ILogger<SaveAdminPermisionCommandHandler> logger,
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

        public async Task<bool> Handle(SaveAdminPermisionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Email.Trim()))
                    throw new DomainException("Email không được trống");

                string errormessage = "";
                List<string> emails = request.Email.Split(";", StringSplitOptions.RemoveEmptyEntries)
                                            .SelectTry(x => CreateEmail(x))
                                            .OnCaughtException(ex => { errormessage = ex.Message; _logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                            .Where(x => x != null).ToList();
                if (!string.IsNullOrEmpty(errormessage))
                {
                    throw new DomainException(errormessage);
                }
                    
                var perrmisions = await _permissionsRepository.GetPermissionsAsync(emails);
                foreach(var email in emails)
                {
                    var perrmision = perrmisions.FirstOrDefault(x => x.Email == email);
                    if (perrmision == null)
                    {
                        perrmision = new Domain.AggregatesModel.UserOrgAggregate.Permission();
                        perrmision.Email = request.Email;

                        perrmision = _permissionsRepository.AddPermission(perrmision);
                    }
                }
                

                return await _permissionsRepository.UnitOfWork.SaveEntitiesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string CreateEmail(string email)
        {
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(email.Trim()))
            {
                return null;
            }

            try
            {
                email = email.Trim();
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address == email)
                {
                    return email.ToLower();
                }
                throw new Exception("Email invalid");
            }
            catch
            {
                throw new Exception("Email invalid");
            }
        }
    }
}

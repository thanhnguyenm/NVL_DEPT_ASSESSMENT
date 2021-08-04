using Microsoft.Extensions.Logging;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class SendEmailJob : IJob
    {
        private readonly ILogger<SendEmailJob> _logger;
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly IEmailHelper _emailHelper;

        public SendEmailJob(ILogger<SendEmailJob> logger,
                            IEmailHelper emailHelper,
                            IPermissionsRepository permissionsRepository)
        {
            _logger = logger;
            _emailHelper = emailHelper;
            _permissionsRepository = permissionsRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Send email start");

            var emails = await _permissionsRepository.GetNewEmailsAsync();
            
            foreach (var email in emails)
            {
                try
                {
                    var addresses = email.To.Split(";").ToList();
                    var rs = _emailHelper.SendEmail(addresses, email.Subject, email.Body);

                    if (rs)
                    {
                        email.Status = "SENT";
                    }
                    else
                    {
                        email.Status = "ERROR";
                    }
                    _permissionsRepository.UpdateEmail(email);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }

            }

            await _permissionsRepository.UnitOfWork.SaveEntitiesAsync();

            _logger.LogInformation("Send email complete");

        }
    }
}

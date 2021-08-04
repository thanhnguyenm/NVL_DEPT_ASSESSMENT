using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.SpaWebApp.Configuration;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Queries;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class ReminderJob : IJob
    {
        private readonly ILogger<ReminderJob> _logger;
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly IEmailHelper _emailHelper;
        private readonly IAdminSetupQueries _adminSetupQueries;
        private readonly AppSettings _setting;

        public ReminderJob(AppSettings setting,
                            ILogger<ReminderJob> logger,
                            IEmailHelper emailHelper,
                            IAdminSetupQueries adminSetupQueries,
                            IPermissionsRepository permissionsRepository)
        {
            _setting = setting;
            _logger = logger;
            _emailHelper = emailHelper;
            _adminSetupQueries = adminSetupQueries;
            _permissionsRepository = permissionsRepository;
        }

        public Task Execute(IJobExecutionContext context)
        {
            
            try
            {
                _logger.LogInformation("Reminder start");

                var reminders = _adminSetupQueries.GetUserPending().GetAwaiter().GetResult();
                var periods = reminders.Select(x => x.Id).Distinct().ToList();

                string subject = "Nhắc nhờ hoàn thành đánh giá dịch vụ các phòng ban";
                StringBuilder bodyBuilder = new StringBuilder();
                bodyBuilder.Append($"Xin chào, <br/><br/>");
                bodyBuilder.Append($"Bạn vẫn chưa thực hiện xong việc đánh giá chất lượng dịch vụ phòng ban đợt \"<b>##PeriodName##</b>\".<br/><br/>");
                bodyBuilder.Append($"Hệ thống sẽ đóng sau ngày ##PeriodTo## nếu hêt hạn đánh giá và sẽ ảnh hưởng đến KPI của bạn. <br/><br/>");
                bodyBuilder.Append($"Vui lòng vào được dẫn sau để đánh giá.<br/><br/>");
                bodyBuilder.Append($"Link ##Link##.<br/><br/><br/><br/>");
                bodyBuilder.Append($"Vui lòng không reply email này.<br/><br/>");
                bodyBuilder.Append($"Trân trọng!<br/>");
                string body = bodyBuilder.ToString();
                body = body.Replace("##Link##", $"{_setting.HostUrl}/");

                foreach(var p in periods)
                {
                    
                    var firstRemider = reminders.FirstOrDefault(x => x.Id == p);
                    if (firstRemider != null)
                    {
                        var copybody = body.Replace("##PeriodName##", firstRemider.PeriodName);
                        copybody = copybody.Replace("##PeriodTo##", firstRemider.PeriodTo.ToString("dd/MM/yyyy"));

                        var emailTos = string.Join(";", reminders.Where(x => x.Id == p).Select(x => x.Email).ToList());
                        Email email = new Email
                        {
                            To = emailTos,
                            Body = copybody,
                            Subject = subject,
                            Status = "NEW"
                        };
                        _permissionsRepository.AddEmail(email);
                    }
                        
                    _permissionsRepository.UnitOfWork.SaveEntitiesAsync().Wait();
                }
                

                _logger.LogInformation("Reminder complete");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}

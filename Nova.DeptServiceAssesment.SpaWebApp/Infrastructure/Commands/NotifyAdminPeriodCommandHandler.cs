using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.SpaWebApp.Configuration;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Extensions;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class NotifyAdminPeriodCommandHandler : IRequestHandler<NotifyAdminPeriodCommand, bool>
    {
        private readonly IAssessmentPeriodRepository _assessmentPeriodRepository;
        private readonly IHttpContextAccessor _context;
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly IIdentityClaimsHelper _identityClaimsHelper;
        private readonly IUserOrgHelper _userService;
        private readonly ILogger<NotifyAdminPeriodCommand> _logger;
        private readonly AppSettings _setting;
        private readonly IAdminSetupQueries _adminSetupQueries;

        public NotifyAdminPeriodCommandHandler(IHttpContextAccessor context,
                                                IAssessmentPeriodRepository assessmentPeriodRepository,
                                                IAssessmentCriteriaRepository assessmentCriteriaRepository,
                                                 IIdentityClaimsHelper identityClaimsHelper,
                                                 IUserOrgHelper userService,
                                                 ILogger<NotifyAdminPeriodCommand> logger,
                                                 IPermissionsRepository permissionsRepository,
                                                 IAdminSetupQueries adminSetupQueries,
                                                 AppSettings setting)
        {
            _context = context;
            _assessmentPeriodRepository = assessmentPeriodRepository;
            _identityClaimsHelper = identityClaimsHelper;
            _setting = setting;
            _userService = userService;
            _logger = logger;
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
            _permissionsRepository = permissionsRepository;
            _adminSetupQueries = adminSetupQueries;
        }

        public async Task<bool> Handle(NotifyAdminPeriodCommand request, CancellationToken cancellationToken)
        {


            try
            {

                var period = await _assessmentPeriodRepository.GetPeriodAsync(request.Id);
                if (period == null) throw new DomainException("Không tìm thấy kỳ đánh giá này");
                if (period.PeriodSelectedDepartments == null || period.PeriodSelectedDepartments.Count() == 0) 
                    throw new DomainException("Không tim thấy phòng ban nào trong kỳ đánh giá này");

                string emailTos = string.Empty;
                string subject = string.Empty;
                StringBuilder bodyBuilder = new StringBuilder();
                if (request.IsReminder)
                {
                    subject = "Nhắc nhờ hoàn thành đánh giá dịch vụ các phòng ban";
                    bodyBuilder.Append($"Xin chào, <br/><br/>");
                    bodyBuilder.Append($"Bạn vẫn chưa thực hiện xong việc đánh giá chất lượng dịch vụ phòng ban đợt \"<b>##PeriodName##</b>\".<br/><br/>");
                    bodyBuilder.Append($"Hệ thống sẽ đóng sau ngày ##PeriodTo## nếu hêt hạn đánh giá và sẽ ảnh hưởng đến KPI của bạn. <br/><br/>");
                    bodyBuilder.Append($"Vui lòng vào được dẫn sau để đánh giá.<br/><br/>");
                    bodyBuilder.Append($"Link ##Link##.<br/><br/><br/><br/>");
                    bodyBuilder.Append($"Vui lòng không reply email này.<br/><br/>");
                    bodyBuilder.Append($"Trân trọng!<br/>");

                    var reminders = _adminSetupQueries.GetUserPendingByPeriod(request.Id).GetAwaiter().GetResult();
                    emailTos = string.Join(";", reminders.Select(x => x.Email));
                }
                else
                {
                    subject = period.PeriodName;
                    bodyBuilder.Append($"Xin chào, <br/><br/>");
                    bodyBuilder.Append($"Bạn được chọn để đánh giá các phòng ban đợt \"<b>##PeriodName##</b>.<br/><br/>\"");
                    bodyBuilder.Append($"Thời gian thực hiện từ ##DateFrom## đến ##DateTo##. <br/><br/>");
                    bodyBuilder.Append($"Vui lòng vào được dẫn sau để đánh giá.<br/><br/>");
                    bodyBuilder.Append($"Link ##Link##.<br/><br/><br/><br/>");
                    bodyBuilder.Append($"Vui lòng không reply email này.<br/><br/>");
                    bodyBuilder.Append($"Trân trọng!<br/>");

                    var userids = period.PeriodSelectedDepartments.Select(x => x.UserId).Distinct().ToList();
                    var users = await _assessmentCriteriaRepository.GetUsersFromDB(userids);
                    emailTos = string.Join(";", users.Select(x => x.Email).ToList());
                }
                
                string body = bodyBuilder.ToString();
                body = body.Replace("##Link##", $"{_context.HttpContext.Request.Scheme}://{_context.HttpContext.Request.Host}/");
                body = body.Replace("##DateFrom##", $"{period.PeriodFrom.ToString("dd/MM/yyyy")}");
                body = body.Replace("##DateTo##", $"{period.PeriodTo.ToString("dd/MM/yyyy")}");
                body = body.Replace("##PeriodName##", $"{period.PeriodName}");

                Email email = new Email
                {
                    To = emailTos,
                    Body = body,
                    Subject = subject,
                    Status = "NEW"
                };
                _permissionsRepository.AddEmail(email);

                return await _permissionsRepository.UnitOfWork.SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}

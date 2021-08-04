using MediatR;
using Microsoft.AspNetCore.Authentication.AzureAD.UI.Pages.Internal;
using Microsoft.Extensions.Logging;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.SpaWebApp.Configuration;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Extensions;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class SaveAssessmentResultCommandHanler : IRequestHandler<SaveAssessmentResultCommand, bool>
    {
        private readonly IAssessmentPeriodRepository _assessmentPeriodRepository;
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly IIdentityClaimsHelper _identityClaimsHelper;
        private readonly IAdminSetupQueries _adminSetupQueries;
        private readonly IAssessmentPeriodQueries _assessmentPeriodQueries;
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly AppSettings _settings;
        private readonly ILogger<SaveAssessmentResultCommandHanler> _logger;

        public SaveAssessmentResultCommandHanler(IAssessmentPeriodRepository assessmentPeriodRepository,
                                                 IAssessmentCriteriaRepository assessmentCriteriaRepository,
                                                 IIdentityClaimsHelper identityClaimsHelper,
                                                 IAdminSetupQueries adminSetupQueries,
                                                 IAssessmentPeriodQueries assessmentPeriodQueries,
                                                 IPermissionsRepository permissionsRepository,
                                                 ILogger<SaveAssessmentResultCommandHanler> logger,
                                                 AppSettings settings)
        {
            _assessmentPeriodRepository = assessmentPeriodRepository;
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
            _identityClaimsHelper = identityClaimsHelper;
            _adminSetupQueries = adminSetupQueries;
            _assessmentPeriodQueries = assessmentPeriodQueries;
            _permissionsRepository = permissionsRepository;
            _settings = settings;
            _logger = logger;
        }

        public async Task<bool> Handle(SaveAssessmentResultCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.ValidateModel();

                var period = await _assessmentPeriodRepository.GetPeriodAsync(request.PeriodId);

                if (period == null) throw new DomainException("Kỳ đánh giá không tồn tại");
                if (DateTime.Today < period.PeriodFrom) throw new DomainException("Kỳ đánh giá chưa đến ngày thực hiện");
                if (period.PeriodTo < DateTime.Today) throw new DomainException("Kỳ đánh giá đã hết hạn thực hiện");
                if (period.PeriodSelectedDepartments == null || !period.PeriodSelectedDepartments.Any(x => x.Id == request.PeriodSelectedDepartmentId))
                    throw new DomainException("Phòng ban này không có trong kỳ đánh giá");

                if (request.Result.Any(x => !period.PeriodQuetions.Any(q => q.Id == x.PeriodQuetionId)))
                    throw new DomainException("Có câu hỏi không thuộc về kỳ đánh giá");

                if (request.Finished && request.Result.Any(x => !x.Result.HasValue))
                {
                    throw new DomainException("Không thể hoàn thành vì chưa đánh giá hết");
                }

                var department = period.PeriodSelectedDepartments.FirstOrDefault(x => x.Id == request.PeriodSelectedDepartmentId);
                var userid = department.UserId;
                foreach (var rs in request.Result)
                {
                    if (rs.ResultId != 0)
                    {
                        var oldEnity = await _assessmentPeriodRepository.GetResultAsync(rs.ResultId);
                        if (oldEnity != null)
                        {
                            oldEnity.Update(rs.PeriodSelectedDepartmentId, rs.PeriodQuetionId, rs.Result, rs.ResultComment, userid);
                            _assessmentPeriodRepository.UpdateResult(oldEnity);
                        }
                    }
                    else
                    {
                        var entityRs = new PeriodAssessmentResult(rs.PeriodSelectedDepartmentId, rs.PeriodQuetionId, userid, rs.Result, rs.ResultComment, userid);
                        entityRs = _assessmentPeriodRepository.AddResult(entityRs);
                    }
                }

                if (request.Finished)
                {
                    department.FinishAssessmentResult(userid);
                    _assessmentPeriodRepository.UpdateSelectedDepartment(department);

                    //send email if all Dept completed
                    SendEmailWhenDeptFinished(request, period);
                }

                return await _assessmentPeriodRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }


        private async void SendEmailWhenDeptFinished(SaveAssessmentResultCommand request, AssessmentPeriod period)
        {
            if (period != null)
            {
                var departmentSelected = period.PeriodSelectedDepartments.FirstOrDefault(x => x.Id == request.PeriodSelectedDepartmentId);
                if(departmentSelected != null)
                {
                    int deptFromId = departmentSelected.DepartmentFrom;
                    if (period.PeriodSelectedDepartments.Count(x => x.DepartmentFrom == deptFromId && x.Id != request.PeriodSelectedDepartmentId && !x.Finished) == 0)
                    {
                        var allDepts = await _assessmentCriteriaRepository.GetDepartmentsFromDB();
                        var deptFromObj = allDepts.FirstOrDefault(x => x.Id == deptFromId);
                        var deptFromCode = (deptFromObj != null ? deptFromObj.Code : deptFromId.ToString());

                        string subject = $"Phòng {deptFromCode} đã hoàn tất chương trình đánh giá CLDV nội bộ các PB";

                        StringBuilder bodyBuilder = new StringBuilder();
                        bodyBuilder.Append($"Xin chào, <br/><br/>");
                        bodyBuilder.Append($"Phòng {deptFromCode} đã hoàn thành việc đánh giá chất lượng dịch vụ phòng ban đợt \"<b>{period.PeriodName}</b>\".<br/><br/>");
                        bodyBuilder.Append($"Cám ơn tất cả các nhân viên đã hoàn thành nhiệm vụ này. <br/><br/>");
                        bodyBuilder.Append($"Vui lòng vào được dẫn sau để xem lại đánh giá.<br/><br/>");
                        bodyBuilder.Append($"Link ##Link##.<br/><br/><br/><br/>");
                        bodyBuilder.Append($"Vui lòng không reply email này.<br/><br/>");
                        bodyBuilder.Append($"Trân trọng!<br/>");
                        string body = bodyBuilder.ToString();
                        body = body.Replace("##Link##", $"{_settings.HostUrl}/");

                        var depttos = period.PeriodSelectedDepartments.Where(x => x.DepartmentFrom == deptFromId && x.Finished).Select(x => x.DepartmentTo).ToList();
                        var emailDetps = allDepts.Where(x => depttos.Contains(x.Id) && !string.IsNullOrEmpty(x.EmailHead)).Select(x => x.EmailHead).ToList();
                        if (!string.IsNullOrEmpty(deptFromObj.EmailHead)) emailDetps.Add(deptFromObj.EmailHead);


                        var emailTos = string.Join(";", emailDetps);
                        Email email = new Email
                        {
                            To = emailTos,
                            Body = body,
                            Subject = subject,
                            Status = "NEW"
                        };
                        _permissionsRepository.AddEmail(email);

                        _logger.LogInformation("Send email complete");
                    }
                }


                
            }

            
        }
    }
}

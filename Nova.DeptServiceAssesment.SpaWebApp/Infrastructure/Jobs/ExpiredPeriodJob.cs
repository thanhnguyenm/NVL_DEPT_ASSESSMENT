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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class ExpiredPeriodJob : IJob
    {
        private readonly ILogger<ExpiredPeriodJob> _logger;
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly IEmailHelper _emailHelper;
        private readonly IAdminSetupQueries _adminSetupQueries;
        private readonly AppSettings _setting;
        private readonly IAssessmentPeriodRepository _assessmentPeriodRepository;
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;

        public ExpiredPeriodJob(AppSettings setting,
                                IAssessmentCriteriaRepository assessmentCriteriaRepository,
                                ILogger<ExpiredPeriodJob> logger,
                                IEmailHelper emailHelper,
                                IAdminSetupQueries adminSetupQueries,
                                IPermissionsRepository permissionsRepository,
                                IAssessmentPeriodRepository assessmentPeriodRepository)
        {
            _setting = setting;
            _logger = logger;
            _emailHelper = emailHelper;
            _adminSetupQueries = adminSetupQueries;
            _permissionsRepository = permissionsRepository;
            _assessmentPeriodRepository = assessmentPeriodRepository;
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            
            try
            {
                _logger.LogInformation("ExpiredPeriodJob start");

                string subject = "Kết quả đánh giá từ các PB khác về sự hài lòng đối với PB ##DepartmentToName##";
                StringBuilder bodyBuilder = new StringBuilder();
                bodyBuilder.Append($"Xin chào, <br/><br/>");
                bodyBuilder.Append($"Chương trình đánh giá CLDV nội bộ của PB kỳ \"<b>##PeriodName##</b>\": Phòng ##DepartmentToName## đã được ##DepartmentFromCount## đánh giá.<br/><br/>");
                bodyBuilder.Append($"Điểm trung bình của từng câu hỏi như sau:<br/><br/>");
                bodyBuilder.Append($"##table## <br/><br/><br/><br/>");
                bodyBuilder.Append($"Vui lòng không reply email này.<br/><br/>");
                bodyBuilder.Append($"Trân trọng!<br/>");
                string body = bodyBuilder.ToString();

                var allDepts = await _assessmentCriteriaRepository.GetDepartmentsFromDB();
                var periods = await _assessmentPeriodRepository.GetExpiredPeriodsAsync();
                foreach(var p in periods)
                {
                    var periodDepartmentTos = p.PeriodSelectedDepartments.Select(x => x.DepartmentTo).Distinct().ToList();
                    foreach(var deptTo in periodDepartmentTos)
                    {
                        var deptToObj = allDepts.FirstOrDefault(x => x.Id == deptTo);
                        if (deptToObj != null && !string.IsNullOrEmpty(deptToObj.EmailHead))
                        {
                            float sumScore = 0.0f;
                            int countScore = 0;
                            var resultTable = await _adminSetupQueries.GetResultByDepartmentTo(p.Id, deptTo);
                            StringBuilder table = new StringBuilder();
                            table.Append("<div style=\"padding-left:50px\"><table style=\"border: 1px solid black;\"><thead><tr><th width=\"200\" align=\"center\" style=\"border: 1px solid black;\">Câu hỏi</th><th width=\"200\" align=\"center\" style=\"border: 1px solid black;\">Điểm Trung Bình</th></tr></thead>");
                            table.Append("<tbody>");
                            int i = 1;
                            if (resultTable != null)
                            {
                                foreach (DataRow row in resultTable.Rows)
                                {
                                    int questionIndex = 0;
                                    float temp = 0;
                                    if (float.TryParse(row.ItemArray[1].ToString(), out temp) &&
                                        int.TryParse(row.ItemArray[0].ToString(), out questionIndex))
                                    {
                                        countScore++;
                                        sumScore += temp;
                                    }
                                    table.Append($"<tr><td width=\"200\" align=\"center\" style=\"border: 1px solid black;\">{i}</td><td width=\"200\" align=\"center\" style=\"border: 1px solid black;\">{row.ItemArray[1].ToString()}</td></tr>");
                                    i++;
                                }
                            }
                            
                            table.Append($"<tr><td width=\"200\" align=\"center\" style=\"border: 1px solid black;\"><b>Điểm Trung bình tất cả các câu hỏi</b></td><td width=\"200\" align=\"center\" style=\"border: 1px solid black;\"><b>{(countScore == 0 ? 0 : sumScore / countScore)}</b></td></tr>");
                            table.Append($"</tbody></table></div>");

                            string copysubject = subject.Replace("##DepartmentToName##", (deptToObj != null ? deptToObj.Code : deptTo.ToString()));
                            string copybody = body.Replace("##DepartmentToName##", (deptToObj != null ? deptToObj.Code : deptTo.ToString()));
                            copybody = copybody.Replace("##DepartmentFromCount##", p.PeriodSelectedDepartments.Count(x => x.DepartmentTo == deptTo).ToString());
                            copybody = copybody.Replace("##PeriodName##", p.PeriodName);
                            copybody = copybody.Replace("##table##", table.ToString());


                            Email email = new Email
                            {
                                To = deptToObj.EmailHead,
                                Body = copybody,
                                Subject = copysubject,
                                Status = "NEW"
                            };
                            _permissionsRepository.AddEmail(email);
                        }
                        
                    }


                }

                await _permissionsRepository.UnitOfWork.SaveEntitiesAsync();
              
                _logger.LogInformation("ExpiredPeriodJob complete");
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}

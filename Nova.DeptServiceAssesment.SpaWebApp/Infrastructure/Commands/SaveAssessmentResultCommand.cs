using MediatR;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Extensions;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class SaveAssessmentResultCommand : IRequest<bool>
    {
        public SaveAssessmentResultCommand()
        {

        }

        public int PeriodId { get; set; }

        public int PeriodSelectedDepartmentId { get; set; }

        public bool Finished { get; set; }

        public UserAssessmentQuestionModel[] Result { get; set; }

        public void ValidateModel()
        {
            if (Result == null) throw new DomainException("Dữ liệu không hợp lệ");

            int firstUserValue = Result.FirstOrDefault().PeriodSelectedDepartmentId;
            foreach(var rs in Result)
            {
                if (rs.Result.HasValue && (rs.Result.Value == 1 || rs.Result.Value == 2) && string.IsNullOrEmpty(rs.ResultComment))
                    throw new DomainException("Lý do không được để trông nếu đánh giá không hài lòng");

                if(firstUserValue != rs.PeriodSelectedDepartmentId)
                    throw new DomainException("Thông tin user phải duy nhất");
            }

        }
    }
}

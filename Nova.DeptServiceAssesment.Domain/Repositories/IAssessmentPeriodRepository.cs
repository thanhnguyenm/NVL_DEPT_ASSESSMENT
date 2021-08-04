using Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate;
using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.Domain.Repositories
{
    public interface IAssessmentPeriodRepository : IRepository<AssessmentPeriod>
    {

        AssessmentPeriod AddPeriod(AssessmentPeriod period);

        void UpdatePeriod(AssessmentPeriod period);

        Task<AssessmentPeriod> GetPeriodAsync(int periodId);

        Task<List<AssessmentPeriod>> GetExpiredPeriodsAsync();

        void UpdateSelectedDepartment(PeriodSelectedDepartment dept);

        Task<PeriodAssessmentResult> GetResultAsync(int resultId);

        PeriodAssessmentResult AddResult(PeriodAssessmentResult result);

        void UpdateResult(PeriodAssessmentResult result);

    }
}

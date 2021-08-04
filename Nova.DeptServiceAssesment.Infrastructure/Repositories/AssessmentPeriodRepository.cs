using Microsoft.EntityFrameworkCore;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.Infrastructure.Repositories
{
    public class AssessmentPeriodRepository : IAssessmentPeriodRepository
    {
        private readonly AssessmentContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public AssessmentPeriodRepository(AssessmentContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<AssessmentPeriod>  GetPeriodAsync(int periodId)
        {
            var period = await _context
                                .AssessmentPeriods
                                .Include(x => x.PeriodQuetions)
                                .Include(x => x.PeriodSelectedDepartments).ThenInclude(x => x.PeriodAssessmentResults)
                                .FirstOrDefaultAsync(o => o.Id == periodId);


            return period;
        }

        public async Task<List<AssessmentPeriod>> GetExpiredPeriodsAsync()
        {
            var periods = await _context
                                .AssessmentPeriods
                                .Include(x => x.PeriodQuetions)
                                .Include(x => x.PeriodSelectedDepartments).ThenInclude(x => x.PeriodAssessmentResults)
                                .Where(x => x.PeriodTo.Date.AddDays(1) == DateTime.Today).ToListAsync();


            return periods;
        }

        public AssessmentPeriod AddPeriod(AssessmentPeriod period)
        {
            return _context.AssessmentPeriods.Add(period).Entity;
        }


        public void UpdatePeriod(AssessmentPeriod period)
        {
            _context.Entry(period).State = EntityState.Modified;
        }


        public void UpdateSelectedDepartment(PeriodSelectedDepartment dept)
        {
            _context.Entry(dept).State = EntityState.Modified;
        }

        public async Task<PeriodAssessmentResult> GetResultAsync(int resultId)
        {
            var result = await _context
                                .PeriodAssessmentResults
                                .FirstOrDefaultAsync(o => o.Id == resultId);
            return result;
        }

        public PeriodAssessmentResult AddResult(PeriodAssessmentResult result)
        {
            return _context.PeriodAssessmentResults.Add(result).Entity;
        }

        public void UpdateResult(PeriodAssessmentResult result)
        {
            _context.Entry(result).State = EntityState.Modified;
        }

    }
}

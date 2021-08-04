using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate
{
    public class PeriodSelectedDepartment : Entity
    {
        public int AssessmentPeriodId { get; private set; }

        public int DepartmentFrom { get; private set; }

        public int UserId { get; private set; }

        public bool Finished { get; private set; }

        public int DepartmentTo { get; private set; }

        private List<PeriodAssessmentResult> _periodAssessmentResult;

        public IEnumerable<PeriodAssessmentResult> PeriodAssessmentResults
        {
            get => _periodAssessmentResult;
            set { _periodAssessmentResult = value.ToList(); }
        }

        public PeriodSelectedDepartment()
        {
            AssessmentPeriodId = 0;
            _periodAssessmentResult = new List<PeriodAssessmentResult>();
            SetCreateInfo(0);
        }

        public PeriodSelectedDepartment(int peroid, int departentFrom, int departentTo, int userid, int ulogid) : this()
        {
            AssessmentPeriodId = peroid;
            DepartmentFrom = departentFrom;
            DepartmentTo = departentTo;
            UserId = userid;

            SetCreateInfo(ulogid);
        }

        public PeriodAssessmentResult AddResult(int _periodSelectedUserId, int periodUQuestionId, int? result, string comment, int userid, int ulogid)
        {
            var existingResult = _periodAssessmentResult.SingleOrDefault(p => p.IsEqualTo(_periodSelectedUserId, periodUQuestionId));

            if (existingResult != null)
            {
                //AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

                return existingResult;
            }

            var newRs = new PeriodAssessmentResult(0, periodUQuestionId, this.UserId, result, comment, userid);

            _periodAssessmentResult.Add(newRs);

            //AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

            SetModifiedInfo(ulogid);

            return newRs;
        }

        public void FinishAssessmentResult(int userid)
        {
            Finished = true;

            SetModifiedInfo(userid);
        }

        public bool IsEqualTo(int departentFrom, int departentTo, int userid)
        {
            return DepartmentFrom == departentFrom && DepartmentTo == departentTo && UserId == userid;
        }
    }
}


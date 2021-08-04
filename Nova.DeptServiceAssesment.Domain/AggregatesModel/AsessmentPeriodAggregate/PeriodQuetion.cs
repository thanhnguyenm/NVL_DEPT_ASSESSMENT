using Nova.DeptServiceAssesment.Domain.AggregatesModel.AssessmentCriteriaAggregate;
using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate
{
    public class PeriodQuetion : Entity
    {
        public int AssessmentPeriodId { get; private set; }

        public int AssessmentQuestionId { get; private set; }

        public AssessmentQuestion AssessmentQuestion { get; private set; }

        public PeriodQuetion()
        {
            AssessmentPeriodId = 0;
            AssessmentQuestionId = 0;
            SetCreateInfo(0);
        }

        public PeriodQuetion(int peroid, int questionid, int userid) : this()
        {
            AssessmentPeriodId = peroid;
            AssessmentQuestionId = questionid;

            SetCreateInfo(userid);
        }

        public bool IsEqualTo(int questionid)
        {
            return AssessmentQuestionId == questionid;
        }
    }
}

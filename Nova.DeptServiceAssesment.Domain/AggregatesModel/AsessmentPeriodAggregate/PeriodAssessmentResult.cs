using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate
{
    public class PeriodAssessmentResult : Entity
    {
        public int PeriodSelectedDepartmentId { get; private set; }

        public int PeriodQuestionId { get; private set; }


        public int? Result { get; private set; }

        public string ResultComment { get; private set; }

        protected PeriodAssessmentResult()
        {
            SetCreateInfo(0);
        }

        public PeriodAssessmentResult(int periodSelectedUserId, int questionId, int selecteduserid, int? result, string comment, int userid) : this()
        {
            PeriodSelectedDepartmentId = periodSelectedUserId;
            PeriodQuestionId = questionId;
            Result = result;
            ResultComment = comment;

            SetCreateInfo(userid);
        }

        public void Update(int periodSelectedDepartmentI, int questionId, int? result, string comment, int userid)
        {
            PeriodSelectedDepartmentId = periodSelectedDepartmentI;
            PeriodQuestionId = questionId;
            Result = result;
            ResultComment = comment;

            SetModifiedInfo(userid);
        }

        public bool IsEqualTo(int periodSelectedDepartmentI, int questionId)
        {
            return PeriodSelectedDepartmentId == periodSelectedDepartmentI && PeriodQuestionId == questionId;
        }
    }
}

using Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate;
using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.AggregatesModel.AssessmentCriteriaAggregate
{
    public class AssessmentQuestion : Entity
    {
        public string Content { get; private set; }

        public int CriteriaId { get; private set; }

        public AssessmentQuestion()
        {
            SetCreateInfo(0);
        }

        public AssessmentQuestion(string content, int userid) : this()
        {
            Content = !string.IsNullOrWhiteSpace(content) ? content : throw new ArgumentNullException(nameof(Content));
            SetCreateInfo(0);
        }

        public AssessmentQuestion(string content, int criteriaid, int userid) : this()
        {
            Content = !string.IsNullOrWhiteSpace(content) ? content : throw new ArgumentNullException(nameof(Content));
            CriteriaId = criteriaid;

            SetCreateInfo(userid);
        }

        public void Update(string content, int userid)
        {
            Content = content;

            SetModifiedInfo(userid);
        }

        public bool IsEqualTo(string content)
        {
            return Content.ToLower() == content.ToLower();
        }
    }
}

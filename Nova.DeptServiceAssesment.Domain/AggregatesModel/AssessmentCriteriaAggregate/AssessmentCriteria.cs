using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.AggregatesModel.AssessmentCriteriaAggregate
{
    public class AssessmentCriteria : Entity, IAggregateRoot
    {
        public string CriteriaName { get; private set; }

        private List<AssessmentQuestion> _assessmentQuestions;

        public IEnumerable<AssessmentQuestion> AssessmentQuestions => _assessmentQuestions;

        protected AssessmentCriteria()
        {
            _assessmentQuestions = new List<AssessmentQuestion>();
            SetCreateInfo(0);
        }

        public AssessmentCriteria(string name, int userid) : this()
        {
            CriteriaName = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(CriteriaName));

            SetCreateInfo(userid);
        }

        public AssessmentQuestion AddQuestion(string content, int userid)
        {
            var existingQuestion = _assessmentQuestions.SingleOrDefault(p => p.IsEqualTo(content));

            if (existingQuestion != null)
            {
                //AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

                return existingQuestion;
            }

            var question = new AssessmentQuestion(content, userid);

            _assessmentQuestions.Add(question);

            //AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

            SetModifiedInfo(userid);

            return question;
        }

    }
}

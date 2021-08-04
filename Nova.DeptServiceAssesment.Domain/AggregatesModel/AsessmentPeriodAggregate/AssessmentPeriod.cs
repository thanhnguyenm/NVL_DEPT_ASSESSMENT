using Nova.DeptServiceAssesment.Domain.Exceptions;
using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate
{
    public class AssessmentPeriod : Entity, IAggregateRoot
    {
        public string PeriodName { get; private set; }
        public DateTime PeriodFrom { get; private set; }
        public DateTime PeriodTo { get; private set; }
        public bool Published { get; private set; }
        public string Note { get; private set; }

        private List<PeriodQuetion> _periodQuetions;

        private List<PeriodSelectedDepartment> _periodSelectedDepartments;

        public IEnumerable<PeriodQuetion> PeriodQuetions => _periodQuetions;

        public IEnumerable<PeriodSelectedDepartment> PeriodSelectedDepartments => _periodSelectedDepartments;

        protected AssessmentPeriod()
        {
            _periodQuetions = new List<PeriodQuetion>();
            _periodSelectedDepartments = new List<PeriodSelectedDepartment>();

            SetCreateInfo(0);
        }

        public AssessmentPeriod(string name, DateTime from, DateTime to, bool published, string note, int userid) : this()
        {
            PeriodName = !string.IsNullOrEmpty(name) ? name : throw new AssessmentPeriodException(nameof(PeriodName));
            if (from > to) throw new AssessmentPeriodException(nameof(PeriodFrom));

            PeriodFrom = from;
            PeriodTo = to;
            Published = published;
            Note = note;

            SetCreateInfo(userid);
        }

        public void Update(string name, DateTime from, DateTime to, bool published, string note, int userid)
        {
            PeriodName = !string.IsNullOrEmpty(name) ? name : throw new AssessmentPeriodException(nameof(PeriodName));
            if (from > to) throw new AssessmentPeriodException(nameof(PeriodFrom));

            PeriodFrom = from;
            PeriodTo = to;
            Published = published;
            Note = note;

            SetModifiedInfo(userid);
        }

        public PeriodQuetion AddQuestion(int questionid, int userid)
        {
            var existingQuestion = _periodQuetions.SingleOrDefault(p => p.IsEqualTo(questionid));

            if (existingQuestion != null)
            {
                //AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

                return existingQuestion;
            }

            var question = new PeriodQuetion(0, questionid, userid);

            _periodQuetions.Add(question);

            //AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

            SetModifiedInfo(userid);

            return question;
        }

        public PeriodSelectedDepartment AddSelectedDepartment(int departmentFrom, int departmentTo, int userid, int ulogid)
        {
            var existingDep = _periodSelectedDepartments.SingleOrDefault(p => p.IsEqualTo(departmentFrom, departmentTo, userid));

            if (existingDep != null)
            {
                //AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

                return existingDep;
            }

            var department = new PeriodSelectedDepartment(0, departmentFrom, departmentTo, userid, ulogid);

            _periodSelectedDepartments.Add(department);

            //AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

            SetModifiedInfo(userid);

            return department;
        }

        public PeriodSelectedDepartment AddSelectedDepartment(PeriodSelectedDepartment _department, int userid)
        {
            var existingDep = _periodSelectedDepartments.SingleOrDefault(p => p.IsEqualTo(_department.DepartmentFrom, _department.DepartmentTo, _department.UserId));

            if (existingDep != null)
            {
                //AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

                return existingDep;
            }

            _periodSelectedDepartments.Add(_department);

            //AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

            SetModifiedInfo(userid);

            return _department;
        }
    }
}


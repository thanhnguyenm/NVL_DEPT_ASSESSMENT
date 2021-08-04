using System;
using System.Collections.Generic;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.Exceptions
{
    public class AssessmentPeriodException : Exception
    {
        public AssessmentPeriodException()
        { }

        public AssessmentPeriodException(string message)
            : base(message)
        { }

        public AssessmentPeriodException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}

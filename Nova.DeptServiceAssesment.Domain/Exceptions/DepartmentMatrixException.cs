using System;
using System.Collections.Generic;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.Exceptions
{
    public class DepartmentMatrixException : Exception
    {
        public DepartmentMatrixException()
        { }

        public DepartmentMatrixException(string message)
            : base(message)
        { }

        public DepartmentMatrixException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}

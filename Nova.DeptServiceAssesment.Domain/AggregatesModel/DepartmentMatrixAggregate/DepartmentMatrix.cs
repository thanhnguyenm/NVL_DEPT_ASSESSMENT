using Nova.DeptServiceAssesment.Domain.Exceptions;
using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nova.DeptServiceAssesment.Domain.AggregatesModel.DepartmentMatrixAggregate
{
    public class DepartmentMatrix : Entity, IAggregateRoot
    {
        public string DepartmentFrom { get; private set; }
        public string DepartmentTo { get; private set; }
        public int Interact { get; private set; }
        public string Note { get; private set; }

        protected DepartmentMatrix()
        {
            SetCreateInfo(0);
        }

        public DepartmentMatrix(string from, string to, int interact, string note, int userid) : this()
        {
            DepartmentFrom = !string.IsNullOrEmpty(from) ? from : throw new DepartmentMatrixException(nameof(DepartmentFrom));
            DepartmentTo = !string.IsNullOrEmpty(to) ? to : throw new DepartmentMatrixException(nameof(DepartmentTo));
            Interact = interact >= 1 && interact <= 3 ? interact : throw new DepartmentMatrixException(nameof(Interact));

            SetCreateInfo(userid);
        }

        public void SetInteract(int interract, int userid)
        {
            Interact = interract;
            SetModifiedInfo(userid);
        }
    }
}


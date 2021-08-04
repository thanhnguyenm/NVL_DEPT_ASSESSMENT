using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class SyncDepartmentsCommand : IRequest<bool>
    {
        public SyncDepartmentsCommand()
        {

        }

    }
}

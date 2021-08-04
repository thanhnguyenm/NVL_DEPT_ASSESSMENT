using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class ImportDepartmentsCommand : IRequest<bool>
    {
        public ImportDepartmentsCommand()
        {

        }

        public FileStream Stream { get; set; }


    }
}

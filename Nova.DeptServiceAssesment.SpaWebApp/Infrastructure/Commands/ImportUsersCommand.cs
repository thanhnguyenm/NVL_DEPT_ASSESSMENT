using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class ImportUsersCommand : IRequest<bool>
    {
        public ImportUsersCommand()
        {

        }

        public FileStream Stream { get; set; }


    }
}

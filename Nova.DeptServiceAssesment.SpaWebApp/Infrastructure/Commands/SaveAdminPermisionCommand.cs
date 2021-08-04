using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class SaveAdminPermisionCommand : IRequest<bool>
    {
        public SaveAdminPermisionCommand()
        {

        }

        public string Email { get; set; }

    }
}

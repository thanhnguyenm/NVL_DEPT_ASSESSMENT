using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class NotifyAdminPeriodCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public bool IsReminder { get; set; }
    }
}

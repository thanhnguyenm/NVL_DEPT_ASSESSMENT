using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class SaveAdminPeriodCommand : IRequest<int>
    {
        public SaveAdminPeriodCommand()
        {

        }

        public int Id { get; set; }
        public string PeriodName { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public bool Published { get; set; }

        public List<int> SelectedQuestions { get; set; }

    }
}

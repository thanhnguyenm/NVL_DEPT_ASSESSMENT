using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper
{
    public interface IEmailHelper
    {
        bool SendEmail(string emailto, string subject, string body);

        bool SendEmail(List<string> emailtos, string subject, string body);
    }
}

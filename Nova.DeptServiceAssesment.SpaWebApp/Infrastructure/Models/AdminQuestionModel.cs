using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models
{
    public class AdminQuestionModel
    {
        public int Id { get; set; }
        public int CriteriaId { get; set; }
        public string CriteriaName { get; set; }
        public string Content { get; set; }
    }
}

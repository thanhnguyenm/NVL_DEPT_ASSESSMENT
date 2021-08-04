using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models
{
    [DataContract]
    public class UserAssessmentQuestionModel
    {
        [DataMember]
        public int ResultId { get; set; }
        [DataMember]
        public int PeriodSelectedDepartmentId { get; set; }
        [DataMember]
        public int PeriodQuetionId { get; set; }
        [DataMember]
        public string QuestionContent { get; set; }
        [DataMember]
        public int CriteriaId { get; set; }
        [DataMember]
        public string CriteriaName { get; set; }
        [DataMember]
        public DateTime PeriodFrom { get; set; }
        [DataMember]
        public DateTime PeriodTo { get; set; }
        [DataMember]
        public int? Result { get; set; }
        [DataMember]
        public string ResultComment { get; set; }
        [DataMember]
        public bool Finished { get; set; }
        [DataMember]
        public bool CanEdit { get; set; }
    }
}

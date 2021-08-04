﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models
{
    public class AdminPeriodModel
    {
        public int Id { get; set; }
        public string PeriodName { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public string Note { get; set; }
        public bool Published { get; set; }

        public List<int> Questions { get; set; }
    }
}


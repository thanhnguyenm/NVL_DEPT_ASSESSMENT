using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models
{
    public class PagedList<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalRecords { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }

        public PagedList(){ }

        public PagedList(IEnumerable<T> data, int totalRecords, int page, int itemsPerPage) 
        {
            Data = data;
            TotalRecords = totalRecords;
            Page = page;
            ItemsPerPage = itemsPerPage;
        }
    }
}

using MediatR;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class SaveQuestionCommand : IRequest<int>
    {
        public SaveQuestionCommand()
        {

        }

        public int Id { get; set; }

        public int CriteriaId { get; set; }

        public string Content { get; set; }


        public void ValidateModel()
        {
            if (CriteriaId == 0) throw new DomainException("Tiêu chuẩn không tồn tại");
            if (String.IsNullOrEmpty(Content) || String.IsNullOrWhiteSpace(Content)) throw new DomainException("Nội dung không được trống");
        }
    }
}


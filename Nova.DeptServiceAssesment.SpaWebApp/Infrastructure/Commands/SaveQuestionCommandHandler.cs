using MediatR;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AssessmentCriteriaAggregate;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Extensions;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class SaveQuestionCommandHandler : IRequestHandler<SaveQuestionCommand, int>
    {
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly IIdentityClaimsHelper _identityClaimsHelper;

        public SaveQuestionCommandHandler(IAssessmentCriteriaRepository assessmentCriteriaRepository,
                                                 IIdentityClaimsHelper identityClaimsHelper)
        {
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
            _identityClaimsHelper = identityClaimsHelper;
        }

        public async Task<int> Handle(SaveQuestionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.ValidateModel();
                int userid = _identityClaimsHelper.CurrentUser.Id;
                AssessmentQuestion entityQuestion = null;

                var criteria = await _assessmentCriteriaRepository.GetCriteriaAsync(request.CriteriaId);
                if (criteria == null) throw new DomainException("Không tồn tại lại tiêu chuẩn này");

                if (request.Id != 0)
                {
                    entityQuestion = criteria.AssessmentQuestions.FirstOrDefault(x => x.Id == request.Id);
                    if (entityQuestion != null)
                    {
                        entityQuestion.Update(request.Content, userid);
                    }
                    else
                    {
                        request.Id = 0;
                    }
                }

                if (request.Id == 0)
                {
                    entityQuestion = criteria.AddQuestion(request.Content, userid);
                }

                await _assessmentCriteriaRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                return entityQuestion.Id;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}

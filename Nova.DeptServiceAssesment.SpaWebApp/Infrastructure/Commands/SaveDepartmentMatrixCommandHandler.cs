using ExcelDataReader;
using MediatR;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.DepartmentMatrixAggregate;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class SaveDepartmentMatrixCommandHandler : IRequestHandler<SaveDepartmentMatrixCommand, bool>
    {
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly IIdentityClaimsHelper _identityClaimsHelper;
        private readonly IUserOrgHelper _userService;

        public SaveDepartmentMatrixCommandHandler(IAssessmentCriteriaRepository assessmentCriteriaRepository,
                                                 IIdentityClaimsHelper identityClaimsHelper,
                                                 IUserOrgHelper userService)
        {
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
            _identityClaimsHelper = identityClaimsHelper;
            _userService = userService;
        }

        public async Task<bool> Handle(SaveDepartmentMatrixCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var lst = new List<DepartmentMatrix>();
                var oldMatrix = await _assessmentCriteriaRepository.GetDepartmentMatrix();
                var needUpdateMatrix = new List<DepartmentMatrix>();
                var keepMatrix = new List<DepartmentMatrix>();

                if (request.Stream != null && request.Stream.Length > 0)
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx)
                    using (var reader = ExcelReaderFactory.CreateReader(request.Stream))
                    {
                        // Use the AsDataSet extension method
                        var result = reader.AsDataSet();

                        // The result of each spreadsheet is in result.Tables

                        var firstRowArray = result.Tables[0].Rows[0].ItemArray;
                        if (result.Tables[0] != null)
                        {
                            for (int i = 1; i < result.Tables[0].Rows.Count; i++)
                            {
                                var arrItems = result.Tables[0].Rows[i].ItemArray;
                                var rowHeader = _userService.FormatDepartmentCode(arrItems[0].ToString());
                                for (int j = 1; j < result.Tables[0].Columns.Count; j++)
                                {
                                    var colHeader = _userService.FormatDepartmentCode(firstRowArray[j].ToString());
                                    if (!lst.Any(x => x.DepartmentFrom == rowHeader && x.DepartmentTo == colHeader)
                                        && arrItems[j] != null && !string.IsNullOrEmpty(arrItems[j].ToString())
                                        && int.TryParse(arrItems[j].ToString(), out int interact))
                                    {
                                        var oldOne = oldMatrix.FirstOrDefault(x => x.DepartmentFrom == rowHeader && x.DepartmentTo == colHeader);
                                        if(oldOne == null)
                                        {
                                            lst.Add(new DepartmentMatrix(rowHeader, colHeader, interact, string.Empty, 0));
                                        }
                                        else if (oldOne != null && oldOne.Interact != interact)
                                        {
                                            oldOne.SetInteract(interact, _identityClaimsHelper.CurrentUser.Id);
                                            oldOne.SetDelete(false, _identityClaimsHelper.CurrentUser.Id);
                                            needUpdateMatrix.Add(oldOne);
                                        }
                                        else if (oldOne != null && oldOne.Interact == interact)
                                        {
                                            oldOne.SetDelete(false, _identityClaimsHelper.CurrentUser.Id);
                                            keepMatrix.Add(oldOne);
                                        }
                                    }

                                }
                            }
                        }

                    }
                }

                var needDelete = oldMatrix.Where(x => !keepMatrix.Any(y => y.Id == x.Id) && !needUpdateMatrix.Any(y => y.Id == x.Id)).ToList();
                foreach (var m in needDelete)
                {
                    m.SetDelete(true, _identityClaimsHelper.CurrentUser.Id);
                }
                _assessmentCriteriaRepository.UpdateDepartmentMatrix(needDelete);
                _assessmentCriteriaRepository.UpdateDepartmentMatrix(needUpdateMatrix);

                await _assessmentCriteriaRepository.SaveDepartmentMatrix(lst);

                return await _assessmentCriteriaRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}

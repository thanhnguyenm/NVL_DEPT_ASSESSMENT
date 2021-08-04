using ExcelDataReader;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands
{
    public class ImportDepartmentsCommandHandler : IRequestHandler<ImportDepartmentsCommand, bool>
    {
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly IIdentityClaimsHelper _identityClaimsHelper;
        private readonly IUserOrgHelper _userService;
        private readonly ILogger<ImportDepartmentsCommandHandler> _logger;

        public ImportDepartmentsCommandHandler(IAssessmentCriteriaRepository assessmentCriteriaRepository,
                                                 IIdentityClaimsHelper identityClaimsHelper,
                                                 ILogger<ImportDepartmentsCommandHandler> logger,
                                                IUserOrgHelper userService)
        {
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
            _identityClaimsHelper = identityClaimsHelper;
            _userService = userService;
            _logger = logger;
        }

        public async Task<bool> Handle(ImportDepartmentsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(request.Stream))
                {
                    // Use the AsDataSet extension method
                    var result = reader.AsDataSet();

                    // The result of each spreadsheet is in result.Tables
                    if (result.Tables[0] != null)
                    {
                        int currentuserid = _identityClaimsHelper.CurrentUser.Id;

                        var oldDepartments = await _assessmentCriteriaRepository.GetDepartmentsFromDB();
                        var updateList = new List<Department>();
                        var addList = new List<Department>();

                        for (int i = 1; i < result.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                var arrItems = result.Tables[0].Rows[i].ItemArray;

                                int idDept;
                                string shortCode = string.Empty;
                                var tempdept = new Department(0, arrItems[2].ToString(), arrItems[2].ToString(), "PB", arrItems[1].ToString(), arrItems[1].ToString(), arrItems[3].ToString(), 0);
                                Department existing = null;

                                if(int.TryParse(arrItems[0].ToString(), out idDept))
                                {
                                    existing = oldDepartments.FirstOrDefault(x => x.Id == idDept);   
                                }
                                else
                                {
                                    
                                    existing = oldDepartments.FirstOrDefault(x => x.ShortCode.ToUpper().TrimEnd().Replace(" ", "") == tempdept.ShortCode.ToUpper().TrimEnd().Replace(" ", ""));
                                }

                                if (existing != null)
                                {
                                    existing.Update(arrItems[2].ToString(), arrItems[2].ToString(), "PB", arrItems[1].ToString(), arrItems[1].ToString(), arrItems[3].ToString(), 0);
                                    updateList.Add(existing);
                                }
                                else
                                {
                                    addList.Add(tempdept);
                                }


                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);

                            }
                        }

                        var needDeleted = oldDepartments.Where(x => !updateList.Contains(x)).ToList();
                        needDeleted.ForEach(x =>
                        {
                            x.SetDelete(true, currentuserid);
                        });

                        await _assessmentCriteriaRepository.AddDepartments(addList);
                        _assessmentCriteriaRepository.UpdateDepartments(updateList);

                    }
                }

                return await _assessmentCriteriaRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}

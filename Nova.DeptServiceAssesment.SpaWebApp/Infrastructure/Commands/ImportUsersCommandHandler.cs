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
    public class ImportUsersCommandHandler : IRequestHandler<ImportUsersCommand, bool>
    {
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        private readonly IIdentityClaimsHelper _identityClaimsHelper;
        private readonly IUserOrgHelper _userService;
        private readonly ILogger<ImportUsersCommandHandler> _logger;

        public ImportUsersCommandHandler(IAssessmentCriteriaRepository assessmentCriteriaRepository,
                                                 IIdentityClaimsHelper identityClaimsHelper,
                                                 ILogger<ImportUsersCommandHandler> logger,
                                                IUserOrgHelper userService)
        {
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
            _identityClaimsHelper = identityClaimsHelper;
            _userService = userService;
            _logger = logger;
        }

        public async Task<bool> Handle(ImportUsersCommand request, CancellationToken cancellationToken)
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

                        var oldUsers = await _assessmentCriteriaRepository.GetUsersFromDB();
                        var updateList = new List<User>();
                        var addList = new List<User>();
                        var keepList = new List<User>();
                        DateTime tempDate = DateTime.MaxValue;

                        for (int i = 1; i < result.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                var arrItems = result.Tables[0].Rows[i].ItemArray;


                                var oldUser = oldUsers.FirstOrDefault(x => x.OrgUserCode == arrItems[0].ToString());
                                if (oldUser == null)
                                {
                                    if (!addList.Any(x => x.OrgUserCode == arrItems[0].ToString()))
                                    {
                                        try
                                        {
                                            
                                            var newUser = new User(arrItems[0].ToString(), arrItems[1].ToString(), "NULL", arrItems[2].ToString(),
                                                                        arrItems[3].ToString(), arrItems[4].ToString(), arrItems[5].ToString(), arrItems[6].ToString(),
                                                                        tempDate, string.Empty, string.Empty, string.Empty, "false", currentuserid);
                                            addList.Add(newUser);
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                                        }
                                    }
                                }
                                else if (
                                        oldUser.FullName != arrItems[1].ToString() ||
                                        oldUser.DepartmentCode != int.Parse(arrItems[2].ToString()) ||
                                        oldUser.JobTitle != arrItems[3].ToString() ||
                                        oldUser.Email != arrItems[5].ToString() ||
                                        oldUser.JobLevel != int.Parse(arrItems[6].ToString()))
                                {
                                    oldUser.Update(arrItems[0].ToString(), arrItems[1].ToString(), "NULL", int.Parse(arrItems[2].ToString()),
                                                                        arrItems[3].ToString(), arrItems[4].ToString(), arrItems[5].ToString(), int.Parse(arrItems[6].ToString()),
                                                                        tempDate, string.Empty, string.Empty, string.Empty, "false", currentuserid);

                                    oldUser.SetDelete(false, currentuserid);
                                    updateList.Add(oldUser);
                                }
                                else
                                {
                                    keepList.Add(oldUser);
                                }


                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);

                            }
                        }

                        var needDeleted = oldUsers.Where(x => !updateList.Contains(x) && !keepList.Contains(x)).ToList();
                        needDeleted.ForEach(x =>
                        {
                            x.SetDelete(true, currentuserid);
                        });

                        await _assessmentCriteriaRepository.AddUsers(addList);
                        _assessmentCriteriaRepository.UpdateUsers(updateList);
                        _assessmentCriteriaRepository.UpdateUsers(needDeleted);

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

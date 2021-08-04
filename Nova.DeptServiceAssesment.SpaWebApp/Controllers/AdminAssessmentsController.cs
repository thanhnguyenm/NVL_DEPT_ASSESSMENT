using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nova.DeptServiceAssesment.SpaWebApp.Configuration;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AdminAssessmentsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserOrgHelper _helper;
        private readonly IAdminSetupQueries _adminAssessmentQueries;
        private readonly IIdentityClaimsHelper _identityClaimsHelper;
        private readonly ILogger<AdminAssessmentsController> _logger;
        private readonly AppSettings _settings;

        public AdminAssessmentsController(IMediator mediator,
                                         IUserOrgHelper helper,
                                         IAdminSetupQueries adminAssessmentQueries,
                                         IIdentityClaimsHelper claimsHelper,
                                         AppSettings setting,
                                         ILogger<AdminAssessmentsController> logger)
        {
            _mediator = mediator;
            _helper = helper;
            _adminAssessmentQueries = adminAssessmentQueries;
            _identityClaimsHelper = claimsHelper;
            _logger = logger;
            _settings = setting;
        }

        [HttpGet("questions")]
        public async Task<PagedList<AdminQuestionModel>> AdminAssessmentQuestions([FromQuery]int activePage, [FromQuery]int itemsPerPage)
        {
            return await _adminAssessmentQueries.GetAdminQuestionsAsync(activePage, itemsPerPage);
        }

        [HttpGet("questions/{id}")]
        public async Task<AdminQuestionModel> AdminAssessmentQuestionByid(int id)
        {
            return await _adminAssessmentQueries.GetAdminQuestionByIdAsync(id);
        }


        [HttpGet("criteria")]
        public async Task<PagedList<QuestionCriteriaModel>> AdminQuestionCriteria()
        {
            return await _adminAssessmentQueries.GetQuestionCriteriaAsync();
        }

        [HttpPost("questions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SaveAdminQuestion([FromBody] SaveQuestionCommand model)
        {
            if (_identityClaimsHelper.CurrentUser != null)
            {
                int newId = await _mediator.Send(model);
                return Ok(newId);
            }

            return BadRequest();
        }


        [HttpGet("departments/matrix")]
        public async Task<DataTable> AdminDepartmentMatrix()
        {
            return await _adminAssessmentQueries.GetDepartmentMatrixAsync();
        }

        [HttpPost("departments/matrix/import")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ImportAdminDepartmentMatrix(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);

                    await _mediator.Send(new SaveDepartmentMatrixCommand() { Stream = stream });
                    return Ok();
                }
            }

            return BadRequest();
        }


        [HttpGet("periods")]
        public async Task<PagedList<AdminPeriodModel>> UserAssessmentPeriods([FromQuery] int activePage, [FromQuery] int itemsPerPage)
        {
            if (_identityClaimsHelper.CurrentUser != null)
            {
                return await _adminAssessmentQueries.GetAdminAssessmentPeriodsAsync(activePage, itemsPerPage);
            }

            return new PagedList<AdminPeriodModel>();
        }

        [HttpGet("periods/{id}")]
        public async Task<AdminPeriodModel> AdminPeriodByid(int id)
        {
            var period = await _adminAssessmentQueries.GetAdminPeriodByIdAsync(id);
            period.Questions = await _adminAssessmentQueries.GetAdminQuestionsPeriod(id);

            return period;
        }

        [HttpGet("periods/{id}/departments")]
        public async Task<PagedList<AdminPeriodDepartmentModel>> AdminPeriodDepartments(int id, [FromQuery] int activePage, [FromQuery] int itemsPerPage)
        {
            var rs = await _adminAssessmentQueries.GetAdminPeriodDepartmentsAsync(id, activePage, itemsPerPage);

            return rs;
        }

        [HttpPost("periods")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SaveAdminPeriod([FromBody] SaveAdminPeriodCommand model)
        {
            if (_identityClaimsHelper.CurrentUser != null)
            {
                int newId = await _mediator.Send(model);
                return Ok(newId);
            }

            return BadRequest();
        }

        [HttpPost("periods/{id}/notify")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> NotifyAdminPeriod([FromBody] NotifyAdminPeriodCommand model)
        {
            if (_identityClaimsHelper.CurrentUser != null)
            {
                var rs = await _mediator.Send(model);
                return Ok(rs);
            }

            return BadRequest();
        }

        [HttpPost("periods/{id}/remind")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RemindAdminPeriod([FromBody] NotifyAdminPeriodCommand model)
        {
            if (_identityClaimsHelper.CurrentUser != null)
            {
                model.IsReminder = true;

                var rs = await _mediator.Send(model);
                return Ok(rs);
            }

            return BadRequest();
        }

        [HttpGet("reports/{type}/periods/{periodId}/departments/{deptid}")]
        public async Task<DataTable> AdminReports(int type, int periodId, int deptid)
        {
            var dt = await _adminAssessmentQueries.GetReports(type, periodId, deptid);
                       

            return dt;
        }

        [HttpGet("departments")]
        public async Task<PagedList<AdminDepartmentModel>> AdminDepartments([FromQuery] string q, [FromQuery] int activePage, [FromQuery] int itemsPerPage)
        {
            var rs = await _adminAssessmentQueries.GetAdminDepartmentsAsync(q, activePage, itemsPerPage);

            return rs;
        }

        [HttpPost("departments/sync")]
        public async Task<IActionResult> AdminSyncDepartments()
        {
            if (_identityClaimsHelper.CurrentUser != null)
            {
                bool rs = await _mediator.Send(new SyncDepartmentsCommand());
                return Ok(rs);
            }

            return BadRequest();
        }

        [HttpPost("departments/import")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ImportAdminDepartments(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);

                    await _mediator.Send(new ImportDepartmentsCommand { Stream = stream });
                    return Ok();
                }
            }

            return BadRequest();
        }


        [HttpGet("users")]
        public async Task<PagedList<AdminUserModel>> AdminUsers([FromQuery] string q, [FromQuery] int activePage, [FromQuery] int itemsPerPage)
        {
            var rs = await _adminAssessmentQueries.GetAdminUsersAsync(q, activePage, itemsPerPage);

            return rs;
        }

        [HttpPost("users/sync")]
        public async Task<IActionResult> AdminSyncUsers()
        {
            if (_identityClaimsHelper.CurrentUser != null)
            {
                bool rs = await _mediator.Send(new SyncUsersCommand());
                return Ok(rs);
            }

            return BadRequest();
        }

        [HttpPost("users/import")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ImportAdminUsers(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);

                    await _mediator.Send(new ImportUsersCommand { Stream = stream });
                    return Ok();
                }
            }

            return BadRequest();
        }

        [HttpGet("permissions")]
        public async Task<PagedList<PermissionModel>> AdminList([FromQuery] int activePage, [FromQuery] int itemsPerPage)
        {
            var rs = await _adminAssessmentQueries.GetPermissions(activePage, itemsPerPage);

            return rs;
        }

        [HttpPost("permissions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SaveAdminPermission([FromBody] SaveAdminPermisionCommand model)
        {
            if (_identityClaimsHelper.CurrentUser != null)
            {
                bool rs = await _mediator.Send(model);
                return Ok(rs);
            }

            return BadRequest();
        }


        [HttpDelete("permissions")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteAdminPermission([FromBody] DeleteAdminPermisionCommand model)
        {
            if (_identityClaimsHelper.CurrentUser != null)
            {
                var rs = await _mediator.Send(model);
                return Ok(rs);
            }

            return BadRequest();
        }

        [HttpGet("permissions/check")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<bool> CheckAdminPermission()
        {
            if (_identityClaimsHelper.CurrentUser != null)
            {
                var rs = await _adminAssessmentQueries.CheckPermission(_identityClaimsHelper.CurrentUser.Email);

                return rs;
            }

            return false;
        }
    }
}

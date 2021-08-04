using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Commands;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Queries;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserAssessmentsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserOrgHelper _helper;
        private readonly IAssessmentPeriodQueries _assessmentQueries;
        private readonly IIdentityClaimsHelper _identityClaimsHelper;
        private readonly ILogger<UserAssessmentsController> _logger;

        public UserAssessmentsController(IMediator mediator,
                                         IUserOrgHelper helper,
                                         IAssessmentPeriodQueries assessmentQueries,
                                         IIdentityClaimsHelper claimsHelper,
                                         ILogger<UserAssessmentsController> logger)
        {
            _mediator = mediator;
            _helper = helper;
            _assessmentQueries = assessmentQueries;
            _identityClaimsHelper = claimsHelper;
            _logger = logger;
        }

        [HttpGet("periods")]
        public async Task<PagedList<UserPeriodModel>> UserAssessmentPeriods([FromQuery]int activePage, [FromQuery]int itemsPerPage)
        {
            if (_identityClaimsHelper.CurrentUser != null)
            {
                return await _assessmentQueries.GetUserAssessmentPeriodsAsync(_identityClaimsHelper.CurrentUser.Id, activePage, itemsPerPage);
            }

            return new PagedList<UserPeriodModel>();
        }


        [HttpGet("periods/{id}")]
        public async Task<PagedList<UserPeriodDepartmentModel>> UserAssessmentPeriods(int id)
        {
            if (_identityClaimsHelper.CurrentUser != null)
            {
                var rs = await _assessmentQueries.GetUserAssessmentPeriodDepartment(_identityClaimsHelper.CurrentUser.Id, id);

                return rs;
            }

            return new PagedList<UserPeriodDepartmentModel>();
        }

        [HttpGet("periods/{id}/departments/{departmentid}")]
        public async Task<PagedList<UserAssessmentQuestionModel>> UserAssessmentQuestions(int id, int departmentid)
        {
            if (_identityClaimsHelper.CurrentUser != null)
            {
                var rs = await _assessmentQueries.GetUserAssessmentQuestions(_identityClaimsHelper.CurrentUser.Id, id, departmentid);

                return rs;
            }

            return new PagedList<UserAssessmentQuestionModel>();
        }

        [HttpPost("results")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UserAssessmentQuestions([FromBody]SaveAssessmentResultCommand model)
        {
            bool commandResult = false;

            if (_identityClaimsHelper.CurrentUser != null)
            {
                model.Finished = false;
                commandResult = await _mediator.Send(model);
            }

            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("results/complete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CompleteUserAssessmentQuestions([FromBody] SaveAssessmentResultCommand model)
        {
            bool commandResult = false;

            if (_identityClaimsHelper.CurrentUser != null)
            {
                model.Finished = true;
                commandResult = await _mediator.Send(model);
            }

            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }

    }
}

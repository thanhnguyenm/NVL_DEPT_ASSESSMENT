using Microsoft.AspNetCore.Http;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.Domain.ExternalModel;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper
{
    public class IdentityClaimsHelper : IIdentityClaimsHelper
    {

        private static User currentUser;
        private IHttpContextAccessor _context;
        private readonly IUserOrgHelper _helper;
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;

        public IdentityClaimsHelper(IHttpContextAccessor context, IUserOrgHelper helper,
                                    IAssessmentCriteriaRepository assessmentCriteriaRepository) 
        {
            _helper = helper;

            _assessmentCriteriaRepository = assessmentCriteriaRepository;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public IdentityModel GetIdentityModel()
        {
            IdentityModel model = new IdentityModel();
            var claims = _context.HttpContext.User.Claims;

            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    if (claim.Type == "name") model.Name = claim.Value;
                    if (claim.Type == "preferred_username") model.Email = claim.Value;
                    if (claim.Type == "iss") model.Issuer = claim.Value;
                    if (claim.Type == "aud") model.Audiance = claim.Value;
                    if (claim.Type == "http://schemas.microsoft.com/identity/claims/tenantid") model.Tenantid = claim.Value;
                }
            }
            

            return model;
        }

        public User CurrentUser { 
            get {

                if(currentUser == null)
                {
                    IdentityModel identityModel = GetIdentityModel();

                    currentUser = _assessmentCriteriaRepository.GetActiveUsersFromDB(identityModel.Email).GetAwaiter().GetResult();
                }

                return currentUser;
            } 
        }
    }
}

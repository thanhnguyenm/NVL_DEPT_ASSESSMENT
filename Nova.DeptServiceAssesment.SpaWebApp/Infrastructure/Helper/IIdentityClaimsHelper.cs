using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.Domain.ExternalModel;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper
{
    public interface IIdentityClaimsHelper
    {
        User CurrentUser { get; }
        IdentityModel GetIdentityModel();
    }
}

using Microsoft.EntityFrameworkCore;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.AsessmentPeriodAggregate;
using Nova.DeptServiceAssesment.Domain.AggregatesModel.UserOrgAggregate;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.Infrastructure.Repositories
{
    public class PermissionsRepository : IPermissionsRepository
    {
        private readonly AssessmentContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public PermissionsRepository(AssessmentContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Permission>> GetPermissionsAsync()
        {
            var p = await _context
                                .Permissions
                                .ToListAsync();


            return p;
        }

        public async Task<Permission> GetPermissionAsync(string email)
        {
            var p = await _context
                                .Permissions
                                .FirstOrDefaultAsync(o => o.Email.ToLower() == email.ToLower());


            return p;
        }

        public async Task<List<Permission>> GetPermissionsAsync(List<string> emails)
        {
            var p = await _context
                                .Permissions
                                .Where(o => !string.IsNullOrEmpty(o.Email) && emails.Contains(o.Email.ToLower())).ToListAsync();


            return p;
        }

        public Permission AddPermission(Permission p)
        {
            p = _context.Permissions.Add(p).Entity;

            return p;
        }

        public void DeletePermission(Permission p)
        {
            _context.Permissions.Remove(p);
        }

        public async Task<List<Email>> GetNewEmailsAsync()
        {
            var p = await _context
                                .Emails
                                .Where(x => x.Status == "NEW").ToListAsync();


            return p;
        }

        public Email AddEmail(Email p)
        {
            p = _context.Emails.Add(p).Entity;

            return p;
        }

        public void UpdateEmail(Email result)
        {
            _context.Entry(result).State = EntityState.Modified;
        }
    }
}

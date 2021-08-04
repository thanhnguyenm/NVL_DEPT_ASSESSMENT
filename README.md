# NVL_DEPT_ASSESSMENT

This project is an application to assess the departments at NVL. System uses data of NVL such as employee, departments
and organization chart. System also integrate with Azure AD of NVL tanent to perform SSO.

## Artchitecture

System is deisgned based on Clean Artichitect and SQRS.

## Solution Structure

Solution contains 4 projects
* Nova.DeptServiceAssesment.Database : database schema
* Nova.DeptServiceAssesment.Domain : business domain
* Nova.DeptServiceAssesment.Infrastructure : infrastructure
* Nova.DeptServiceAssesment.SpaWebApp : API and FE

### Techniques

* Database : Entity Framework and SQL
* Front-End : ReactJS
* Backend : C#, Mediator, Dapper and ASP.NET Core API


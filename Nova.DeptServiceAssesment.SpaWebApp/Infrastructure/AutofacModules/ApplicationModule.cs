using Autofac;
using Nova.DeptServiceAssesment.Domain.Repositories;
using Nova.DeptServiceAssesment.Infrastructure.Repositories;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.AutofacModules
{
    public class ApplicationModule
        : Autofac.Module
    {

        public string QueriesConnectionString { get; }

        public ApplicationModule(string qconstr)
        {
            QueriesConnectionString = qconstr;

        }

        protected override void Load(ContainerBuilder builder)
        {

            builder.Register(c => new AssessmentPeriodQueries(QueriesConnectionString))
                .As<IAssessmentPeriodQueries>()
                .InstancePerLifetimeScope();

            builder.Register(c => new AdminSetupQueries(QueriesConnectionString))
                .As<IAdminSetupQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AssessmentPeriodRepository>()
                .As<IAssessmentPeriodRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AssessmentCriteriaRepository>()
                .As<IAssessmentCriteriaRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PermissionsRepository>()
                .As<IPermissionsRepository>()
                .InstancePerLifetimeScope();

            //builder.RegisterType<RequestManager>()
            //   .As<IRequestManager>()
            //   .InstancePerLifetimeScope();

            //builder.RegisterAssemblyTypes(typeof(CreateOrderCommandHandler).GetTypeInfo().Assembly)
            //    .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

        }
    }
}

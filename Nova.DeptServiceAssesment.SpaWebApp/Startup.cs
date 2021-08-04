using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nova.DeptServiceAssesment.Infrastructure;
using Nova.DeptServiceAssesment.SpaWebApp.Configuration;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.AutofacModules;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Extensions;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Helper;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Jobs;
using Nova.DeptServiceAssesment.SpaWebApp.Infrastructure.Queries;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Nova.DeptServiceAssesment.SpaWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            services.AddMvc(options => { options.Filters.Add(typeof(HttpGlobalExceptionFilter)); })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonOptions(options => {
                         options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                         options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                     });
            

            services
                .AddCustomConfiguration(Configuration)
                .AddCustomDbContext(Configuration)
                .AddCustomService(Configuration)
                //.AddCustomSwagger(Configuration)
                .AddCustomAuthentication(Configuration);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterModule(new MediatorModule());
            builder.RegisterModule(new ApplicationModule(Configuration["AppSettings:ConnectionStrings:DefaultConnectionString"]));
            AutofacContainer = builder.Build();

            return new AutofacServiceProvider(AutofacContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();

            }

            
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            CustomExtensionsMethods.SeedData(app.ApplicationServices);
        }

    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkSqlServer()
                   .AddDbContext<AssessmentContext>(options =>
                   {
                       options.UseSqlServer(configuration["AppSettings:ConnectionStrings:DefaultConnectionString"]);
                   },
                       ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                   );

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Assessment API v1",
                    Version = "v1",
                    Description = "The Assessment Service HTTP API v1"
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
            IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                var authSettings = configuration.GetSection("AppSettings").Get<AppSettings>();

                options.Authority = authSettings.AzureAd.Authority;
                options.RequireHttpsMetadata = false;
                options.Audience = authSettings.AzureAd.ClientId;
            });

            return services;
        }

        public static IServiceCollection AddCustomService(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddTransient<IUserOrgHelper, UserOrgHelper>()
                .AddTransient<IIdentityClaimsHelper, IdentityClaimsHelper>()
                .AddHttpClient<IUserOrgHelper, UserOrgHelper>()
                .Services
                .AddTransient<IEmailHelper, EmailHelper>();

            // Add Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();


            // Add our job
            services.AddSingleton<SendEmailJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(SendEmailJob),
                cronExpression: configuration.GetValue<string>("AppSettings:CronJobs:SendEmailJob") // run every 1 hour
                ));

            services.AddSingleton<ReminderJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(ReminderJob),
                cronExpression: configuration.GetValue<string>("AppSettings:CronJobs:ReminderJob")// run every day at 9g00 am
                ));

            services.AddSingleton<ExpiredPeriodJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(ExpiredPeriodJob),
                cronExpression: configuration.GetValue<string>("AppSettings:CronJobs:ExpiredPeriodJob")// run every day at 9g30 am
                ));

            services.AddHostedService<QuartzHostedService>();

            return services;
        }

        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var config = new AppSettings();
            configuration.Bind("AppSettings", config);
            services.AddSingleton(config);

            return services;
        }

        public static void SeedData(IServiceProvider services)
        {
            //cache data
            var userService = services.GetService<IUserOrgHelper>();
            userService.GetDepartment();
            userService.GetUsers();

            //seed database
            var setting = services.GetService<AppSettings>();
            if(setting.SeedData.ToLower() == "true")
            {
                var context = services.GetService<AssessmentContext>();
                var env = services.GetService<IHostingEnvironment>();
                var logger = services.GetService<ILogger<AssessmentContextSeed>>();

                new AssessmentContextSeed()
                    .SeedAsync(context, env, logger, userService, setting)
                    .Wait();
            }   
        }
    }
}

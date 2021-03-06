﻿using Foundation.Api.Contracts;
using Foundation.Api.Contracts.Metadata;
using Foundation.Api.Implementations;
using Foundation.Api.Implementations.Metadata;
using Foundation.Api.Implementations.Project;
using Foundation.Api.Middlewares;
using Foundation.Api.Middlewares.SignalR;
using Foundation.Api.Middlewares.SignalR.Implementations;
using Foundation.Api.Middlewares.WebApi;
using Foundation.Api.Middlewares.WebApi.OData;
using Foundation.Api.Middlewares.WebApi.OData.ActionFilters;
using Foundation.Core.Contracts;
using Foundation.Core.Contracts.Project;
using Foundation.Core.Implementations;
using Foundation.DataAccess.Contracts;
using Foundation.Test.Api.Middlewares;
using Foundation.Test.Api.Middlewares.JobScheduler.Implementations;
using Foundation.Test.DataAccess.Implementations;
using Foundation.Test.Model.Implemenations;
using System;
using System.Reflection;
using System.Web.Http;

namespace Foundation.Test.Api.Implementations.Project
{
    public class TestFoundationDependenciesManager : IDependenciesManager
    {
        private readonly bool _useSso;

        public TestFoundationDependenciesManager(bool useSso, Action<IDependencyManager> additionalDependencies = null)
        {
            _useSso = useSso;
            _additionalDependencies = additionalDependencies;
        }

        private readonly Action<IDependencyManager> _additionalDependencies;

        public virtual void ConfigureDependencies(IDependencyManager dependencyManager)
        {
            dependencyManager.RegisterMinimalDependencies();

            dependencyManager.RegisterInstance(DefaultAppEnvironmentProvider.Current);
            dependencyManager.RegisterInstance(DefaultJsonContentFormatter.Current);
            dependencyManager.RegisterInstance(DefaultPathProvider.Current);

            dependencyManager.Register<ITimeZoneManager, DefaultTimeZoneManager>();
            dependencyManager.Register<IScopeStatusManager, DefaultScopeStatusManager>();
            dependencyManager.Register<IRequestInformationProvider, DefaultRequestInformationProvider>();
            dependencyManager.Register<ILogger, DefaultLogger>();
            dependencyManager.Register<IUserInformationProvider, DefaultUserInformationProvider>();
            dependencyManager.Register<ILogStore, TestLogStore>();

            dependencyManager.Register<IDateTimeProvider, DefaultDateTimeProvider>(lifeCycle: DepepdencyLifeCycle.SingleInstance);
            dependencyManager.Register<IRandomStringProvider, DefaultRandomStringProvider>(lifeCycle: DepepdencyLifeCycle.SingleInstance);
            dependencyManager.Register<ICertificateProvider, DefaultCertificateProvider>(lifeCycle: DepepdencyLifeCycle.SingleInstance);
            dependencyManager.Register<IExceptionToHttpErrorMapper, DefaultExceptionToHttpErrorMapper>(lifeCycle: DepepdencyLifeCycle.SingleInstance);
            dependencyManager.Register<INameService, DefaultNameService>(lifeCycle: DepepdencyLifeCycle.SingleInstance);
            dependencyManager.Register<ITestUserService, TestUserService>(lifeCycle: DepepdencyLifeCycle.SingleInstance);

            dependencyManager.RegisterAppEvents<RazorViewEngineConfiguration>();
            dependencyManager.RegisterAppEvents<InitialTestDataConfiguration>();
            dependencyManager.RegisterAppEvents<FoundationInitialNameServiceConfiguration>();
            dependencyManager.RegisterAppEvents<TestInitialNameServiceConfiguration>();

            dependencyManager.RegisterOwinMiddleware<OwinCompressionMiddlewareConfiguration>();
            dependencyManager.RegisterOwinMiddleware<StaticFilesMiddlewareConfiguration>();
            dependencyManager.RegisterOwinMiddleware<AutofacDependencyInjectionMiddlewareConfiguration>();
            dependencyManager.RegisterOwinMiddleware<OwinExceptionHandlerMiddlewareConfiguration>();
            dependencyManager.RegisterOwinMiddleware<LogRequestInformationMiddlewareConfiguration>();
            dependencyManager.RegisterOwinMiddleware<SignInPageMiddlewareConfiguration>();
            dependencyManager.RegisterOwinMiddleware<ReadAuthTokenFromCookieMiddlewareConfiguration>();
            if (_useSso)
                dependencyManager.RegisterOwinMiddleware<SingleSignOnMiddlewareConfiguration>();
            else
                dependencyManager.RegisterOwinMiddleware<EmbeddedOAuthMiddlewareConfiguration>();
            dependencyManager.RegisterOwinMiddleware<RedirectToSsoIfNotLoggedInMiddlewareConfiguration>();
            dependencyManager.RegisterOwinMiddleware<LogUserInformationMiddlewareConfiguration>();
            dependencyManager.RegisterOwinMiddleware<MetadataMiddlewareConfiguration>();

            dependencyManager.RegisterDefaultWebApiConfiguration(typeof(FoundationEdmModelProvider).GetTypeInfo().Assembly, typeof(TestEdmModelProvider).GetTypeInfo().Assembly);

            dependencyManager.RegisterUsing<IOwinMiddlewareConfiguration>(resolver =>
            {
                return dependencyManager.CreateChildDependencyManager(childDependencyManager =>
                {
                    childDependencyManager.RegisterGlobalWebApiActionFiltersUsing(httpConfiguration =>
                    {
                        httpConfiguration.Filters.Add(new AuthorizeAttribute());
                    });

                    childDependencyManager.RegisterWebApiMiddlewareUsingDefaultConfiguration();

                }).Resolve<WebApiMiddlewareConfiguration>();

            }, lifeCycle: DepepdencyLifeCycle.SingleInstance, overwriteExciting: false);

            dependencyManager.RegisterUsing<IOwinMiddlewareConfiguration>(resolver =>
            {
                return dependencyManager.CreateChildDependencyManager(childDependencyManager =>
                {
                    childDependencyManager.RegisterGlobalWebApiActionFiltersUsing(httpConfiguration =>
                    {
                        httpConfiguration.Filters.Add(new DefaultODataAuthorizeAttribute());
                    });

                    childDependencyManager.RegisterWebApiODataMiddlewareUsingDefaultConfiguration();
                    childDependencyManager.RegisterEdmModelProvider<FoundationEdmModelProvider>();
                    childDependencyManager.RegisterEdmModelProvider<TestEdmModelProvider>();

                }).Resolve<WebApiODataMiddlewareConfiguration>();

            }, lifeCycle: DepepdencyLifeCycle.SingleInstance, overwriteExciting: false);

            dependencyManager.RegisterDefaultPageMiddlewareUsingDefaultConfiguration();

            dependencyManager.RegisterSignalRConfiguration<SignalRAuthroizeConfiguration>();
            dependencyManager.RegisterSignalRMiddlewareUsingDefaultConfiguration(typeof(MessagesHub).GetTypeInfo().Assembly);

            dependencyManager.RegisterBackgroundJobWorkerUsingDefaultConfiguration<JobSchedulerInMemoryBackendConfiguration>();

            dependencyManager.Register<IAppMetadataProvider, DefaultAppMetadataProvider>(lifeCycle: DepepdencyLifeCycle.SingleInstance);
            dependencyManager.RegisterMetadata(typeof(FoundationEdmModelProvider).GetTypeInfo().Assembly, typeof(TestEdmModelProvider).GetTypeInfo().Assembly);

            dependencyManager.RegisterGeneric(typeof(IRepository<>).GetTypeInfo(), typeof(TestEfRepository<>).GetTypeInfo(), DepepdencyLifeCycle.InstancePerLifetimeScope);

            dependencyManager.RegisterGeneric(typeof(IEntityWithDefaultGuidKeyRepository<>).GetTypeInfo(), typeof(TestEfEntityWithDefaultGuidKeyRepository<>).GetTypeInfo(), DepepdencyLifeCycle.InstancePerLifetimeScope);

            dependencyManager.RegisterDbContext<TestDbContext, InMemoryDbContextObjectsProvider>();

            dependencyManager.RegisterEfCoreAutoMapper();

            dependencyManager.RegisterDtoModelMapperConfiguration<TestDtoModelMapperConfiguration>();

            if (_additionalDependencies != null)
                _additionalDependencies(dependencyManager);
        }
    }
}

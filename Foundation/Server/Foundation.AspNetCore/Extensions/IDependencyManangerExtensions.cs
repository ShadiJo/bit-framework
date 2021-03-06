﻿using Foundation.AspNetCore.Contracts;
using Foundation.AspNetCore.Middlewares;
using Foundation.Core.Contracts;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundation.Core.Contracts
{
    public static class IDependencyManangerExtensions
    {
        public static IDependencyManager RegisterAspNetCoreMiddleware<TMiddleware>(this IDependencyManager dependencyManager)
            where TMiddleware : class, IAspNetCoreMiddlewareConfiguration
        {
            if (dependencyManager == null)
                throw new ArgumentNullException(nameof(dependencyManager));

            dependencyManager.Register<IAspNetCoreMiddlewareConfiguration, TMiddleware>(lifeCycle: DepepdencyLifeCycle.SingleInstance, overwriteExciting: false);

            return dependencyManager;
        }

        public static IDependencyManager RegisterAspNetCoreMiddlewareUsing(this IDependencyManager dependencyManager, Action<IApplicationBuilder> aspNetCoreAppCustomizer, RegisterKind registerKind)
        {
            if (dependencyManager == null)
                throw new ArgumentNullException(nameof(dependencyManager));

            if (aspNetCoreAppCustomizer == null)
                throw new ArgumentNullException(nameof(aspNetCoreAppCustomizer));

            dependencyManager.RegisterInstance<IAspNetCoreMiddlewareConfiguration>(new DelegateAspNetCoreMiddlewareConfiguration(aspNetCoreAppCustomizer, registerKind), overwriteExciting: false);

            return dependencyManager;
        }
    }
}

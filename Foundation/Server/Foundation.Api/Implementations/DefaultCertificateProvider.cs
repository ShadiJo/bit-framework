﻿using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Foundation.Core.Contracts;
using Foundation.Core.Models;

namespace Foundation.Api.Implementations
{
    public class DefaultCertificateProvider : ICertificateProvider
    {
        private readonly IAppEnvironmentProvider _appEnvironmentProvider;
        private readonly IPathProvider _pathProvider;
        private X509Certificate2 _certificate;

        protected DefaultCertificateProvider()
        {
        }

        public DefaultCertificateProvider(IAppEnvironmentProvider appEnvironmentProvider, IPathProvider pathProvider)
        {
            if (appEnvironmentProvider == null)
                throw new ArgumentNullException(nameof(appEnvironmentProvider));

            if (pathProvider == null)
                throw new ArgumentNullException(nameof(pathProvider));

            _appEnvironmentProvider = appEnvironmentProvider;
            _pathProvider = pathProvider;
        }

        public virtual X509Certificate2 GetSignelSignOnCertificate()
        {
            if (_certificate == null)
            {
                AppEnvironment activeAppEnvironment = _appEnvironmentProvider.GetActiveAppEnvironment();

                string password = activeAppEnvironment
                    .GetConfig<string>("IdentityCertificatePassword");

                _certificate = new X509Certificate2(File.ReadAllBytes(_pathProvider.MapPath(activeAppEnvironment.GetConfig<string>("IdentityServerCertificatePath"))),
                    password);
            }

            return _certificate;
        }

        public virtual void Dispose()
        {
            if (_certificate != null)
                _certificate.Dispose();
        }
    }
}
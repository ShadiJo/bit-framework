﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Foundation.Test.Api.Middlewares.WebApi.Tests
{
    [TestClass]
    public class ODataMetadataTests
    {
        [TestMethod]
        [TestCategory("WebApi"), TestCategory("OData")]
        public async Task AllEdmProvidersMustProvideTheirOwnMetadata()
        {
            using (TestEnvironment testEnvironment = new TestEnvironment())
            {
                OAuthToken token = testEnvironment.Server.Login("ValidUserName", "ValidPassword");

                string[] edmModelProviders = new string[] { "Foundation", "Test" };

                foreach (string edmModelProvider in edmModelProviders)
                {
                    HttpResponseMessage getMetadataResponse = await testEnvironment.Server.GetHttpClient(token)
                            .GetAsync($"/odata/{edmModelProvider}/$metadata");

                    Assert.AreEqual(HttpStatusCode.OK, getMetadataResponse.StatusCode);

                    Assert.AreEqual("application/xml", getMetadataResponse.Content.Headers.ContentType.MediaType);
                }
            }
        }
    }
}

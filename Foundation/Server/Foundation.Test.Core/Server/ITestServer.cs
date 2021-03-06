﻿using Microsoft.AspNet.SignalR.Client;
using Microsoft.Owin.Testing;
using OpenQA.Selenium.Remote;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Test
{
    public class OAuthToken
    {
        public virtual string token_type { get; set; }

        public virtual string access_token { get; set; }
    }

    public class RemoteWebDriverOptions
    {
        public string Uri { get; set; } = null;

        public OAuthToken Token { get; set; } = null;

        public bool ClientSideTest { get; set; } = true;
    }

    public interface ITestServer : IDisposable
    {
        RemoteWebDriver GetWebDriver(RemoteWebDriverOptions options = null);

        OAuthToken Login(string userName, string password);

        ODataClient BuildODataClient(Action<HttpRequestMessage> beforeRequest = null,
            Action<HttpResponseMessage> afterResponse = null, OAuthToken token = null, string route = null);

        ODataBatch BuildODataBatchClient(Action<HttpRequestMessage> beforeRequest = null,
           Action<HttpResponseMessage> afterResponse = null, OAuthToken token = null, string route = null);

        HttpClient GetHttpClient(OAuthToken token = null);

        IHubProxy BuildSignalRClient(OAuthToken token = null, Action<string, dynamic> onMessageRecieved = null);

        void Initialize(Uri uri);

        Uri Uri { get; }
    }
}

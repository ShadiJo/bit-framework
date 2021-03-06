﻿using System;
using Foundation.Core.Contracts;
using Foundation.Api.Implementations;
using System.Web.Http;
using Foundation.Model.Contracts;
using Foundation.Api.ApiControllers;
using System.Reflection;

namespace Simple.OData.Client
{
    public static class ODataClientExtensions
    {
        public static IBoundClient<TDto> Controller<TController, TDto>(this IODataClient client)
            where TDto : class, IDto
            where TController : DtoController<TDto>
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            return client.For<TDto>(typeof(TController).GetTypeInfo().Name.Replace("Controller", string.Empty));
        }
    }
}

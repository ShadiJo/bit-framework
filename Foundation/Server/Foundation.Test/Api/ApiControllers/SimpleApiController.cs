﻿using System.Web.Http;

namespace Foundation.Test.Api.ApiControllers
{
    public class SimpleApiController : ApiController
    {
        [Route("customers/{customerId}/orders")]
        [HttpGet]
        public virtual IHttpActionResult FindOrdersByCustomer(int customerId)
        {
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using WebAPI.Lib;
using Dapper;

namespace WebAPI.Filter
{
    public class Login : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            /* <summary>
                * custom your authorization program here
                * </summary> */
        }
    }
}

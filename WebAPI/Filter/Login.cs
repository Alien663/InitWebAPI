using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Filter;

public class Login : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        /* <summary>
        *   custom your authorization program here
        * </summary> 
        */
    }
}

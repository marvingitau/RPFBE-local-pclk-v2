using Microsoft.AspNetCore.Authorization;
using System;

namespace RPFBE.Auth
{
    public class RoleAttribute : AuthorizeAttribute
    {
        public RoleAttribute(params string[] roles)
        {
            Roles = String.Join(",", roles);
        }
    }
}

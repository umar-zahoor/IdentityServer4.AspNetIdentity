using System;
using Microsoft.AspNetCore.Identity;

namespace Host.Models
{
    // Add profile data for api users by adding properties to the ApiUser class
    public class ApiUser : IdentityUser<Guid>
    {
    }

    public class ApiRole : IdentityRole<Guid>
    {

    }
}

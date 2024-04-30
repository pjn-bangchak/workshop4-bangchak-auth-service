using Microsoft.AspNetCore.Identity;

namespace BangchakAuthService.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    public string Fullname {get;set;} = null!;
}


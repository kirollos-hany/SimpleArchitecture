using Microsoft.AspNetCore.Identity;

namespace SimpleArchitecture.Authentication.Types;

public class UserRole : IdentityUserRole<int>
{
  public Role? Role { get; private set; }
  
  public User? User { get; private set; }
}

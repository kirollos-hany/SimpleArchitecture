using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleArchitecture.Auditing.Types;
using SimpleArchitecture.Authentication.Types;
using SimpleArchitecture.Data.Interfaces;

#nullable disable

namespace SimpleArchitecture.Data;

public class SimpleArchitectureDbContext : IdentityDbContext<User,
    Role,
    int,
    UserClaim,
    UserRole,
    UserLogin,
    RoleClaim,
    UserToken>, IDbContext
{
    public SimpleArchitectureDbContext(DbContextOptions<SimpleArchitectureDbContext> options) : base(options)
    {
        
    }
    public DbSet<ActionAudit> ActionAudits { get; set; }

    public DbSet<LoginAudit> LoginAudits { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
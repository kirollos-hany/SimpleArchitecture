using Microsoft.EntityFrameworkCore;
using SimpleArchitecture.Auditing.Types;
using SimpleArchitecture.Authentication.Types;

namespace SimpleArchitecture.Data.Interfaces;

public interface IDbContext
{
    DbSet<User> Users { get; }
    
    DbSet<ActionAudit> ActionAudits { get; }

    DbSet<LoginAudit> LoginAudits { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
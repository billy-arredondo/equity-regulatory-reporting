using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Persistence.Identity;
using equity_regulatory_reporting.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    AuditableEntityInterceptor auditInterceptor)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
    public DbSet<DocumentTypePersonType> DocumentTypePersonTypes => Set<DocumentTypePersonType>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Participation> Participations => Set<Participation>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<BoardMember> BoardMembers => Set<BoardMember>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(auditInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

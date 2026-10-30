using AuthService.Domain.Entities.Org;
using AuthService.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infra;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public const string Schema = "auth";

    public DbSet<AppUser> AppUsers { get; set; } = default!;
    public DbSet<UserDetails> UserDetails { get; set; } = default!;
    public DbSet<UserAddress> UserAddresses { get; set; } = default!;
    public DbSet<AppRole> AppRoles { get; set; } = default!;
    public DbSet<AppUserRole> AppUserRoles { get; set; } = default!;
    public DbSet<Organization> Organizations { get; set; } = default!;
    public DbSet<OrgAddress> OrgAddresses { get; set; } = default!;
    public DbSet<OrganizationDept> OrganizationDepts { get; set; } = default!;
    public DbSet<OrganizationRole> OrganizationRoles { get; set; } = default!;
    public DbSet<OrgUserRole> OrgUserRoles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
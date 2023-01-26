using Microsoft.EntityFrameworkCore;

namespace WorkforceDataApi.Models;

public class WorkforceDbContext : DbContext
{
    private string _connectionString;

    public WorkforceDbContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new Exception("Connection string DefaultConnection is missing.");
    }

    public DbSet<TpsExtractDataItem> TpsExtractDataItems => Set<TpsExtractDataItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(
                _connectionString,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "workforce_data"))
            .UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasCollation(name: "case_insensitive", locale: "und-u-ks-level2", provider: "icu", deterministic: false);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkforceDbContext).Assembly);
    }
}

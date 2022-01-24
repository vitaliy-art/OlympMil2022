using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository.EntityFrameworkCore;
using Task.Context.Models;

namespace Task.Context;

public class Context : DbContext
{
    private static bool _migrated;

    public DbSet<Division>? Divisions { get; set; }
    public DbSet<Cadet>? Cadets { get; set; }
    public DbSet<Officer>? Officers { get; set; }

    public Context(DbContextOptions<Context> options)
        : base(options)
    {
        if (!_migrated)
        {
            _migrated = true;
            Database.Migrate();
        }
    }
}

public class SqliteRepositoryFactory :
    RepositoryFactory<Context>,
    IDesignTimeDbContextFactory<Context>
{
    private readonly string? _connectionString = null;

    public SqliteRepositoryFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqliteRepositoryFactory()
    {}

    public override Context CreateDbContext(string[]? args = null)
    {
        DbContextOptionsBuilder<Context> builder = new();
        if (_connectionString is null)
            builder = builder.UseSqlite("Filename=bd.db");
        else
            builder.UseSqlite(_connectionString);
        builder = builder.EnableSensitiveDataLogging();
        return new Context(builder.Options);
    }
}

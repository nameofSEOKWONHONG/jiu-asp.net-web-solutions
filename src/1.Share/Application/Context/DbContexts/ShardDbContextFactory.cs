using Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Context;

public class ShardDbContext : DbContextBase
{
    public ShardDbContext(DbContextOptions options) : base(options)
    {
    }
}

public class ShardDbContextFactory : IDbContextFactory<ShardDbContext>
{
    private readonly string _connectionString;
    public ShardDbContextFactory(IOptions<ShardDbOption> options)
    {
        _connectionString = options.Value.ConnectionString;
    }
    
    public ShardDbContext CreateDbContext()
    {
        var builder = new DbContextOptionsBuilder<ShardDbContext>();
        builder.UseNpgsql(_connectionString);
        return new ShardDbContext(builder.Options);
    }
}
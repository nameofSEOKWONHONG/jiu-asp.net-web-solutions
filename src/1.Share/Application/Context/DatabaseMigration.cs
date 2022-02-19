using System;
using System.Linq;
using System.Threading.Tasks;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Application.Context;

public class DatabaseMigration
{
    private readonly ILogger _logger;
    private readonly ApplicationDbContext _dbContext;
    public DatabaseMigration(ILogger<DatabaseMigration> logger,
        ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Migration()
    {
        var exists = await _dbContext.Migrations.Where(m => m.MIGRATION_YN == true && m.COMPLETE_YN == false)
            .FirstOrDefaultAsync();

        if (exists.xIsNotEmpty())
        {
            await _dbContext.Database.MigrateAsync();
            exists.MIGRATION_YN = true;
            exists.COMPLETE_YN = true;
            exists.UPDATE_DT = DateTime.UtcNow;
            exists.UPDATE_ID = "SYSTEM";
            _dbContext.Migrations.Update(exists);
            await _dbContext.SaveChangesAsync();
        }
    }
}
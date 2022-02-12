using Application.Context;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace QutarzScheduleService;

public class EmailPrintJob : IJob
{
    private readonly ILogger _logger;
    private readonly JIUDbContext _dbContext;
    public EmailPrintJob(ILogger<EmailPrintJob> logger, JIUDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        var user = await _dbContext.Users.FirstAsync();
        _logger.LogInformation(user.xToJson());
    }
}
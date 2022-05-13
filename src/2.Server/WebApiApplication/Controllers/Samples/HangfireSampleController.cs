using System;
using System.Linq;
using System.Threading;
using Application.Context;
using Hangfire;
using Infrastructure.Abstract.Controllers;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace WebApiApplication.Controllers;

public class HangfireSampleController : ApiControllerBase<HangfireSampleController>
{
    private readonly ApplicationDbContext _dbContext;
    public HangfireSampleController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("AsyncBackgroundJob")]
    public IActionResult AsyncBackgroundJob()
    {
        var worker = new Worker(this._dbContext);
        //비순차실행
        BackgroundJob.Enqueue(() => worker.Work(1));
        BackgroundJob.Enqueue(() => worker.Work(2));
        BackgroundJob.Enqueue(() => worker.Work(3));
        BackgroundJob.Enqueue(() => worker.Work(4));
        BackgroundJob.Enqueue(() => worker.Work(5));
        BackgroundJob.Enqueue(() => worker.Work(6));
        BackgroundJob.Enqueue(() => worker.Work(7));
        BackgroundJob.Enqueue(() => worker.Work(8));

        return Ok();
    }
    
    [HttpPost("SyncBackgroundJob")]
    public IActionResult SyncBackgroundJob()
    {
        var worker = new Worker(this._dbContext);
        //순차실행 보장
        var id = BackgroundJob.Enqueue(() => worker.Work(1));
        id = BackgroundJob.ContinueJobWith(id, () => worker.Work(2));
        id = BackgroundJob.ContinueJobWith(id, () => worker.Work(3));
        id = BackgroundJob.ContinueJobWith(id, () => worker.Work(4));
        id = BackgroundJob.ContinueJobWith(id, () => worker.Work(5));
        id = BackgroundJob.ContinueJobWith(id, () => worker.Work(6));
        id = BackgroundJob.ContinueJobWith(id, () => worker.Work(7));
        id = BackgroundJob.ContinueJobWith(id, () => worker.Work(8));
        return Ok(id);
    }

    [HttpPost("DelayBackgroundJob")]
    public IActionResult DelayBackgroundJob()
    {
        var worker = new Worker(this._dbContext);
        var jobId = BackgroundJob.Schedule(() => worker.Work(99),TimeSpan.FromMinutes(1));
        return Ok();
    }
}

public class Worker
{
    private readonly ApplicationDbContext _dbContext;
    public Worker(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public void Work(int num)
    {
        Console.WriteLine($"{num} : I'm working at hangfire.");
        var user = _dbContext.Users.First();
        Console.WriteLine($"user email is {user.EMAIL}");
        Thread.Sleep(1000);
    }
}
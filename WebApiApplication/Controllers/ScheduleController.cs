using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Context;
using Hangfire;
using Infrastructure.Abstract;
using Microsoft.AspNetCore.Mvc;
using QutarzScheduleService;

namespace WebApiApplication.Controllers;

public class ScheduleController : ApiControllerBase<ScheduleController>
{
    private readonly JIUDbContext _dbContext;
    private readonly EmailPrintJob _emailPrintJob;
    public ScheduleController(JIUDbContext dbContext, EmailPrintJob emailPrintJob)
    {
        _dbContext = dbContext;
        _emailPrintJob = emailPrintJob;
    }

    [HttpPost("ScheduleRegistration")]
    public async Task<IActionResult> ScheduleRegistration()
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
    
    [HttpPost("ScheduleRegistrationContiune")]
    public async Task<IActionResult> ScheduleRegistrationContiune()
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

    [HttpPost("SchduleDelayedRegistration")]
    public async Task<IActionResult> SchduleDelayedRegistration()
    {
        var worker = new Worker(this._dbContext);
        var jobId = BackgroundJob.Schedule(() => worker.Work(99),TimeSpan.FromMinutes(1));
        return Ok();
    }
}

public class Worker
{
    private readonly JIUDbContext _dbContext;
    public Worker(JIUDbContext dbContext)
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
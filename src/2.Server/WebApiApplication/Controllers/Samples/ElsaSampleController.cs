using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using Elsa.Activities.Workflows;
using Elsa.Activities.Workflows.Workflow;
using Elsa.ActivityResults;
using Elsa.Builders;
using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;
using Esprima.Ast;
using Infrastructure.Abstract.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace WebApiApplication.Controllers;

public class ElsaSampleController : ApiControllerBase<ElsaSampleController>
{
    private IBuildsAndStartsWorkflow _workflowRunner;
    public ElsaSampleController(IBuildsAndStartsWorkflow workflowRunner)
    {
        _workflowRunner = workflowRunner;
    }

    [HttpGet]
    public async Task<IActionResult> RunWorkflow()
    {
        var result = await _workflowRunner.BuildAndStartWorkflowAsync<HelloWorldWorlflow>();
        return ResultOk(result.WorkflowInstance.GetMetadata("result"));
    }
}

public class HelloWorldWorlflow : IWorkflow
{
    public void Build(IWorkflowBuilder builder)
    {
        builder.StartWith<HelloActivity>()
            .Then<WorldActivity>();
    }
}

public class HelloActivity : Activity
{
    public override async ValueTask<IActivityExecutionResult> ExecuteAsync(ActivityExecutionContext context)
    {
        return Done("Hello");
    }
}

public class WorldActivity : Activity
{
    public override async ValueTask<IActivityExecutionResult> ExecuteAsync(ActivityExecutionContext context)
    {
        context.WorkflowInstance.SetMetadata("result", $"{context.Input} World");
        return Done();
    }
}
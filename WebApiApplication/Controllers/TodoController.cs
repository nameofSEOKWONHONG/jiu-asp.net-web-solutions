using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Infrastructure.Message;
using Domain.Entities;
using Domain.Enums;
using eXtensionSharp;
using Hangfire;
using Infrastructure.Abstract;
using Infrastructure.Notifies;
using Microsoft.AspNetCore.Mvc;
using TodoApplication.Features.Command;
using TodoApplication.Features.Query;

namespace WebApiApplication.Controllers
{
    public class TodoController : AuthorizedController<TodoController>
    {
        [HttpGet("GetAllTodo")]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = true)]
        public async Task<IActionResult> GetAllTodo()
        {
            var session = await this._sessionContextService.GetSessionAsync();
            var result = await _mediator.Send(new GetAllTodoQuery(session.User.Id));

            #region [publish sample]
            await _mediator.Publish(new MessageNotify() { MessageTypes = new[]{ ENUM_MESSAGE_TYPE.SMS, ENUM_MESSAGE_TYPE.EMAIL }});
            #endregion
            
            return Ok(result);
        }

        [HttpGet("GetTodo/{id}")]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new [] {"id"})]
        public async Task<IActionResult> GetTodo(int id)
        {
            var result = await _mediator.Send(new GetTodoQuery(id));
            return Ok(result);
        }

        [HttpGet("GetTodoByDate/{from}/{to}")]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new [] {"from", "to"})]
        public async Task<IActionResult> GetTodoByDate(DateTime from, DateTime to)
        {
            var session = await _sessionContextService.GetSessionAsync();
            var result = await _mediator.Send(new GetTodoByDateQuery(session.User.Id, from, to));
            return Ok(result);
        }

        [HttpGet("GetTodoBySelectedDate/{selectedDate}")]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new [] {"selectedDate"})]        
        public async Task<IActionResult> GetTodoBySelectedDate(DateTime selectedDate)
        {
            var session = await _sessionContextService.GetSessionAsync();
            var result = await _mediator.Send(new GetTodoBySelectedDateQuery(session.User.Id, selectedDate));
            return Ok(result);
        }

        [HttpPost("SaveTodo")]
        public async Task<IActionResult> SaveTodo(Todo todo)
        {
            if (!this.TryValidate(todo, out ActionResult validateResult))
            {
                return validateResult;
            }
            
            var session = await _sessionContextService.GetSessionAsync();
            todo.WriteId = session.User.Id;
            todo.WriteDt = DateTime.UtcNow;
            var result = await _mediator.Send(new SaveTodoCommand(todo));
            if (result.Data.Id >= 0)
            {
                #region [hangfire sample]
                BackgroundJob.Enqueue(() => 
                    _mediator.Publish(
                        new MessageNotify()
                        {
                            MessageTypes = new[] {ENUM_MESSAGE_TYPE.EMAIL},
                            MessageRequest = new EmailMessageRequest(new[]
                            {
                                "test@gmail.com"
                            }, "test", "hello", null)
                        }, CancellationToken.None
                    )
                );
                #endregion
            }

            return Ok(result);
        }

        [HttpPost("RemoveTodo")]
        public async Task<IActionResult> RemoveTodo(int id)
        {
            var result = await _mediator.Send(new RemoveTodoCommand(id));
            return Ok(result);
        }
    }
}
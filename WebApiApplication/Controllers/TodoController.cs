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
        [HttpGet("GetAll")]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllTodoQuery(this.SessionContext.UserId));

            #region [publish sample]
            //await _mediator.Publish(new MessageNotify() { MessageTypes = new[]{ ENUM_MESSAGE_TYPE.SMS, ENUM_MESSAGE_TYPE.EMAIL }});
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

        [HttpGet("GetTodoFromTo/{from}/{to}")]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new [] {"from", "to"})]
        public async Task<IActionResult> GetTodo(DateTime from, DateTime to)
        {
            var result = await _mediator.Send(new GetTodoByDateQuery(this.SessionContext.UserId, from, to));
            return Ok(result);
        }

        [HttpGet("GetTodoFrom/{selectedDate}")]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new [] {"selectedDate"})]        
        public async Task<IActionResult> GetTodo(DateTime selectedDate)
        {
            var result = await _mediator.Send(new GetTodoBySelectedDateQuery(this.SessionContext.UserId, selectedDate));
            return Ok(result);
        }

        [HttpPost("SaveTodo")]
        public async Task<IActionResult> SaveTodo(Todo todo)
        {
            if (!this.TryValidate(todo, out ActionResult validateResult))
            {
                return validateResult;
            }

            todo.WriteId = this.SessionContext.UserId.ToString();
            todo.WriteDt = DateTime.UtcNow;
            var result = await _mediator.Send(new SaveTodoCommand(todo));
            if (result.Data.Id >= 0)
            {
                #region [hangfire sample]
                BackgroundJob.Enqueue(() => 
                    _mediator.Publish(
                        new NotifyMessage()
                        {
                            // MessageTypes = new[] {ENUM_NOTIFY_MESSAGE_TYPE.EMAIL},
                            // NotifyMessageRequest = new EmailNotifyMessageRequest(new[]
                            // {
                            //     "test@gmail.com"
                            // }, "test", "hello", null)
                            MessageTypes = new[] {ENUM_NOTIFY_MESSAGE_TYPE.EMAIL, ENUM_NOTIFY_MESSAGE_TYPE.LINE},
                            NotifyMessageRequest = new EmailNotifyMessageRequest(new[]
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
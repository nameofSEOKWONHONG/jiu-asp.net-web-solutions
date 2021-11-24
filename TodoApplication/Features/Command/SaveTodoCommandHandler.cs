using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.Todo;
using Application.Response;
using Domain.Entities;
using MediatR;

namespace TodoApplication.Features.Command
{
    public record SaveTodoCommand(Todo todo) : IRequest<Result<Todo>>;
    public class SaveTodoCommandHandler : IRequestHandler<SaveTodoCommand, Result<Todo>>
    {
        private readonly ITodoService _todoService;
        public SaveTodoCommandHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        public async Task<Result<Todo>> Handle(SaveTodoCommand request, CancellationToken cancellationToken)
        {
            Todo todo = null;
            if (request.todo.Id <= 0) todo = await _todoService.InsertTodoAsync(request.todo);
            else todo = await _todoService.UpdateTodoAsync(request.todo);
            return await Result<Todo>.SuccessAsync(todo);
        }
    }
}
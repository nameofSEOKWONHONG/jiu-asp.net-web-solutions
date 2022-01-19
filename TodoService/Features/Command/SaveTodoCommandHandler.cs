using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.Todo;
using Application.Response;
using Domain.Entities;
using MediatR;

namespace TodoService.Features.Command
{
    public record SaveTodoCommand(TB_TODO TbTodo) : IRequest<Result<TB_TODO>>;
    public class SaveTodoCommandHandler : IRequestHandler<SaveTodoCommand, Result<TB_TODO>>
    {
        private readonly ITodoService _todoService;
        public SaveTodoCommandHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        public async Task<Result<TB_TODO>> Handle(SaveTodoCommand request, CancellationToken cancellationToken)
        {
            TB_TODO tbTodo = null;
            if (request.TbTodo.ID <= 0) tbTodo = await _todoService.InsertTodoAsync(request.TbTodo);
            else tbTodo = await _todoService.UpdateTodoAsync(request.TbTodo);
            return await Result<TB_TODO>.SuccessAsync(tbTodo);
        }
    }
}
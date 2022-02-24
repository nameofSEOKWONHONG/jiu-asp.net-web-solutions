using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Response;
using MediatR;
using TodoService.Services;

namespace TodoService.Features.Command
{
    public record SaveTodoCommand(TB_TODO TbTodo) : IRequest<ResultBase<TB_TODO>>;
    public class SaveTodoCommandHandler : IRequestHandler<SaveTodoCommand, ResultBase<TB_TODO>>
    {
        private readonly ITodoService _todoService;
        public SaveTodoCommandHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        public async Task<ResultBase<TB_TODO>> Handle(SaveTodoCommand request, CancellationToken cancellationToken)
        {
            TB_TODO tbTodo = null;
            if (request.TbTodo.ID <= 0) tbTodo = await _todoService.InsertTodoAsync(request.TbTodo);
            else tbTodo = await _todoService.UpdateTodoAsync(request.TbTodo);
            return await ResultBase<TB_TODO>.SuccessAsync(tbTodo);
        }
    }
}
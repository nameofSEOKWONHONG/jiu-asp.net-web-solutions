using System.Threading;
using System.Threading.Tasks;
using Domain.Response;
using MediatR;
using TodoService.Services;

namespace TodoService.Features.Command
{
    public record RemoveTodoCommand(int id) : IRequest<ResultBase<bool>>;
    public class RemoveTodoCommandHandler : IRequestHandler<RemoveTodoCommand, ResultBase<bool>>
    {
        private readonly ITodoService _todoService;
        public RemoveTodoCommandHandler(ITodoService todoService)
        {
            _todoService = todoService;
        }
        public async Task<ResultBase<bool>> Handle(RemoveTodoCommand request, CancellationToken cancellationToken)
        {
            return await ResultBase<bool>.SuccessAsync(await _todoService.RemoveTodoAsync(request.id));
        }
    }
}
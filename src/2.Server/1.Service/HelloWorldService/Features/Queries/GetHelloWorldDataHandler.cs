using Application.Interfaces.HelloWorld;
using Domain.Response;
using MediatR;

namespace HelloWorldService.Features.Queries
{
    public record GetHelloWorldDataQuery() : IRequest<ResultBase<string>>;
    public class GetHelloWorldDataHandler : IRequestHandler<GetHelloWorldDataQuery, ResultBase<string>>
    {
        private readonly IHelloWorldService _helloWorldService;
        public GetHelloWorldDataHandler(IHelloWorldService helloWorldService)
        {
            _helloWorldService = helloWorldService;
        }
        public Task<ResultBase<string>> Handle(GetHelloWorldDataQuery request, CancellationToken cancellationToken)
        {
            return ResultBase<string>.SuccessAsync(data:_helloWorldService.HelloWorld());
        }
    }
}
using Application.Interfaces.HelloWorld;

namespace HelloWorldService.Services
{
    public class HelloWorldService : IHelloWorldService
    {
        public string HelloWorld()
        {
            return "HelloWorld";
        }
    }
}
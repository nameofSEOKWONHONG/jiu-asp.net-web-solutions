using System.Collections.Generic;
using System.Threading.Tasks;
using eXtensionSharp;

namespace Domain.Response
{
    public class ResultBase : IResultBase
    {
        public ResultBase()
        {
        }

        public List<string> Messages { get; set; } = new List<string>();

        public bool Succeeded { get; set; }

        public static IResultBase Fail()
        {
            return new ResultBase { Succeeded = false };
        }

        public static IResultBase Fail(string message)
        {
            return new ResultBase { Succeeded = false, Messages = new List<string> { message } };
        }

        public static IResultBase Fail(List<string> messages)
        {
            return new ResultBase { Succeeded = false, Messages = messages };
        }

        public static Task<IResultBase> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public static Task<IResultBase> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public static Task<IResultBase> FailAsync(List<string> messages)
        {
            return Task.FromResult(Fail(messages));
        }

        public static IResultBase Success()
        {
            return new ResultBase { Succeeded = true };
        }

        public static IResultBase Success(string message)
        {
            return new ResultBase { Succeeded = true, Messages = new List<string> { message } };
        }

        public static Task<IResultBase> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public static Task<IResultBase> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }
    }

    public class ResultBase<T> : ResultBase, IResultBase<T>
    {
        public ResultBase()
        {
        }

        public T Data { get; set; }

        public new static ResultBase<T> Fail()
        {
            return new ResultBase<T> { Succeeded = false };
        }

        public new static ResultBase<T> Fail(string message)
        {
            return new ResultBase<T> { Succeeded = false, Messages = new List<string> { message } };
        }

        public new static ResultBase<T> Fail(List<string> messages)
        {
            return new ResultBase<T> { Succeeded = false, Messages = messages };
        }
        
        public static ResultBase<T> Fail(T data)
        {
            return new ResultBase<T> { Succeeded = false, Data = data, 
                Messages = data.xIsEmpty() ? new List<string>{"data not exists or other problem"} : null };
        }

        public new static Task<ResultBase<T>> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        
        public new static Task<ResultBase<T>> FailAsync(T data)
        {
            return Task.FromResult(Fail(data));
        }

        public new static Task<ResultBase<T>> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public new static Task<ResultBase<T>> FailAsync(List<string> messages)
        {
            return Task.FromResult(Fail(messages));
        }

        public new static ResultBase<T> Success()
        {
            return new ResultBase<T> { Succeeded = true };
        }

        public new static ResultBase<T> Success(string message)
        {
            return new ResultBase<T> { Succeeded = true, Messages = new List<string> { message } };
        }

        public static ResultBase<T> Success(T data)
        {
            return new ResultBase<T> { Succeeded = true, Data = data };
        }

        public static ResultBase<T> Success(T data, string message)
        {
            return new ResultBase<T> { Succeeded = true, Data = data, Messages = new List<string> { message } };
        }

        public static ResultBase<T> Success(T data, List<string> messages)
        {
            return new ResultBase<T> { Succeeded = true, Data = data, Messages = messages };
        }

        public new static Task<ResultBase<T>> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public new static Task<ResultBase<T>> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }

        public static Task<ResultBase<T>> SuccessAsync(T data)
        {
            return Task.FromResult(Success(data));
        }

        public static Task<ResultBase<T>> SuccessAsync(T data, string message)
        {
            return Task.FromResult(Success(data, message));
        }
    }
}
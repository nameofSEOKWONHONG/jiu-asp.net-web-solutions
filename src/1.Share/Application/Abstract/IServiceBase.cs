using System.Collections.Generic;
using Domain.Response;

namespace Application.Abstract;

public interface IServiceBase<T>
{
     public ResultBase<T> Create(T item);
     public ResultBase<T> Update(T item);
     public ResultBase<T> Delete(T item);
     public ResultBase<T> Select(T item);
     public ResultBase<IEnumerable<T>> SelectAll(T request, int currentPage = 1, int pageSize = 50);
}
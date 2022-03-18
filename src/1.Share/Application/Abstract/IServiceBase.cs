using System.Collections.Generic;
using Domain.Response;

namespace Application.Abstract;

public interface IServiceBase<T>
{
     /// <summary>
     /// 단건 생성
     /// </summary>
     /// <param name="item"></param>
     /// <returns></returns>
     public ResultBase<T> Create(T item);
     
     /// <summary>
     /// 다건 생성
     /// </summary>
     /// <param name="items"></param>
     /// <returns></returns>
     public ResultBase<IEnumerable<T>> CreateBulk(IEnumerable<T> items);
     
     /// <summary>
     /// 단건 수정
     /// </summary>
     /// <param name="item"></param>
     /// <returns></returns>
     public ResultBase<T> Update(T item);
     
     /// <summary>
     /// 다건 수정
     /// </summary>
     /// <param name="items"></param>
     /// <returns></returns>
     public ResultBase<IEnumerable<T>> UpdateBulk(IEnumerable<T> items);
     
     /// <summary>
     /// 단건 삭제
     /// </summary>
     /// <param name="item"></param>
     /// <returns></returns>
     public ResultBase<T> Delete(T item);
     
     /// <summary>
     /// 다건 삭제
     /// </summary>
     /// <param name="items"></param>
     /// <returns></returns>
     public ResultBase<IEnumerable<T>> DeleteBulk(IEnumerable<T> items);
     
     /// <summary>
     /// 단건 조회
     /// </summary>
     /// <param name="item"></param>
     /// <returns></returns>
     public ResultBase<T> Select(T item);
     
     /// <summary>
     /// 다건 조회
     /// </summary>
     /// <param name="request"></param>
     /// <param name="currentPage"></param>
     /// <param name="pageSize"></param>
     /// <returns></returns>
     public ResultBase<IEnumerable<T>> SelectAll(T request, int currentPage = 1, int pageSize = 50);
}
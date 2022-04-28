using System.Collections.Generic;
using System.Linq;
using EFCore.BulkExtensions;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Application.Base;

public interface IRepositoryBase<T>
{
     /// <summary>
     /// 단건 생성
     /// </summary>
     /// <param name="item"></param>
     /// <returns></returns>
     T Create(T item);
     
     /// <summary>
     /// 다건 생성
     /// </summary>
     /// <param name="items"></param>
     /// <returns></returns>
     void CreateBulk(IEnumerable<T> items);
     
     /// <summary>
     /// 단건 수정
     /// </summary>
     /// <param name="item"></param>
     /// <returns></returns>
     T Update(T item);
     
     /// <summary>
     /// 다건 수정
     /// </summary>
     /// <param name="items"></param>
     /// <returns></returns>
     void UpdateBulk(IEnumerable<T> items);
     
     /// <summary>
     /// 단건 삭제
     /// </summary>
     /// <param name="item"></param>
     /// <returns></returns>
     T Delete(T item);
     
     /// <summary>
     /// 다건 삭제
     /// </summary>
     /// <param name="items"></param>
     /// <returns></returns>
     void DeleteBulk(IEnumerable<T> items);
     
     /// <summary>
     /// 단건 조회
     /// </summary>
     /// <param name="item"></param>
     /// <returns></returns>
     T Fetch(T item);
     
     /// <summary>
     /// 다건 조회
     /// </summary>
     /// <param name="request"></param>
     /// <param name="currentPage"></param>
     /// <param name="pageSize"></param>
     /// <returns></returns>
     IEnumerable<T> Query(T request, int currentPage = 1, int pageSize = 50);

     bool ComplexNonQuery(T entity);
     IEnumerable<TView> ComplexQuery<TView>(T entity) => default;     
}

public abstract class RepositoryBase<TDbContext, TEntity> : IRepositoryBase<TEntity>
     where TDbContext : DbContext
     where TEntity : class
{
     protected readonly TDbContext _dbContext;
     public RepositoryBase(TDbContext dbContext)
     {
          _dbContext = dbContext;
     }
     
     public TEntity Create(TEntity item)
     {
          var result = _dbContext.Add(item);
          _dbContext.SaveChanges();
          return result.Entity;
     }

     public void CreateBulk(IEnumerable<TEntity> items)
     {
          _dbContext.BulkInsert<TEntity>(items.ToList());
          _dbContext.SaveChanges();
     }

     public TEntity Update(TEntity item)
     {
          var result = _dbContext.Update(item);
          _dbContext.SaveChanges();
          return result.Entity;
     }

     public void UpdateBulk(IEnumerable<TEntity> items)
     {
          _dbContext.BulkUpdate(items.ToList());
          _dbContext.SaveChanges();
     }

     public TEntity Delete(TEntity item)
     {
          var result = _dbContext.Remove(item);
          _dbContext.SaveChanges();
          return result.Entity;
     }

     public void DeleteBulk(IEnumerable<TEntity> items)
     {
          _dbContext.BulkDelete(items.ToList());
          _dbContext.SaveChanges();
     }

     public virtual bool ComplexNonQuery(TEntity entity) => default;
     
     public virtual IEnumerable<TView> ComplexQuery<TView>(TEntity entity) => default;

     public virtual TEntity Fetch(TEntity item) => null;

     public virtual IEnumerable<TEntity> Query(TEntity request, int currentPage = 1, int pageSize = 50) => null;
}
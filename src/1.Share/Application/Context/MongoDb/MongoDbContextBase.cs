using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Configuration;
using Domain.Entities;
using Domain.Entities.Book;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Application.Context.MongoDb;

public class MongoDbContextBase<T> where T : NoSqlEntityBase
{
    protected readonly IMongoCollection<T> _collection;

    public MongoDbContextBase(string collectionName, IOptions<MongoDbOption> options)
    {
        var mongoClient = new MongoClient(options.Value.MongoDbConnectionInfos[collectionName].ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(options.Value.MongoDbConnectionInfos[collectionName].DatabaseName);
        _collection = mongoDatabase.GetCollection<T>(options.Value.MongoDbConnectionInfos[collectionName].CollectionName);
    }

    public virtual async Task<List<T>> GetAsync() => await _collection.Find(_ => true).ToListAsync();
    public virtual async Task<T> GetAsync(string id) => await _collection.Find(x => x.ID == id).FirstOrDefaultAsync();
    public virtual async Task CreateAsync(T item) => await _collection.InsertOneAsync(item);
    public virtual async Task UpdateAsync(string id, T item) => await _collection.ReplaceOneAsync(x => x.ID == id, item);
    public virtual async Task RemoveAsync(string id) => await _collection.DeleteOneAsync(x => x.ID == id);
}


//sample code
public class BookDbContext : MongoDbContextBase<Books>
{
    public BookDbContext(IOptions<MongoDbOption> options) : base(nameof(Books), options)
    {
        
    }
}
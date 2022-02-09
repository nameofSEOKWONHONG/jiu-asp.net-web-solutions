using System.Collections.Generic;

namespace Domain.Configuration;

public class MongoDbOption
{
    public Dictionary<string, MongoDbConnectionInfo> MongoDbConnectionInfos { get; set; }
}

public class MongoDbConnectionInfo
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string CollectionName { get; set; }    
}
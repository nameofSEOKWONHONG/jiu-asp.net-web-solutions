using Chloe.Mapper;

namespace Chloe.Query.Mapping
{
    public interface IObjectActivatorCreator
    {
        Type ObjectType { get; }
        bool IsRoot { get; set; }
        IObjectActivator CreateObjectActivator();
        IObjectActivator CreateObjectActivator(IDbContext dbContext);
        IFitter CreateFitter(IDbContext dbContext);
    }
}

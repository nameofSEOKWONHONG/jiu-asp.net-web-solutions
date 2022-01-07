namespace Chloe.Extension
{
    static class Utils
    {
        public static DbParam[] BuildParams(IDbContext dbContext, object parameter)
        {
            DbContext context = dbContext as DbContext;
            if (context == null)
            {
                var holdDbContextProp = dbContext.GetType().GetProperty("HoldDbContext");
                if (holdDbContextProp != null)
                {
                    dbContext = Chloe.Reflection.ReflectionExtension.FastGetMemberValue(holdDbContextProp, dbContext) as IDbContext;
                    return BuildParams(dbContext, parameter);
                }
            }

            return PublicHelper.BuildParams(context, parameter);
        }
    }
}

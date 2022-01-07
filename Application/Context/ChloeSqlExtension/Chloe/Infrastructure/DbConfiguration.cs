using Chloe.Entity;
using Chloe.Infrastructure.Interception;

namespace Chloe.Infrastructure
{
    public static class DbConfiguration
    {
        /// <summary>
        /// Fluent Mapping
        /// </summary>
        /// <param name="entityTypeBuilders"></param>
        public static void UseTypeBuilders(params IEntityTypeBuilder[] entityTypeBuilders)
        {
            EntityTypeContainer.UseBuilders(entityTypeBuilders);
        }
        /// <summary>
        /// Fluent Mapping
        /// </summary>
        /// <param name="entityTypeBuilderTypes"></param>
        public static void UseTypeBuilders(params Type[] entityTypeBuilderTypes)
        {
            EntityTypeContainer.UseBuilders(entityTypeBuilderTypes);
        }
        public static void UseEntities(params TypeDefinition[] entityTypeDefinitions)
        {
            EntityTypeContainer.Configure(entityTypeDefinitions);
        }

        public static void UseInterceptors(params IDbCommandInterceptor[] interceptors)
        {
            if (interceptors == null)
                return;

            foreach (var interceptor in interceptors)
            {
                if (interceptor == null)
                    continue;

                DbInterception.Add(interceptor);
            }
        }

        /// <summary>
        /// 配置映射类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static MappingTypeBuilder ConfigureMappingType<T>()
        {
            return MappingTypeSystem.Configure<T>();
        }
    }
}

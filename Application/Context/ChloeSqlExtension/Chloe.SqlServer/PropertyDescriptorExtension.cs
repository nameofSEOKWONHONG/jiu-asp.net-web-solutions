using Chloe.Descriptors;

namespace Chloe.SqlServer
{
    public static class PropertyDescriptorExtension
    {
        /// <summary>
        /// 判断字段是否为 timestamp 类型
        /// </summary>
        /// <param name="propertyDescriptor"></param>
        /// <returns></returns>
        public static bool IsTimestamp(this PrimitivePropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor.IsRowVersion && propertyDescriptor.PropertyType == PublicConstants.TypeOfByteArray;
        }
    }
}

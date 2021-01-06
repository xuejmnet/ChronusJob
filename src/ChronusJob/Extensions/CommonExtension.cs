using System;
using ChronusJob.Abstractions;

namespace ChronusJob.Extensions
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 06 January 2021 13:08:55
* @Email: 326308290@qq.com
*/
    public static class CommonExtension
    {
        
        public static bool IsJobType(this Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));
            return typeof(IJob).IsAssignableFrom(entityType);
        }
    }
}
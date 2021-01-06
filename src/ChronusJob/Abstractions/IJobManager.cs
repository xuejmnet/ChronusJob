using System;
using System.Reflection;

namespace ChronusJob.Abstractions
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 06 January 2021 13:10:13
* @Email: 326308290@qq.com
*/
    public interface IJobManager
    {
        void AddJobType(TypeInfo jobTypeInfo);
    }
}
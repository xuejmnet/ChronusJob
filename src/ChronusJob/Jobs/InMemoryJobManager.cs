using System;
using System.Collections.Generic;
using System.Reflection;
using ChronusJob.Abstractions;

namespace ChronusJob.Jobs
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 06 January 2021 13:11:38
* @Email: 326308290@qq.com
*/
    public class InMemoryJobManager:IJobManager
    {
        private readonly List<TypeInfo> _jobs = new List<TypeInfo>();
        public void AddJobType(TypeInfo jobTypeInfo)
        {
           if(!_jobs.Contains(jobTypeInfo))
               _jobs.Add(jobTypeInfo);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using ChronusJob.Jobs;

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
        void AddJob(JobEntry  jobEntry);

        List<JobEntry> GetNowRunJobs();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class InMemoryJobManager : IJobManager
    {
        private readonly List<JobEntry> _jobs = new List<JobEntry>();


        public void AddJob(JobEntry jobEntry)
        {
            if (_jobs.Any(job => job.JobName == jobEntry.JobName))
                throw new ArgumentException($"发现重复的任务名称:{jobEntry.JobName},请确认");
            _jobs.Add(jobEntry);
        }

        public List<JobEntry> GetNowRunJobs()
        {
            var now = DateTime.UtcNow;
            return _jobs.Where(o => o.NextUtcTime.HasValue && o.NextUtcTime.Value < now).ToList();
        }
    }
}
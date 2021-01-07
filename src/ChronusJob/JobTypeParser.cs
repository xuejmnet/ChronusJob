using System;
using System.Collections.Generic;
using System.Reflection;
using ChronusJob.Cron;
using ChronusJob.Extensions;
using ChronusJob.Jobs;
using ChronusJob.Jobs.Attributes;

namespace ChronusJob
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 06 January 2021 15:48:59
* @Email: 326308290@qq.com
*/
    public class JobTypeParser
    {
        private JobTypeParser(){}

        public static List<JobEntry> Parse(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!type.IsJobType())
                throw new NotSupportedException(type.FullName);
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var list = new List<JobEntry>();
            foreach (var method in methods)
            {
                var jobRun = method.GetCustomAttribute<JobRunAttribute>();
                if ( jobRun!= null)
                {
                    var now = DateTime.UtcNow;
                    var jobEntry = new JobEntry()
                    {
                        BeginUtcTime = jobRun.BeginUtcTime,
                        Cron = jobRun.Cron,
                        JobName = jobRun.Name ?? (type.Name + "." + method.Name),
                        NextUtcTime = new CronExpression(jobRun.Cron).GetTimeAfter(jobRun.BeginUtcTime),
                        SkipIfRunning = jobRun.SkipIfRunning,
                        JobClass = type,
                        JobMethod = method
                    };
                    list.Add(jobEntry);
                }
            }

            return list;
        }
    }
}
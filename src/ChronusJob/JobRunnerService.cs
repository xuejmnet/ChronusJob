using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChronusJob.Abstractions;
using ChronusJob.Cron;
using ChronusJob.Extensions;
using ChronusJob.Helpers;
using ChronusJob.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChronusJob
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 06 January 2021 13:00:11
* @Email: 326308290@qq.com
*/
    public class JobRunnerService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IJobManager _jobManager;
        private readonly ILogger<JobRunnerService> _logger;
        private readonly IJobFactory _jobFactory;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private const long DEFAULT_MILLIS = 1000L;

        /// <summary>
        /// 最大休眠时间30秒
        /// </summary>
        private const long MAX_DELAY_MILLIS = 30000L;

        public JobRunnerService(IServiceProvider serviceProvider, IJobManager jobManager, ILogger<JobRunnerService> logger, IJobFactory jobFactory)
        {
            _serviceProvider = serviceProvider;
            _jobManager = jobManager;
            _logger = logger;
            _jobFactory = jobFactory;
            Init();
        }

        private void Init()
        {
            var assemblies = AssemblyHelper.CurrentDomain.GetAssemblies();
            foreach (var x in assemblies)
            {
                // 查找接口为Job的类
                var types = x.DefinedTypes.Where(y => y.IsJobType()).ToList();
                foreach (var y in types)
                {
                    var jobs = JobTypeParser.Parse(y.AsType());
                    foreach (var job in jobs)
                    {
                        _jobManager.AddJob(job);
                    }
                }
            }
        }

        public async Task StartAsync()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                var delayMs = 0L;
                try
                {
                    delayMs = LoopJobAndGetWaitMillis();
                }
                catch (Exception e)
                {
                    _logger.LogError($"job runner service exception : {e}");
                    await Task.Delay((int) DEFAULT_MILLIS, _cts.Token);
                }

                if (delayMs > 0)
                    await Task.Delay((int) Math.Min(MAX_DELAY_MILLIS, delayMs), _cts.Token); //最大休息为MAX_DELAY_MILLIS
            }
        }

        public Task StopAsync()
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>next utc time that job will restart</returns>
        private long LoopJobAndGetWaitMillis()
        {
            var beginTime = UtcTime.CurrentTimeMillis();
            ;
            var costTime = 0L;
            var runJobs = _jobManager.GetNowRunJobs();
            if (!runJobs.Any())
            {
                var minJobUtcTime = _jobManager.GetNextJobUtcTime();
                if (!minJobUtcTime.HasValue)
                {
                    //return wait one second
                    costTime = UtcTime.CurrentTimeMillis() - beginTime;
                    if (DEFAULT_MILLIS < costTime)
                        return 0L;
                    return DEFAULT_MILLIS - costTime;
                }
                else
                {
                    //return next job run time
                    return UtcTime.InputUtcTimeMillis(minJobUtcTime.Value) - beginTime;
                }
            }

            foreach (var job in runJobs)
            {
                DoJob(job);
            }

            costTime = UtcTime.CurrentTimeMillis() - beginTime;
            if (costTime > DEFAULT_MILLIS)
            {
                return 0L;
            }

            return DEFAULT_MILLIS - costTime;
        }

        private void DoJob(JobEntry jobEntry)
        {
            if (jobEntry.StartRun())
            {
                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var job = _jobFactory.CreateJobInstance(scope, jobEntry);
                            if (job == null)
                                _logger.LogWarning($"###  job  [{jobEntry.JobName}] cant created, create method :{(jobEntry.CreateFromServiceProvider ? "from service provider" : "activator")}");
                            var method = jobEntry.JobMethod;
                            var @params = method.GetParameters().Select(x => scope.ServiceProvider.GetService(x.ParameterType)).ToArray();
#if DEBUG
                            _logger.LogInformation($"###  job  [{jobEntry.JobName}]  start success.");
#endif
                            if (method.IsAsyncMethod())
                            {
                                if (method.Invoke(job, @params) is Task task)
                                    await task;
                            }
                            else
                            {
                                method.Invoke(job, @params);
                            }
#if DEBUG
                            _logger.LogInformation($"###  job  [{jobEntry.JobName}]  invoke complete.");
#endif
                            jobEntry.NextUtcTime = new CronExpression(jobEntry.Cron).GetTimeAfter(DateTime.UtcNow);
                            if (!jobEntry.NextUtcTime.HasValue)
                                _logger.LogWarning($"###  job [{jobEntry.JobName}] is stopped.");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"###  job [{jobEntry.JobName}]  invoke fail : {e}.");
                    }
                    finally
                    {
                        jobEntry.CompleteRun();
                    }
                });
            }
        }
    }
}
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChronusJob.Abstractions;
using ChronusJob.Cron;
using ChronusJob.Extensions;
using ChronusJob.Helpers;
using ChronusJob.Jobs;
using ChronusJob.Jobs.Attributes;
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
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private const long DEFAULT_MILLIS = 1000L;

        public JobRunnerService(IServiceProvider serviceProvider, IJobManager jobManager, ILogger<JobRunnerService> logger)
        {
            _serviceProvider = serviceProvider;
            _jobManager = jobManager;
            _logger = logger;
            Init();
        }

        private void Init()
        {
            var assemblies = AssemblyHelper.CurrentDomain.GetAssemblies();
            foreach (var x in assemblies)
            {
                // 查找基类为Job的类
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
                var delayMs = 1000L;
                try
                {
                    var beginTime = UtcTime.CurrentTimeMillis();
                    LoopWork();
                    var costTime = UtcTime.CurrentTimeMillis() - beginTime;
                    if (DEFAULT_MILLIS < costTime)
                    {
                        delayMs = 0;
                    }
                    else
                    {
                        delayMs = DEFAULT_MILLIS - costTime;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"job runner service exception : {e}");
                    await Task.Delay((int) DEFAULT_MILLIS, _cts.Token);
                }

                await Task.Delay((int) delayMs, _cts.Token);
            }
        }

        public Task StopAsync()
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }

        private void LoopWork()
        {
            var runJobs = _jobManager.GetNowRunJobs();
            if (!runJobs.Any())
                return;
            foreach (var job in runJobs)
            {
                DoJob(job);
            }
        }

        private void DoJob(JobEntry jobEntry)
        {
            if (jobEntry.StartRun())
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var args = jobEntry.JobClass.GetConstructors()
                                .First()
                                .GetParameters()
                                .Select(x =>
                                {
                                    if (x.ParameterType == typeof(IServiceProvider))
                                        return scope.ServiceProvider;
                                    else
                                        return scope.ServiceProvider.GetService(x.ParameterType);
                                })
                                .ToArray();
                            var job = Activator.CreateInstance(jobEntry.JobClass, args);
                            var method = jobEntry.JobMethod;
                            var @params = method.GetParameters().Select(x => scope.ServiceProvider.GetService(x.ParameterType)).ToArray();

                            _logger.LogInformation($"job  [{jobEntry.JobName}]  start success");
                            method.Invoke(job, @params);
                            _logger.LogInformation($"job  [{jobEntry.JobName}]  invoke complete");
                            jobEntry.NextUtcTime = new CronExpression(jobEntry.Cron).GetTimeAfter(DateTime.UtcNow);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"job [{jobEntry.JobName}]  invoke fail : {e}");
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
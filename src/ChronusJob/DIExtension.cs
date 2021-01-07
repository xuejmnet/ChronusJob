using System;
using ChronusJob.Abstractions;
using ChronusJob.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace ChronusJob
{
/*
* @Author: xjm
* @Description:
* @Date: Thursday, 07 January 2021 13:34:52
* @Email: 326308290@qq.com
*/
    public class DIExtension
    {
        public IServiceCollection AddChronusJob(IServiceCollection services)
        {
            services.AddSingleton<IJobManager, InMemoryJobManager>()
                .AddSingleton<JobRunnerService>()
                .AddSingleton<ChronusJobBootstrapper>();
            return services;
        }
    }
}
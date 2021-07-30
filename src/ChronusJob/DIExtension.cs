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
    public static class DIExtension
    {
        public static IServiceCollection AddChronusJob(this IServiceCollection services, Action<IServiceProvider,JobGlobalOptions> config=null)
        {
            services.AddSingleton(sp =>
                {
                    var option = new JobGlobalOptions();
                    config?.Invoke(sp,option);
                    return option;
                }).AddSingleton<IJobManager, InMemoryJobManager>()
                .AddSingleton<JobRunnerService>()
                .AddSingleton<IJobFactory, DefaultJobFactory>()
                .AddHostedService<ChronusJobBootstrapper>();
            return services;
        }
    }
}
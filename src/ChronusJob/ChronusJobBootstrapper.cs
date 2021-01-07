using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace ChronusJob
{
/*
* @Author: xjm
* @Description:
* @Date: Thursday, 07 January 2021 13:29:24
* @Email: 326308290@qq.com
*/
    internal class ChronusJobBootstrapper : BackgroundService
    {
        private readonly JobRunnerService _jobRunnerService;

        public ChronusJobBootstrapper(JobRunnerService jobRunnerService)
        {
            _jobRunnerService = jobRunnerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _jobRunnerService.StartAsync();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _jobRunnerService.StopAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
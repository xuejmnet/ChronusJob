using System;
using ChronusJob.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace ChronusJob.Abstractions
{
/*
* @Author: xjm
* @Description:
* @Date: Friday, 08 January 2021 08:22:41
* @Email: 326308290@qq.com
*/
    public interface IJobFactory
    {
        object CreateJobInstance(IServiceScope scope,JobEntry jobEntry);
    }
}
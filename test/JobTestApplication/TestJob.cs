using System;
using System.Threading;
using ChronusJob.Abstractions;
using ChronusJob.Jobs.Attributes;

namespace JobTestApplication
{
/*
* @Author: xjm
* @Description:
* @Date: Thursday, 07 January 2021 13:38:45
* @Email: 326308290@qq.com
*/
    public class TestJob:IJob
    {
        [JobRun(Name = "printjob",Cron = "30 33 14 * * ?")]
        public void Print()
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            Thread.Sleep(new Random().Next(1,2000));
        }
    }
}
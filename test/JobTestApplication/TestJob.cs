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
        [JobRun(Name = "printjob",Cron = "*/10 * * * * ?",Begin = "2021-01-07 15:46:30")]
        public void Print()
        {
            Console.WriteLine("Print"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            Thread.Sleep(new Random().Next(1,2000));
        }
        // [JobRun(Name = "printjob1",Cron = "*/15 * * * * ?",Begin = "2021-01-07 15:46:30")]
        public void Print1()
        {
            Console.WriteLine("Print1"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            Thread.Sleep(new Random().Next(1,2000));
        }
    }
}
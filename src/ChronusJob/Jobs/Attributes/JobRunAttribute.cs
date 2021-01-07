using System;

namespace ChronusJob.Jobs.Attributes
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 06 January 2021 13:04:57
* @Email: 326308290@qq.com
*/
    public class JobRunAttribute:Attribute
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginUtcTime { get; set; }=DateTime.UtcNow;
        /// <summary>
        /// 默认每分钟执行
        /// </summary>
        public string Cron { get; set; } = "0 * * * * ? *";
        /// <summary>
        /// 如果正在运行就跳过
        /// </summary>
        public bool SkipIfRunning { get; set; } = true;
    }
}
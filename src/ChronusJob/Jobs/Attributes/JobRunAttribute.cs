using System;

namespace ChronusJob.Jobs.Attributes
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 06 January 2021 13:04:57
* @Email: 326308290@qq.com
*/
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class JobRunAttribute:Attribute
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginUtcTime { get; set; }

        public string Begin
        {
            get { return BeginUtcTime.ToString(); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var localTime = Convert.ToDateTime(value);
                    var utcOffset = TimeZoneInfo.Local.BaseUtcOffset.TotalMilliseconds;
                    BeginUtcTime = localTime.AddMilliseconds(-utcOffset);
                }
                else
                {
                    BeginUtcTime=DateTime.UtcNow;
                }
            }
        }
        /// <summary>
        /// 默认每分钟执行
        /// </summary>
        public string Cron { get; set; } = "0 * * * * ? *";
        /// <summary>
        /// 如果正在运行就跳过
        /// </summary>
        public bool SkipIfRunning { get; set; } = true;

        /// <summary>
        /// 是否从di容器中获取
        /// </summary>
        public bool CreateFromServiceProvider { get; set; } = false;
        /// <summary>
        /// 是否马上执行在程序启动后 需要优先满足Begin条件
        /// </summary>
        public bool RunOnceOnStart { get; set; } = false;
    }
}
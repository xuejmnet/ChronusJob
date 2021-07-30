using System;

namespace ChronusJob.Jobs
{
/*
* @Author: xjm
* @Description:
* @Date: Friday, 08 January 2021 08:16:08
* @Email: 326308290@qq.com
*/
    public class JobGlobalOptions
    {
        /// <summary>
        /// 延迟启动时间
        /// </summary>
        public int DelaySecondsOnStart { get; set; } = 0;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;
    }
}
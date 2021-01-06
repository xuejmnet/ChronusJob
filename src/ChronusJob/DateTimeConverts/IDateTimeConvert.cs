using System;

namespace ChronusJob.DateTimeConverts
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 06 January 2021 12:58:55
* @Email: 326308290@qq.com
*/
    public interface IDateTimeConvert
    {
        DateTime? ConvertTo(DateTime? utcTime);
    }
}
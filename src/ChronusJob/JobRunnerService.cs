using System;
using System.Linq;
using ChronusJob.Abstractions;
using ChronusJob.Extensions;
using ChronusJob.Helpers;

namespace ChronusJob
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 06 January 2021 13:00:11
* @Email: 326308290@qq.com
*/
    public class JobRunnerService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IJobManager _jobManager;

        public JobRunnerService(IServiceProvider serviceProvider,IJobManager jobManager)
        {
            _serviceProvider = serviceProvider;
            _jobManager = jobManager;
        }

        public void Init()
        {
            var assemblies = AssemblyHelper.CurrentDomain.GetAssemblies();
            foreach (var x in assemblies)
            {
                // 查找基类为Job的类
                var types = x.DefinedTypes.Where(y => y.IsJobType()).ToList();
                foreach (var y in types)
                {
                    _jobManager.AddJobType(y);
                }
            }
            
        }
    }
}
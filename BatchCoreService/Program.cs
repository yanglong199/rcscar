using System.ServiceProcess;

namespace BatchCoreService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;               //开服务
            ServicesToRun = new ServiceBase[] 
			{ 
				new BatchCoreService() 
			};
            ServiceBase.Run(ServicesToRun);           //在服务中调用运行的程序
        }
    }
}

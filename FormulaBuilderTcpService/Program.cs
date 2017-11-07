using System.ServiceProcess;

namespace FormulaBuilderTcpService
{
    static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var ServicesToRun = new ServiceBase[]
            {
                new TcpService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
using System.ServiceProcess;

namespace FormulaBuilderTcpService
{
    public partial class TcpService : ServiceBase
    {
        public TcpService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            AsyncSocketListener.StartListening();
        }

        protected override void OnStop()
        {
        }

#if DEBUG

        public void OnStartDebug(string[] args)
        {
            OnStart(args);
        }

        public void OnStopDebug()
        {
            OnStop();
        }

#endif
    }
}
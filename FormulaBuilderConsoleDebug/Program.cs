using System;
using System.ServiceModel;
using FormulaBuilderService;
using FormulaBuilderTcpService;

namespace FormulaBuilderConsoleDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new TcpService();

            service.OnStartDebug(args);

            service.OnStopDebug();
        }

        private static void PipeMessagingHost()
        {
            using (var host = new ServiceHost(
                typeof(FormulaBuilderFormulaService)))
            {
                host.Open();

                var pipeFactory =
                    new ChannelFactory<IFormulaService>(
                        new NetNamedPipeBinding(),
                        new EndpointAddress(
                            "net.pipe://localhost/FormulaBuilder"));

                var pipeProxy =
                    pipeFactory.CreateChannel();

                Console.WriteLine(pipeProxy.GetFormulasStringRepresentation(@"<request>
	                                <expression>
                                        <operation>minus</operation>
		                                <operand>
			                                <const>20</const>
		                                </operand>
		                                <operand>
			                                <expression>
				                                <operand>
					                                <const>10</const>
				                                </operand>
				                                <operand>
					                                <const>5</const>
				                                </operand>
				                                <operation>minus</operation>
			                                </expression>
		                                </operand>
	                                </expression>
                                </request>"));

                host.Close();
            }
        }
    }
}
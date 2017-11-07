using System;
using System.ServiceModel;

namespace FormulaBuilderClient
{
    [ServiceContract]
    public interface IFormulaService
    {
        [OperationContract]
        string GetFormulasStringRepresentation(string value);
    }


    class Program
    {
        static void Main(string[] args)
        {
            TcpMessaging();
            Console.ReadKey();
        }

        private static void TcpMessaging()
        {
            TcpClient.StartClient();
        }

        private static void PipeMessaging()
        {
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
				                                <operation>minus</operation>
				                                <operand>
					                                <const>10</const>
				                                </operand>
				                                <operand>
					                                 <expression>
				                                <operation>minus</operation>
				                                <operand>
					                                <const>10</const>
				                                </operand>
				                                <operand>
					                                <const>5</const>
				                                </operand>
			                                </expression>
				                                </operand>
			                                </expression>
		                                </operand>
	                                </expression>
                                </request>"));
        }
    }
}
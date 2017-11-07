using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FormulaBuilderClient
{
    public class TcpClient
    {
        public static void StartClient()
        {
            var requestXml = @"<request>
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
					                                 <expression>
                                        <operation>minus</operation>
		                                <operand>
			                                <const>20</const>
		                                </operand>
		                                <operand>
			                                <expression>
				                                <operation>minus</operation>
				                                <operand>
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
				                                </operand>
			                                </expression>
				                                </operand>
			                                </expression>
		                                </operand>
	                                </expression>
                                </request>";

            var bytes = new byte[2048];

            try
            {
                var ipHostInfo = Dns.GetHostEntry("localhost");
                var ipAddress = ipHostInfo.AddressList[1];
                var remoteEp = new IPEndPoint(ipAddress, 10001);

                var sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(remoteEp);

                Debug.WriteLine($"connected to {sender.RemoteEndPoint}");

                var msg = Encoding.ASCII.GetBytes($"{requestXml}<EOF>");

                sender.Send(msg);

                var bytesRec = sender.Receive(bytes);
                Debug.WriteLine($"result = {Encoding.ASCII.GetString(bytes, 0, bytesRec)}");

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}
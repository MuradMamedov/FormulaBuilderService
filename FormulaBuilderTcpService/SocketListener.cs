using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using FormulaBuilderLib;

namespace FormulaBuilderTcpService
{
    public class StateObject
    {
        public const int BufferSize = 1024;

        public Socket ClientSocket { get; set; } = null;

        public byte[] Buffer { get; } = new byte[BufferSize];

        public StringBuilder RecievedData { get; } = new StringBuilder();
    }

    public class AsyncSocketListener
    {
        public static readonly ManualResetEvent MessageRead =
            new ManualResetEvent(false); //event for returning to listening next client

        public static void StartListening()
        {
            var hostName = ConfigurationManager.AppSettings["Host"];
            var ipHostInfo = Dns.GetHostEntry(hostName);
            Debug.WriteLine($"HostName: {ipHostInfo.HostName}");

            var ip = ConfigurationManager.AppSettings["Address"];
            var ipAddress = ipHostInfo.AddressList.First(address => address.ToString().Equals(ip));
            Debug.WriteLine($"IPAddress: {ipAddress}");

            var port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            var localEndPoint = new IPEndPoint(ipAddress, port);
            Debug.WriteLine($"Port: {localEndPoint.Port}");

            var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                var backlog = int.Parse(ConfigurationManager.AppSettings["BackLog"]);
                listener.Bind(localEndPoint);
                listener.Listen(backlog);
                while (true)
                {
                    MessageRead.Reset();
                    listener.BeginAccept(AcceptCallback, listener);
                    MessageRead.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw; //something messy with configuration - abort
            }
        }

        public static void AcceptCallback(IAsyncResult result)
        {
            MessageRead.Set();
            var listener = (Socket) result.AsyncState;
            var handler = listener.EndAccept(result);

            var state = new StateObject {ClientSocket = handler};
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
        }

        public static void ReadCallback(IAsyncResult result)
        {
            try
            {
                var state = (StateObject) result.AsyncState;
                var handler = state.ClientSocket;

                var bytesRead = handler.EndReceive(result);

                if (bytesRead > 0)
                {
                    state.RecievedData.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
                    var eofString = "<EOF>";
                    var content = state.RecievedData.ToString();
                    if (content.IndexOf(eofString, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        var response = CreateResponse(content.Replace(eofString, ""));
                        Send(handler, response);
                    }
                    else
                    {
                        handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private static string CreateResponse(string value)
        {
            var response = new Response();
            RequestWrapper request;
            List<string> errors;
            using (Stream stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(value);
                writer.Flush();
                stream.Position = 0;
                request = WrapperFactory.CreateRequestWrapper(stream, out errors);
            }

            response.Result = request?.StringForm ?? "";
            response.Errors = errors.ToArray();

            var xsSubmit = new XmlSerializer(typeof(Response));
            using (var sww = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, response);
                    return sww.ToString();
                }
            }
        }

        private static void Send(Socket handler, string data)
        {
            Debug.WriteLine($"Sending {data} to client.");
            var byteData = Encoding.ASCII.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, handler);
        }

        private static void SendCallback(IAsyncResult result)
        {
            try
            {
                var handler = (Socket) result.AsyncState;
                var bytesSent = handler.EndSend(result);
                Debug.WriteLine($"Sent {bytesSent} bytes to client.");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
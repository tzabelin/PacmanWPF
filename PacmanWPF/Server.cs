using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server
{
    private Socket listener;

    public Server(int PortNumber)
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = host.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PortNumber);

        listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(localEndPoint);
        listener.Listen(1);

        StartServer();
    }
    private void StartServer()
    {
        try
        {
            Socket handler = listener.Accept();

            string data = null;
            byte[] bytes = null;

            bytes = new byte[1024];
            int bytesRec = 0;
            while (Encoding.ASCII.GetString(bytes, 0, bytesRec) != "END")
            {
                bytesRec = handler.Receive(bytes);
                data = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                byte[] msg = Encoding.ASCII.GetBytes(data);
                handler.Send(Encoding.ASCII.GetBytes("ACK"));
                Console.WriteLine("Text received : {0}\n", data);
            }

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            Console.ReadKey();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.ToString());
        }
    }
}

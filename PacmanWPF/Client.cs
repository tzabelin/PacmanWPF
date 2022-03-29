using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Client
{
    private Socket sender;
    private byte[] response;

    public Client(IPHostEntry DestinationHostName, int PortNumber)
    {
        response = new byte[1024];
        IPHostEntry host = DestinationHostName;
        IPAddress ipAddress = host.AddressList[0];
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, PortNumber);
        
        sender = new Socket(ipAddress.AddressFamily,
        SocketType.Stream, ProtocolType.Tcp);
        StartClient(remoteEP);
    }

    public Client(IPAddress DestinationIP, int PortNumber)
    {
        IPAddress ipAddress = DestinationIP;
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, PortNumber);

        sender = new Socket(ipAddress.AddressFamily,
        SocketType.Stream, ProtocolType.Tcp);
        StartClient(remoteEP);
    }

    public void SendData(string msg)
    {
        int bytesRec = 0;
        do
        {
            sender.Send(Encoding.ASCII.GetBytes(msg));
            bytesRec = sender.Receive(response);

            Console.WriteLine("Echoed test = {0}",
            Encoding.ASCII.GetString(response, 0, bytesRec));
        } while (Encoding.ASCII.GetString(response, 0, bytesRec) != "ACK");

        if (Encoding.ASCII.GetString(response, 0, bytesRec) == "END")
        {
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }

    private void StartClient(IPEndPoint remoteEP)
    {
        byte[] bytes = new byte[1024];
        
        try
        {
            sender.Connect(remoteEP);
            SendData("Test1");
         
            SendData("Test2");

            SendData("END");

            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            Console.ReadKey();
        }
      
        catch (Exception e)
        {
            Console.Error.WriteLine("Exception in net client: {0}", e.Message);
        }
    }
}

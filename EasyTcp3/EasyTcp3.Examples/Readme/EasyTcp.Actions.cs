using System;
using System.Net;
using EasyTcp3.Actions;
using EasyTcp3.Actions.ActionUtils;
using EasyTcp3.ClientUtils;
using EasyTcp3.Server;
using EasyTcp3.Server.ServerUtils;

namespace EasyTcp3.Examples.Readme
{
    public static class EasyTcp_Actions
    {
        const ushort PORT = 100;

        public static void Connect()
        {
            using var client = new EasyTcpClient();
            if(!client.Connect(IPAddress.Loopback, PORT)) return; 
            client.SendAction("ECHO","Hello server");
            client.SendAction("BROADCAST","Hello everyone"); 
        }
        
        public static void Run()
        {
            using var server = new EasyTcpActionServer().Start(PORT);
            server.OnConnect += (sender, client) => Console.WriteLine($"Client connected [ip: {client.GetIp()}]");
            server.OnDisconnect += (sender, client) => Console.WriteLine($"Client disconnected [ip: {client.GetIp()}]");
        }

        [EasyTcpAction("ECHO")]
        public static void EchoAction(object sender, Message message)
        {
            message.Client.Send(message);
        }

        [EasyTcpAction("BROADCAST")]
        public static void BroadCastAction(object sender, Message message)
        {
            var server = (EasyTcpServer) sender;
            server.SendAll(message);
        }
    }
}
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Extensions;
using Utilities.Net.Sockets;

namespace Utilities.Tests.Net
{
    [TestClass]
    public class SocketTests
    {
        private const Int32 WAIT_TIMEOUT = 5000;
        private readonly Byte[] _data = new Byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

        [TestMethod]
        public void SendDataBetweenOneTcpClientAndOneTcpServer()
        {
            var resetEvent = new ManualResetEvent(false);
            var address = IPAddress.Loopback;
            var port = 55555;
            var receivedData = new Byte[] { };
            var client = new SocketClient();
            var server = new SocketServer(address, port);
            server.ReceivedData += (s, e) => { receivedData = e.ReceivedData; resetEvent.Set(); };
            server.Start();
            client.Connect(address, port);
            client.SendData(_data);
            resetEvent.WaitOne(WAIT_TIMEOUT);
            server.Stop();
            server.Dispose();
            client.Dispose();

            CollectionAssert.AreEqual(_data, receivedData);
        }

        [TestMethod]
        public void SendDataBetweenTwentyTcpClientsAndOneTcpServer()
        {
            var resetEvent = new ManualResetEvent(false);
            var address = IPAddress.Loopback;
            var port = 55555;
            var numberClients = 20;
            var results = new ConcurrentDictionary<IPEndPoint, Byte[]>();
            var clients = new List<SocketClient>();
            var server = new SocketServer(address, port);
            server.ReceivedData += (s, e) => {
                var data = results.ContainsKey(e.RemotePoint) ? results[e.RemotePoint] : new Byte[] { };
                data = data.Concat(e.ReceivedData);
                results[e.RemotePoint] = data;
            };
            server.Start();
            
            for (var i = 0; i < numberClients; i++)
            {
                var client = new SocketClient();
                client.Connect(address, port);
                clients.Add(client);
            }
            for (var i = 0; i < numberClients; i++)
            {
                clients[i].SendData(_data);
            }
            
            resetEvent.WaitOne(WAIT_TIMEOUT);
            server.Stop();
            server.Dispose();
            for (var i = 0; i < numberClients; i++)
            {
                clients[i].Dispose();
            }

            Assert.AreEqual(numberClients, results.Count);
            foreach (Byte[] data in results.Values)
            {
                CollectionAssert.AreEqual(_data, data);                
            }
        }

        [TestMethod]
        public void SendDataBetweenOneUdpClientAndOneUdpServer()
        {
            var resetEvent = new ManualResetEvent(false);
            var address = IPAddress.Loopback;
            var port = 55555;
            var receivedData = new Byte[] { };
            var client = new SocketDatagramClient();
            var server = new SocketDatagramServer(port);
            server.ReceivedData += (s, e) => { receivedData = e.ReceivedData; resetEvent.Set(); };
            server.Start();
            client.Connect(address, port);
            client.SendData(_data);
            resetEvent.WaitOne(WAIT_TIMEOUT);
            server.Stop();
            server.Dispose();
            client.Dispose();

            CollectionAssert.AreEqual(_data, receivedData);
        }
    }
}

using BullyAlgorithm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace BullyAlgorithm.Services
{
    public class ServerSocket
    {

        private Socket _server;
        private ConcurrentBag<Socket> _cleints;
        private List<int> dd = new List<int>(); 
        public ServerSocket(CancellationToken ct)
        {

            _cleints = new ConcurrentBag<Socket>();
            InitSocketServer(ct);
        }
        private async Task InitSocketServer(CancellationToken ct)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            var _bindingAddress = new IPEndPoint(ipAddress, 1000);

            _server = new Socket(_bindingAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(_bindingAddress);
            _server.Listen(1);
            //try
            //{

            //    var handler = await _server.AcceptAsync();
            //    _cleints.Add(handler);
            //    while (!ct.IsCancellationRequested)
            //    {
            //        int x=handler.SendBufferSize;

            //        byte[] buffer = new byte[300];
            //        var recieved = await handler.ReceiveAsync(buffer, SocketFlags.None);

            //        foreach(var socket in _cleints)
            //        {
            //            if(socket!=handler)
            //            {
            //                await socket.SendAsync(buffer, SocketFlags.None);
            //            }
            //        }

            //    }
            //}catch(Exception ex)
            //{

            //}
            //_server.ReceiveTimeout = 5000;
            //_server.SendTimeout = 5000;
            _server.BeginAccept(new AsyncCallback(AcceptCallBack), null);

        }

        private void AcceptCallBack(IAsyncResult Ar)
        {
            var socket = _server.EndAccept(Ar);
            var buffer = new byte[33];
            _cleints.Add(socket);
            try
            {
                while (true)
                {
                    string data = "";
                    while (true)
                    {
                        int rec = socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                        data += Encoding.ASCII.GetString(buffer, 0, rec);
                        if (data.LastIndexOf('}') == rec - 1)
                            break;
                    }
                    if (!string.IsNullOrEmpty(data))
                    {
                        var messages = ReadAllBufferMessages(data);
                        //send all messages
                        foreach (var msg in messages)
                        {
                            foreach (var client in _cleints)
                            {
                                if (client != socket && client.Connected)
                                {
                                    var sendBuffer = Encoding.ASCII.GetBytes(msg);
                                    client.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
                                }
                            }
                        }
                    }
                    //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), new { socket, buffer });
                    _server.BeginAccept(new AsyncCallback(AcceptCallBack), null);
                }
            }catch(SocketException ex)
            {

                //_server.BeginAccept(new AsyncCallback(AcceptCallBack), null);
            }

        }

        private void ReceiveCallBack(IAsyncResult Ar)
        {
            dynamic state = Ar.AsyncState;
            Socket socket = (Socket)state.socket;
            var recieved = socket.EndReceive(Ar);
            byte[] data = new byte[recieved];
            dd.Add(recieved);
            Array.Copy((byte[])state.buffer, data, recieved);

            string msgStr = Encoding.ASCII.GetString(data);
            //var message = JsonConvert.DeserializeObject<Message>(msgStr);
            foreach(var client in _cleints)
            {
                if(client!=socket)
                    client.Send(data);
            }

            var buffer = new byte[300];
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), new { socket, buffer });

        }

        private List<string> ReadAllBufferMessages(string buffer)
        {
            var bufferString = buffer.ToCharArray();
            int idx = 0;
            List<string> ans = new List<string>();

            while(idx<bufferString.Length)
            {
                int endOfMessage = FindChar(bufferString, '}');
                if (endOfMessage != -1)
                {
                    string message = SubString(bufferString, idx, endOfMessage);
                    idx = endOfMessage + 1;

                    bufferString[endOfMessage] = '*';
                    ans.Add(message);
                }
                else
                    break;
            }

            return ans;

        }
        private int FindChar(char[]arr,char val)
        {
            for(int i=0;i<arr.Length ;++i)
            {
                if (arr[i] == val)
                    return i;
            }
            return -1;
        }
        private string SubString(char[] arr,int start,int end)
        {
            string ans = "";
            for(int i=start; i<=end;++i)
            {
                ans += arr[i];
            }
            return ans;
        }

    }
}

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
        private List<Socket> _cleints;
        private List<int> dd = new List<int>(); 
        public ServerSocket()
        {

            _cleints = new List<Socket>();
            InitSocketServer();
        }
        private void InitSocketServer()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            var _bindingAddress = new IPEndPoint(ipAddress, 1000);

            _server = new Socket(_bindingAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(_bindingAddress);
            _server.Listen(1);
            _server.BeginAccept(new AsyncCallback(AcceptCallBack), null);

        }

        private void AcceptCallBack(IAsyncResult Ar)
        {
            var socket = _server.EndAccept(Ar);
            //message size 
            var buffer = new byte[33];
            _cleints.Add(socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), new { socket, buffer });
            _server.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }

        private void ReceiveCallBack(IAsyncResult Ar)
        {
            dynamic state = Ar.AsyncState;
            Socket socket = (Socket)state.socket;
            var recieved = socket.EndReceive(Ar);
            byte[] data = new byte[recieved];
            
            Array.Copy((byte[])state.buffer, data, recieved);

            string msgStr = Encoding.ASCII.GetString(data);
            dd.Add(recieved);

            foreach (var client in _cleints)
            {
                if (client != socket && client.Connected)
                    client.Send(data, 0, data.Length, SocketFlags.None);
            }

            var buffer = new byte[33];
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

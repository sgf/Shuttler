using System;
using System.Linq;
using System.Collections.Generic;

using System.Text;
using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.Security.Cryptography;
using Shuttler.Artery;

namespace Shuttler.IM.Client
{
   class Program
   {
      static void Main(string[] args)
      {
         Client client = new Client();
         client.Start();

         Console.Read();
      }
   }

   class Client
   {
      private IDebugger _debug = new Debugger<Client>(Level.Info);
      private byte[] _socketBuffer = new byte[1024];
      private Socket _socket;
      private bool _isConnected;

      public void Start()
      {
         SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
         _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

         EndPoint sep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);

         SocketAsyncEventArgs socketConnectionArgs = new SocketAsyncEventArgs();
         socketConnectionArgs.UserToken = sep;
         socketConnectionArgs.RemoteEndPoint = sep;
         socketConnectionArgs.Completed += SocketConnect_Completed;

         _socket.ConnectAsync(socketConnectionArgs);
      }

      private void SocketConnect_Completed(object sender, SocketAsyncEventArgs e)
      {
         try
         {
            _isConnected = (e.SocketError == SocketError.Success);
            if (_isConnected)
            {
               Console.WriteLine("ID");
               string id = "overred@gmail.com";
               string pwd = "overred";

               _socket.Send(LoginBytes(id, pwd));
               SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
               receiveArgs.SetBuffer(_socketBuffer, 0, 1024);
               receiveArgs.Completed += SocketRead_Completed;
               _socket.ReceiveAsync(receiveArgs);

            }
         }
         catch (Exception ex)
         {
            _debug.Error(ex, "SocketConnect_Completed");
         }
      }

      private void SocketRead_Completed(object sender, SocketAsyncEventArgs e)
      {
         try
         {
            int bytesRead = e.BytesTransferred;
            if (bytesRead > 0)
            {
               SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
               receiveArgs.SetBuffer(_socketBuffer, 0, 1024);
               receiveArgs.Completed += SocketRead_Completed;
               _socket.ReceiveAsync(receiveArgs);

               StreamBuffer stream = new StreamBuffer(_socketBuffer);

               IPacket packet = new Packet();
               packet.Read(stream);

               Response response = new Response(packet);
               _debug.Info(response);

               ushort command = response.Command;
               switch (command)
               {
                  case 1:
                     LoginHandler(response);
                     break;
                  case 2:
                     //GetBuddyHandler(stream);
                     break;
               }
            }

         }
         catch (Exception ex)
         {
            _debug.Error(ex, "SocketRead_Completed");
         }
      }

      private void LoginHandler(Response response)
      {

         if (response.Body.Any((n) => n ==0x01))
            Console.WriteLine("Login Success!");
         else
            Console.WriteLine("Pwd Wrong!");
      }

      private byte[] LoginBytes(string id, string pwd)
      {
         using (StreamBuffer stream = new StreamBuffer())
         {

            byte[] header =Global.TOAD_HEADER;

            stream.Put(header);
            ushort command = 0x0001;
            stream.Put(command);
            byte[] tidStream = Encoding.UTF8.GetBytes(id);
            byte[] tidBytes = new byte[40];
            Array.Copy(tidStream, tidBytes, tidStream.Length);

            stream.Put(tidBytes);//from
            stream.Put(tidBytes);//to

            long callID = 0L;
            stream.Put(callID);

            byte[] pwdHexs = Encoding.UTF8.GetBytes(pwd);
            byte[] remains = SHA1.Create().ComputeHash(pwdHexs);

            stream.Put(remains.Length + 1);
            stream.Put(remains);

            byte status = 0x01;
            stream.Put(status);

            byte[] tail = Global.TOAD_TAIL;
            stream.Put(tail);

            return stream.ToByteArrays();
         }
      }

   }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   internal class ArgsPoolTuples
   {

      private static BufferPoolManager[] _bufferPool=new BufferPoolManager[2];
      private static SocketArgsPoolManager[] _socketArgsPool=new SocketArgsPoolManager[2];

      public static void InitializeAll()
      {
         _bufferPool[ArgsMode.Send] = new BufferPoolManager();
         _bufferPool[ArgsMode.Send].Initialize();

         _bufferPool[ArgsMode.Receive] = new BufferPoolManager();
         _bufferPool[ArgsMode.Receive].Initialize();

         _socketArgsPool[ArgsMode.Send] = new SocketArgsPoolManager();
         _socketArgsPool[ArgsMode.Send].Initialize();

         _socketArgsPool[ArgsMode.Receive] = new SocketArgsPoolManager();
         _socketArgsPool[ArgsMode.Receive].Initialize();
      }

      public static void DisposeAll()
      {
         _bufferPool[ArgsMode.Send].Dispose();
         _bufferPool[ArgsMode.Receive].Dispose();

         _socketArgsPool[ArgsMode.Send].Dispose();
         _socketArgsPool[ArgsMode.Receive].Dispose();
      }


      public static SocketArgsPoolManager SendArgsTuple
      { get { return _socketArgsPool[ArgsMode.Send]; } }

      public static SocketArgsPoolManager ReceiveArgsTuple
      { get { return _socketArgsPool[ArgsMode.Receive]; } }

      public static BufferPoolManager SendBufferTuple
      { get { return _bufferPool[ArgsMode.Send]; } }

      public static BufferPoolManager ReceiveBufferTuple
      { get { return _bufferPool[ArgsMode.Receive]; } }

   }

   internal static class ArgsMode 
   {
      public  const int Send = 0, Receive = 1;
   }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shuttler.Artery
{
   public static class ByteBufferExtensions
   {
      public static byte[] BuffersBlockCopy(this byte[] src, int offset, int count)
      {
         var dst = new byte[count];
         Buffer.BlockCopy(src, offset, dst, 0, count);
         return dst;
      }

      public static byte[] BufferTrimFront(this byte[] buffer)
      {
          int i=0;
          int offset = 0;
          int len = buffer.Length;
          for (; i < len; i++)
          {
              byte b = buffer[i];
              if (b != 0)
              {
                  offset = i;
                  break;
              }
          }

          if(offset==0&&i>0)
              return new byte[0];

          byte[] tem = new byte[len - offset];
          Buffer.BlockCopy(buffer, offset, tem, 0, len - offset);
          return tem;
      }

      public static byte[] BufferTrimEnd(this byte[] buffer)
      {
          int offset = 0;
          int len = buffer.Length;
          int i = len - 1;
          for (; i >= 0; i--)
          {
              byte b = buffer[i];
              if (b != 0)
              {
                  offset = i;
                  break;
              }

          }
          if(i==0&&offset==0) 
              return new byte[0];
          byte[] tem = new byte[offset + 1];
          Buffer.BlockCopy(buffer, 0, tem, 0, offset + 1);
          return tem;
      }

   }
}

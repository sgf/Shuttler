using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;      
namespace ProtoBuf
{
   public class SerializerExt
   {
      public static byte[] SerializerStreamToBytes<T>(T instance)
      {
         using (Stream stream = new MemoryStream())
         {
            //if(instance==null)return new byte[0];

            Serializer.Serialize<T>(stream, instance);
            int length = (int)stream.Length;

            byte[] bytes = new byte[length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, length);
            return bytes;
         }
      }

      public static T DeserializeBytesToT<T>(byte[] buffer)
      {
         using (Stream stream = new MemoryStream(buffer))
         {
            try
            {
               return Serializer.Deserialize<T>(stream);
            }
            catch (Exception ex) { }
            return default(T);
         }
      }
   }
}

using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Shuttler.Artery
{
   [ProtoContract]
   public class RpcContext<T>
   {
      [ProtoMember(1)]
      public string MethodName { get; set; }

      [ProtoMember(2)]
      public T Value { get; set; }

      [ProtoMember(3)]
      public string RpcException { get; set; }

      [ProtoMember(4)]
      public bool RetBoolResult { get; set; }
   }
}

using System;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   public  static class Global
   {
	 /// <summary>
	 /// length:byte
	 /// </summary>
	 public const int TOAD_HEADER_LENGTH = 3;
      public const int TOAD_TAIL_LENGTH = 3;

      /// <summary>
      /// Non-num shuttlerid length
      /// </summary>
      public const int TOAD_TOADIDCHARS_LENGTH = 40;

      public const int TOAD_BODY_LENGTH = 1024;

	 /// <summary>
	 /// const
	 /// </summary>
	 public static readonly byte[] TOAD_HEADER={0x01,0x01,0x01};
	 public static readonly byte[] TOAD_TAIL ={0x03,0x03,0x03};
   }
}

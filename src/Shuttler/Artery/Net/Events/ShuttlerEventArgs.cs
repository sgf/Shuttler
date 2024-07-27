using System;
using System.Collections.Generic;

namespace Shuttler.Artery
{
   [Serializable]
   public class ShuttlerEventArgs:EventArgs
   {
	 public ShuttlerEventArgs(ShuttlerAgent shuttler,string token)
	 {
	    Shuttler = shuttler;
	    Token = token;
	 }

      public ShuttlerAgent Shuttler
      { get; private set; }

      public string Token
      { get; private set; }
   }
}

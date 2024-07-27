using System;
using System.Net;

namespace Shuttler.Artery
{
   public class Utility
   {
	 public static IPEndPoint IPEndPointParser(string epid)
	 {
	    if (!epid.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase))
		  throw new ArgumentException("Invalid IPEndPoint");

	    epid=epid.Substring(4);
	    int pos=epid.LastIndexOf(':');
	    if (pos <= 0 || pos > epid.Length - 1)
		  throw new ArgumentException("Epid Format *error*!");
	    return new IPEndPoint(IPAddress.Parse(epid.Substring(0, pos)), int.Parse(epid.Substring(pos + 1)));
	 }
   }
}

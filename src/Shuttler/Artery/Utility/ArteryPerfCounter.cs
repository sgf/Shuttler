using System;
using System.Diagnostics;

namespace Shuttler.Artery
{

   [PerfCounterCategoryAttribute("Artery")]
   internal class ArteryPerfCounter
   {
      public readonly static ArteryPerfCounter Instance = PerfCounterFactory.Create<ArteryPerfCounter>();

      [PerfCounterAttribute("#Connected Total.", "", PerformanceCounterType.NumberOfItems32)]
      public PerformanceCounter NumberOfConnected;

      [PerfCounterAttribute("#DisConnected Total.", "", PerformanceCounterType.NumberOfItems32)]
      public PerformanceCounter NumberOfDisConnected;

      [PerfCounterAttribute("#SocketRec Total.", "", PerformanceCounterType.NumberOfItems32)]
      public PerformanceCounter NumberOfSocketRec;

      [PerfCounterAttribute("#ParserError Total.", "", PerformanceCounterType.NumberOfItems32)]
      public PerformanceCounter NumberOfParserError;

      [PerfCounterAttribute("#ParserSuccess Total.", "", PerformanceCounterType.NumberOfItems32)]
      public PerformanceCounter NumberOfParserSucc;


      [PerfCounterAttribute("Connected /Sec.", "", PerformanceCounterType.RateOfCountsPerSecond32)]
      public PerformanceCounter RateOfConnected;

      [PerfCounterAttribute("DisConnected /Sec.", "", PerformanceCounterType.RateOfCountsPerSecond32)]
      public PerformanceCounter RateOfDisConnected;
      //rpc
      [PerfCounterAttribute("RpcServer Invoked /Sec.", "", PerformanceCounterType.RateOfCountsPerSecond32)]
      public PerformanceCounter RateOfRpcServerInvoked;

      [PerfCounterAttribute("RpcClient Invoke /Sec.", "", PerformanceCounterType.RateOfCountsPerSecond32)]
      public PerformanceCounter RateOfRpcClientInvoke;

      [PerfCounterAttribute("#Transaction Clear Total.", "", PerformanceCounterType.NumberOfItems32)]
      public PerformanceCounter NumberOfTranClear;

   }
}

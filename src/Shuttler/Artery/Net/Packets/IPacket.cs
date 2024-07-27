using System;
using System.Linq;
using System.Collections.Generic;

namespace Shuttler.Artery
{
    public interface IPacket
    {
        Shuttler_Header Header { get; }
        Shuttler_FromChars From { get; }
        Shuttler_ToChars To { get; }
        Shuttler_Command Command { get; }
        Shuttler_CallID CallID { get; }
        Shuttler_Length Length { get; }
        Shuttler_Body Body { get; set; }
        Shuttler_Tail Tail { get; }
        bool Read(StreamBuffer stream);
    }
}

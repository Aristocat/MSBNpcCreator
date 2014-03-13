using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBNpcCreator
{
    public struct MaplePacket
    {
        public long TimeStamp { get; set; }
        public ushort Length { get; set; }
        public ushort Opcode { get; set; }
        public byte[] Buffer { get; set; }
    }
}

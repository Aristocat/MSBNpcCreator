using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBNpcCreator
{
    public static class RecvOpcode
    {
        public static ushort CHANGE_MAP;
        public static ushort NPC_TALK;
        public static ushort NPC_TALK_MORE;
    }

    public static class SendOpcode
    {
        public static ushort SHOW_STATUS_INFO;
        public static ushort NPC_TALK;
        public static ushort WARP_TO_MAP;
    }
}

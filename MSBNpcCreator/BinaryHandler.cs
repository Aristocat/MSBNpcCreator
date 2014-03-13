using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSBNpcCreator
{
    class BinaryHandler
    {
        private static ushort mLocalPort = 0;
        private static ushort mRemotePort = 0;
        private static ushort mBuild = 0;
        private static byte mLocale = 0;

        private static string mRemoteEndpoint = "???";
        private static string mLocalEndpoint = "???";

        public static void ParseBinary(string pPath)
        {
            using (FileStream Stream = new FileStream(pPath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader Reader = new BinaryReader(Stream);
                mBuild = Reader.ReadUInt16();
                ushort version = mBuild;

                if (mBuild == 0x2012)
                {
                    mLocale = (byte)Reader.ReadUInt16();
                    mBuild = Reader.ReadUInt16();
                    mLocalPort = Reader.ReadUInt16();
                }
                else if (mBuild == 0x2014)
                {
                    mLocalEndpoint = Reader.ReadString();
                    mLocalPort = Reader.ReadUInt16();
                    mRemoteEndpoint = Reader.ReadString();
                    mRemotePort = Reader.ReadUInt16();

                    mLocale = (byte)Reader.ReadUInt16();
                    mBuild = Reader.ReadUInt16();
                }
                else if (mBuild == 0x2015 || mBuild == 0x2020)
                {
                    mLocalEndpoint = Reader.ReadString();
                    mLocalPort = Reader.ReadUInt16();
                    mRemoteEndpoint = Reader.ReadString();
                    mRemotePort = Reader.ReadUInt16();

                    mLocale = Reader.ReadByte();
                    mBuild = Reader.ReadUInt16();
                }
                else
                {
                    mLocalPort = Reader.ReadUInt16();
                }

                while (Stream.Position < Stream.Length)
                {
                    long Timestamp = Reader.ReadInt64();
                    ushort Size = Reader.ReadUInt16();
                    ushort Opcode = Reader.ReadUInt16();
                    bool Outbound;
                    if (version >= 0x2020)
                    {
                        Outbound = Reader.ReadBoolean();
                    }
                    else
                    {
                        Outbound = (Size & 0x8000) != 0;
                        Size = (ushort)(Size & 0x7FFF);
                    }

                    //string OpcodeName = "0x" + Opcode.ToString("X4");

                    byte[] Buffer = Reader.ReadBytes(Size);

                    HandlePacket(Opcode, new Packet(Buffer), Outbound);
                }
            }
        }

        public static int NpcID = 0;
        public static int lastMsg = 0, mode = 1, status = 0, myStatus = 0, selection = -1;
        public static SortedDictionary<int, string> selections;
        public static bool ChangingMap = false;
        public static StringBuilder Script = null;

        public static void HandlePacket(ushort Opcode, Packet pPacket, bool Recv)
        {
            if (!Recv && Opcode == (ushort)SendOpcode.NPC_TALK)
            {
                if (Script == null)
                    ResetNPC();
                pPacket.ReadByte();
                int npcId = pPacket.ReadInt();
                if (NpcID == 0)
                    NpcID = npcId;
                if (NpcID != npcId)
                {
                    PrintNPC();
                    ResetNPC();
                    NpcID = npcId;
                }
                if (pPacket.ReadBool()) //new v141
                    pPacket.ReadInt();
                byte msgType = pPacket.ReadByte();
                byte msgOption = pPacket.ReadByte();
                int fakeNPC = 0;
                if ((msgOption & 4) != 0)
                    fakeNPC = pPacket.ReadInt();
                if (status != myStatus)
                {
                    Script.Append("\t} else if (status == ").Append(status).Append(") {\r\n");
                    Script.Append("\t\t");
                    myStatus = status;
                }
                bool select = selection > -1;
                if (select)
                {
                    for (int i = 0; i < selections.Count; i++)
                        if (selections.Keys.ElementAt<int>(i) < selection)
                        {
                            if (Script[Script.Length - 1] != '\t')
                                Script.Append("\t\t");
                            Script.Append("if (selection == ");
                            Script.Append(selections.Keys.ElementAt<int>(i));
                            Script.Append(") {\r\n\t\t//");
                            Script.Append(selections[selections.Keys.ElementAt<int>(i)]);
                            Script.Append("\r\n\t\t}\r\n\t\telse ");
                        }
                    Script.Append("if (selection == ").Append(selection).Append(") {\r\n");
                    Script.Append("\t\t\t");
                }
                Script.Append("cm.");
                string msg = pPacket.ReadMapleString();
                if (msgType == 1)
                {
                    bool prev = pPacket.ReadBool();
                    bool next = pPacket.ReadBool();
                }
                else if (msgType == 5)
                {
                    selections = new SortedDictionary<int, string>();
                    foreach (string s in Split(msg, "#l"))
                    {
                        if (Split(s, "#L").Length < 2)
                            continue;
                        int optNum = int.Parse(Split(Split(s, "#L")[1], "#")[0]);
                        string opt = Split(Split(s, "#L")[1], "#")[1];
                        selections.Add(optNum, opt);
                    }
                }
                if (msgType == 0 && pPacket.Remaining > 0)
                {
                    short msgID = pPacket.ReadShort();
                    switch (msgID)
                    {
                        case 0:
                            Script.Append("sendOk(\"");
                            break;
                        case 1:
                            Script.Append("sendPrev(\"");
                            break;
                        case 256:
                            Script.Append("sendNext(\"");
                            break;
                        case 257:
                            Script.Append("sendNextPrev(\"");
                            break;
                        default:
                            Script.Append("sendUnknown(\"");
                            break;
                    }
                }
                else
                {
                    switch (msgType)
                    {
                        case 1:
                            Script.Append("sendImage(\"");
                            break;
                        case 2:
                            Script.Append("sendYesNo(\"");
                            break;
                        case 5:
                            Script.Append("sendSimple(\"");
                            break;
                        case 15:
                            Script.Append("sendAcceptDecline(\"");
                            break;
                        default:
                            Script.Append("sendUnknown(\"");
                            break;
                    }
                }
                Script.Append(msg.Replace("\r", "\\r").Replace("\n", "\\n")).Append("\"");
                if (msgType != 1 && msgOption > 0)
                {
                    Script.Append(", ").Append(msgOption);
                    if ((msgOption & 4) != 0)
                        Script.Append(", ").Append(fakeNPC);
                }
                Script.Append(");\r\n");
            }
            else if (!Recv && Opcode == (ushort)SendOpcode.WARP_TO_MAP)
            {
                if (ChangingMap && Script != null)
                {
                    if (status != myStatus)
                    {
                        Script.Append("\t} else if (status == ").Append(status).Append(") {\r\n");
                        Script.Append("\t\t");
                        myStatus = status;
                    }
                    pPacket.Skip(36);
                    Script.Append("cm.warp(").Append(pPacket.ReadInt());
                    Script.Append(", ").Append(pPacket.ReadByte()).Append(");\r\n\t\t");
                    PrintNPC();
                    ResetNPC();
                }
                ChangingMap = false;
                if (Script != null)
                    PrintNPC();
            }
            else if (!Recv && Opcode == (ushort)SendOpcode.SHOW_STATUS_INFO)
            {
                if (Script == null)
                    return;
                if (status != myStatus)
                {
                    Script.Append("\t} else if (status == ").Append(status).Append(") {\r\n");
                    Script.Append("\t\t");
                    myStatus = status;
                }
                byte mode = pPacket.ReadByte();
                switch (mode)
                {
                    case 0:
                        pPacket.Skip(1);
                        Script.Append("cm.gainItem(");
                        Script.Append(pPacket.ReadInt());
                        Script.Append(", ");
                        Script.Append(pPacket.ReadInt());
                        Script.Append(");\r\n\t\t");
                        break;
                    case 3:
                        pPacket.ReadBool();
                        Script.Append("cm.gainExp(").Append(pPacket.ReadLong()).Append(");\r\n\t\t");
                        break;
                    case 0xC:
                        Script.Append("cm.getPlayer().updateInfoQuest(");
                        Script.Append(pPacket.ReadUShort());
                        Script.Append(", \"");
                        Script.Append(pPacket.ReadMapleString());
                        Script.Append("\");\r\n\t\t");
                        break;
                    default:
                        Script.Append("//Action: ").Append(mode).Append(" Data: ");
                        Script.Append(BitConverter.ToString(pPacket.ReadLeftoverBytes()).Replace("-", " "));
                        Script.Append("\r\n\t\t");
                        break;
                }
            }
            else if (Recv && Opcode == (ushort)RecvOpcode.NPC_TALK)
            {
                PrintNPC();
                ResetNPC();
            }
            else if (Recv && Opcode == (ushort)RecvOpcode.NPC_TALK_MORE)
            {
                if (Script == null)
                    return;
                lastMsg = pPacket.ReadByte();
                mode = pPacket.ReadByte();
                if (mode == 1)
                    status++;
                if (selection > -1)
                {
                    if (Script[Script.Length - 1] != '\t')
                        Script.Append("\t\t");
                    Script.Append("}\r\n");
                    for (int i = 0; i < selections.Count; i++)
                        if (selections.Keys.ElementAt<int>(i) > selection)
                        {
                            if (Script[Script.Length - 1] != '\t')
                                Script.Append("\t\t");
                            Script.Append("else if (selection == ");
                            Script.Append(selections.Keys.ElementAt<int>(i));
                            Script.Append(") {\r\n\t\t//");
                            Script.Append(selections[selections.Keys.ElementAt<int>(i)]);
                            Script.Append("\r\n\t\t}\r\n");
                        }
                }
                if (lastMsg == 5)
                    selection = pPacket.ReadInt();
                else
                    selection = -1;
            }
            else if (Recv && Opcode == (ushort)RecvOpcode.CHANGE_MAP)
            {
                ChangingMap = true;
            }
        }

        public static void PrintNPC()
        {
            if (NpcID <= 0 || Script == null)
                return;
            if (Script[Script.Length - 1] != '\t')
                Script.Append("\t\t");
            Script.Append("cm.dispose();\r\n\t}\r\n}");
            System.Windows.Forms.Clipboard.SetText(Script.ToString().Replace("\t", "    "));
            System.Windows.Forms.MessageBox.Show(Script.ToString().Replace("\t", "    ")
                + "\r\n\r\n\r\nScript copied to clipboard.");
            Script = null;
        }

        public static void ResetNPC()
        {
            NpcID = 0;
            lastMsg = 0;
            mode = 1;
            status = 0;
            myStatus = 0;
            ChangingMap = false;
            Script = new StringBuilder();
            Script.Append("var status = -1;\r\n\r\n");
            Script.Append("function start() {\r\n");
            Script.Append("\taction(1, 0, 0);\r\n");
            Script.Append("}\r\n\r\n");
            Script.Append("function action(mode, type, selection) {\r\n");
            Script.Append("\tif (mode == 0 && type == 0) {\r\n");
            Script.Append("\t\tstatus--;\r\n");
            Script.Append("\t} else if (mode == -1) {\r\n");
            Script.Append("\t\tcm.dispose();\r\n");
            Script.Append("\t\treturn;\r\n");
            Script.Append("\t} else {\r\n");
            Script.Append("\t\tstatus++;\r\n");
            Script.Append("\t}\r\n");
            Script.Append("\tif (status == 0) {\r\n\t\t");
        }

        public static string[] Split(string toSplit, string split)
        {
            return toSplit.Split(new string[] { split }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}

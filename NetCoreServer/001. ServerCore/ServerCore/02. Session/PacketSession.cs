using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;

            while (true)
            {
                if (buffer.Count < HeaderSize) break;

                ushort packetSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < packetSize) break;

                OnParsePacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, packetSize));

                processLen += packetSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + packetSize, buffer.Count - packetSize);
            }

            return processLen;
        }

        public abstract void OnParsePacket(ArraySegment<byte> buffer);
    }
}

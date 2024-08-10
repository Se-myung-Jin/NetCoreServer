using ServerCore;
using System.Text;

namespace Server
{
    public enum PacketID
    {
        PlayerInfoReq = 1,
        ChatReq = 2,
        ChatRes = 3,
    }

    public class ChatReq : IPacket
    {
        public string Chat { get; set; }
        public ushort PacketId { get { return (ushort)PacketID.ChatReq; } }

        public void ReadPacket(ArraySegment<byte> segment)
        {
            ushort count = 0;

            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            count += sizeof(ushort);
            ushort chatLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
            count += sizeof(ushort);
            Chat = Encoding.Unicode.GetString(span.Slice(count, chatLen));
            count += chatLen;
        }

        public ArraySegment<byte> WritePacket()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), PacketId);
            count += sizeof(ushort);
            ushort chatLen = (ushort)Encoding.Unicode.GetBytes(Chat, 0, Chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), chatLen);
            count += sizeof(ushort);
            count += chatLen;
            success &= BitConverter.TryWriteBytes(span, count);

            if (success == false) return null;

            return SendBufferHelper.Close(count);
        }
    }

    public class ChatRes : IPacket
    {
        public int PlayerId { get; set; }
        public string Chat { get; set; }
        public ushort PacketId { get { return (ushort)PacketID.ChatRes; } }

        public void ReadPacket(ArraySegment<byte> segment)
        {
            ushort count = 0;

            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            count += sizeof(ushort);
            this.PlayerId = BitConverter.ToInt32(span.Slice(count, span.Length - count));
            count += sizeof(int);
            ushort chatLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
            count += sizeof(ushort);
            Chat = Encoding.Unicode.GetString(span.Slice(count, chatLen));
            count += chatLen;
        }

        public ArraySegment<byte> WritePacket()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), PacketId);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), PlayerId);
            count += sizeof(int);
            ushort chatLen = (ushort)Encoding.Unicode.GetBytes(Chat, 0, Chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), chatLen);
            count += sizeof(ushort);
            count += chatLen;
            success &= BitConverter.TryWriteBytes(span, count);

            if (success == false) return null;

            return SendBufferHelper.Close(count);
        }
    }

    public class PlayerInfoReq : IPacket
    {
        public long PlayerId { get; set; }
        public string Name { get; set; }
        public ushort PacketId { get { return (ushort)PacketID.PlayerInfoReq; } }

        public List<SkillInfo> Skills = new List<SkillInfo>();

        public struct SkillInfo
        {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte> span, ref ushort count)
            {
                bool success = true;

                success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), id);
                count += sizeof(int);
                success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), level);
                count += sizeof(short);
                success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), duration);
                count += sizeof(float);

                return success;
            }

            public void Read(ReadOnlySpan<byte> span, ref ushort count)
            {
                id = BitConverter.ToInt32(span.Slice(count, span.Length - count));
                count += sizeof(int);
                level = BitConverter.ToInt16(span.Slice(count, span.Length - count));
                count += sizeof(short);
                duration = BitConverter.ToSingle(span.Slice(count, span.Length - count));
                count += sizeof(float);
            }
        }

        public void ReadPacket(ArraySegment<byte> segment)
        {
            ushort count = 0;

            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            count += sizeof(ushort);
            this.PlayerId = BitConverter.ToInt64(span.Slice(count, span.Length - count));
            count += sizeof(long);
            ushort nameLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
            count += sizeof(ushort);
            this.Name = Encoding.Unicode.GetString(span.Slice(count, nameLen));
            count += nameLen;
            Skills.Clear();
            ushort skillLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
            count += sizeof(ushort);
            for (int i = 0; i < skillLen; i++)
            {
                SkillInfo skillInfo = new SkillInfo();
                skillInfo.Read(span, ref count);
                Skills.Add(skillInfo);
            }
        }

        public ArraySegment<byte> WritePacket()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), PacketId);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.PlayerId);
            count += sizeof(long);
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.Name, 0, this.Name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), nameLen);
            count += sizeof(ushort);
            count += nameLen;
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)Skills.Count);
            count += sizeof(ushort);
            foreach (SkillInfo skill in Skills)
            {
                success &= skill.Write(span, ref count);
            }
            success &= BitConverter.TryWriteBytes(span, count);

            if (success == false) return null;

            return SendBufferHelper.Close(count);
        }
    }
}

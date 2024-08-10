using ServerCore;

namespace Server
{
    public interface IRoom<T> where T : PacketSession
    {
        void Enter(T session);
        void Leave(T session);
    }
}

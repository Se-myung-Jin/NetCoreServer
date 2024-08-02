namespace ServerCore
{
    public interface IJobQueue
    {
        void Push(Action job);
    }
}

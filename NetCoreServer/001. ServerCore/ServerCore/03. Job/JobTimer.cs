namespace ServerCore
{
    public class JobTimer
    {
        PriorityQueue<JobTimerItem> priorityQueue = new PriorityQueue<JobTimerItem>();
        object lockObject = new object();

        public static JobTimer Instance { get; } = new JobTimer();

        public void Push(Action action, int tickAfter = 0)
        {
            JobTimerItem item = new JobTimerItem();
            item.SetTimeTick(System.Environment.TickCount + tickAfter);
            item.SetAction(action);

            lock (lockObject)
            {
                priorityQueue.Push(item);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;

                JobTimerItem item;

                lock (lockObject)
                {
                    if (priorityQueue.Count == 0) break;

                    item = priorityQueue.Peek();
                    if (item.CanExecJobTime(now) == false) break;

                    priorityQueue.Pop();
                }

                item.DoAction();
            }
        }
    }
}

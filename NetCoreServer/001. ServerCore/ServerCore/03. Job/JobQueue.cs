namespace ServerCore
{
    public class JobQueue : IJobQueue
    {
        Queue<Action> jobQueue = new Queue<Action>();

        object lockObject = new object();
        bool flush = false;

        public void Push(Action job)
        {
            bool flush = false;

            lock (lockObject)
            {
                jobQueue.Enqueue(job);
                
                if (this.flush == false)
                {
                    this.flush = flush = true;
                }
            }

            if (flush)
            {
                Flush();
            }
        }

        Action Pop()
        {
            lock (lockObject)
            {
                if (jobQueue.Count == 0)
                {
                    this.flush = false;
                    return null;
                }

                return jobQueue.Dequeue();
            }
        }

        void Flush()
        {
            while (true)
            {
                Action action = Pop();
                if (action == null) return;

                action.Invoke();
            }
        }
    }
}

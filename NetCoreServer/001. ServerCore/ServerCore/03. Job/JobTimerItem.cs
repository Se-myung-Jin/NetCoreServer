namespace ServerCore
{
    struct JobTimerItem : IComparable<JobTimerItem>
    {
        Action action;
        int execTick;

        public int CompareTo(JobTimerItem other)
        {
            return other.execTick - execTick;
        }

        public void SetTimeTick(int tick)
        {
            execTick = tick;
        }

        public void SetAction(Action action)
        {
            this.action = action;
        }

        public bool CanExecJobTime(int tick)
        {
            return execTick <= tick;
        }

        public void DoAction()
        {
            action.Invoke();
        }
    }
}

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public struct ScheduleResult
    {
        public bool ScheduleComplete { get;}

        public int UsersChanged { get; }

        public ScheduleResult(bool scheduleComplete, int usersChanged = 0)
        {
            ScheduleComplete = scheduleComplete;
            UsersChanged = usersChanged;
        }
    }
}
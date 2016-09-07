using System;

namespace Domain
{
    public class CascadeLevel
    {
        protected CascadeLevel()
        {

        }

        public CascadeLevel(int orderNumber, NotificationTypes notificationTypes, Workplace responsible = null, int headLevel = 0, TimeSpan? delay = null)
        {
            Responsible = responsible;
            HeadLevel = headLevel;
            TriggeredAt = null;
            OrderNumber = orderNumber;
            NotificationTypes = notificationTypes;
            if (delay.HasValue)
            {
                DelayTicks = delay.Value.Ticks;
            }
        }

        public int OrderNumber { get; set; }
        public long DelayTicks { get; private set; }
        public Workplace Responsible { get; set; }
        public int HeadLevel { get; set; }
        public DateTime? TriggeredAt { get; set; }
        public NotificationTypes NotificationTypes { get; set; }

        public TimeSpan Delay
        {
            get { return new TimeSpan(DelayTicks); }
            set { DelayTicks = value.Ticks; }
        }

        public void Start()
        {
            TriggeredAt = DateTime.Now;
        }
    }
}
using System;

namespace Domain
{
    public class WayPoint
    {
        public Workplace FromWorkplace { get; protected set; }
        public Workplace ToWorkplace { get; protected set; }
        public WayPoint Parent { get; set; }
        public bool IsClosed { get; private set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? ClosedAt { get; private set; }

        protected WayPoint()
        {
        }

        public override string ToString()
        {
            return $"FromWorkplace: {FromWorkplace}, ToWorkplace: {ToWorkplace}, IsClosed: {IsClosed}, CreatedAt: {CreatedAt}, ClosedAt: {ClosedAt}";
        }

        public WayPoint(Workplace fromWorkplace, Workplace toWorkplace, WayPoint parent = null)
        {
            FromWorkplace = fromWorkplace;
            ToWorkplace = toWorkplace;
            Parent = parent;
            IsClosed = false;
            CreatedAt = DateTime.Now;
        }

        public void Close()
        {
            IsClosed = true;
            ClosedAt = DateTime.Now;
        }
    }
}
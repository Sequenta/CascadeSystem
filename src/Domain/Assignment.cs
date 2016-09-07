using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Domain
{
    public class Assignment
    {
        protected Assignment()
        {
        }

        public Assignment(string title, string text, DateTime deadline, Cascade cascade = null)
        {
            Title = title;
            Text = text;
            Deadline = deadline;
            CreatedAt = DateTime.Now;
            Cascade = cascade;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsFinish { get; set; }
        public bool IsStart { get; set; }
        public Cascade Cascade { get; set; }

        /// <summary>
        /// Дата создания поручения
        /// </summary>
        public DateTime CreatedAt { get; protected set; }


        /// <summary>
        /// Срок выполнения
        /// </summary>
        public DateTime Deadline { get; set; }
        public List<WayPoint> WayPoints { get; set; }

        public override string ToString()
        {
            return $"Поручение: {Title}";
        }

        public void SendToResponsibles(Workplace sender, Workplace[] responsibles)
        {
            foreach (var responsible in responsibles)
            {
                WayPoints.Add(new WayPoint(sender, responsible));
            }
        }
    }
}
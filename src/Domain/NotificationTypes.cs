using System;
using System.ComponentModel;

namespace Domain
{
    [Flags]
    public enum NotificationTypes
    {
        [Description("Отправка поручения")]
        None = 1,
        [Description("Отправка поручения")]
        Assignment = 2,
        [Description("Оповещение по электронной почте")]
        Email = 4,
        [Description("Оповещение оп SMS")]
        Sms = 8
    }
}
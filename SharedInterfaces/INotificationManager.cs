using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoCompany.Interfaces
{
    /// <summary>
    ///     Allows an object to implement a NotificationManager, and represents a method
    ///     that are used to notify recipients using particular way.
    /// </summary>
    public interface INotificationManager
    {
        void NotifyAbout<T>(IEnumerable<T> info);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoCompany.Interfaces
{
    public interface INotificationManager
    {
        void NotifyAbout<T>(IEnumerable<T> info);
    }
}

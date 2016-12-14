using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInterfaces
{
    public interface IChangeDetector
    {
        event EventHandler<string> ErrorEvent;
        event EventHandler ChangeHasDetectedEvent;
        void FindChanges();
    }
}

using System;
using NoCompany.Interfaces;

namespace NoCompany.Data
{
    public class ViabilityObserver : IViabilityObserver
    {
        public event EventHandler SomeBodyStillAlive;

        public void Update(object sender)
        {
            SomeBodyStillAlive(sender, EventArgs.Empty);
        }
    }
}
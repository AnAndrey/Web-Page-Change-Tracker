using System;

namespace NoCompany.Interfaces
{
    public interface IViabilityObserver
    {
        /// <summary>
        /// To signal about healthy status.
        /// </summary>
        event EventHandler SomeBodyStillAlive;

        /// <summary>
        /// Let observer know about activity
        /// </summary>
        void Update(object sender);
    }
}
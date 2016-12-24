using System;

namespace NoCompany.Interfaces
{
    public interface IViable
    {
        /// <summary>
        /// Event to distinguish time consumable operations and hangs.
        /// </summary>
        event EventHandler ImStillAlive;
    }
}
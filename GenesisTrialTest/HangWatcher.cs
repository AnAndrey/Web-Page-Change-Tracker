using System;
using System.Threading;

namespace GenesisTrialTest
{
    internal class HangWatcher : IDisposable
    {
        private CancellationTokenSource _cts;
        public HangWatcher(int millisecondsTimeOut)
        {
            _cts = new CancellationTokenSource(millisecondsTimeOut);
        }

        public void PostPone(int millisecondsDelay)
        {
            _cts.CancelAfter(millisecondsDelay);
        }

        public void Dispose()
        {
            if (_cts != null)
            {
                _cts.Dispose();
                _cts = null;
            }
        }
        public CancellationToken Token { get { return _cts.Token; } }
    }
}

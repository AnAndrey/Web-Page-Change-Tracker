using NoCompany.Interfaces;

namespace NoCompany.Data
{
    public abstract class ControlableExecutionBase : ICancelable, IViable
    {
        private volatile bool isCancellationRequested;
        public virtual bool IsCancellationRequested
        {
            get { return isCancellationRequested; }
            set { isCancellationRequested = value; }
        }

        public virtual void Cancel()
        {
            IsCancellationRequested = true;
        }

        protected virtual void ShouldStopOperating()
        {
            if (IsCancellationRequested)
                throw new StopOperatoinException();
        }

        protected IViabilityObserver _viabilityObserver;
        public virtual IViabilityObserver ViabilityObserver
        {
            get { return _viabilityObserver; }
            set { _viabilityObserver = value; }
        }
        protected virtual void KeepTracking()
        {
            if (ViabilityObserver != null)
                ViabilityObserver.Update(this);
        }
    }
}
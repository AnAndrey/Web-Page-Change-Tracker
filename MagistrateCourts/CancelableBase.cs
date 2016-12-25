namespace NoCompany.Data
{
    public abstract class CancelableBase 
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
    }
}
namespace NoCompany.Interfaces
{
    public interface ICancelable
    {
        void Cancel();
        bool IsCancellationRequested { get; }
    }
}
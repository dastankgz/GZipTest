namespace GZipTest.Abstract
{
    public interface ICommand
    {
        void Execute();
        void Cancel();

        bool IsCompleted { get; }
    }
}
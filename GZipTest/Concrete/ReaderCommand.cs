using System.Threading;
using GZipTest.Abstract;
using NLog;

namespace GZipTest.Concrete
{
    public class ReaderCommand : ICommand
    {
        private readonly IBlockReader _reader;
        private readonly BlockSynchronizer _synchronizer;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ReaderCommand(IBlockReader reader, BlockSynchronizer synchronizer)
        {
            _reader = reader;
            _synchronizer = synchronizer;
        }

        public void Execute()
        {
            var readerThread = new Thread(Read);
            readerThread.Start();
        }

        public void Cancel()
        {
            throw new System.NotImplementedException();
        }

        public bool IsCompleted { get; private set; }

        private void Read()
        {
            while (true)
            {
                var block = _reader.ReadBlock();
                if (block == Block.Null)
                {
                    IsCompleted = true;
                    _synchronizer.SetDone();
                    Logger.Trace("All blocks are read");
                    return;
                }
                Logger.Trace($"Block #{block.Id} is read");
                _synchronizer.Set(block);
            }
        }
    }
}
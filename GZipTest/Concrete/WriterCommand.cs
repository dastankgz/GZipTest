using System;
using System.Threading;
using GZipTest.Abstract;
using NLog;

namespace GZipTest.Concrete
{
    public class WriterCommand : ICommand
    {
        private readonly BlockSynchronizer _synchronizer;
        private readonly IBlockWriter _writer;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public WriterCommand(BlockSynchronizer synchronizer, IBlockWriter writer)
        {
            _synchronizer = synchronizer;
            _writer = writer;
        }

        public void Execute()
        {
            var thread = new Thread(WriteBlock);
            thread.Start();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public bool IsCompleted { get; private set; }

        private void WriteBlock()
        {
            while (true)
            {
                var block = _synchronizer.GetNext();

                if (block == Block.Null)
                {
                    Logger.Trace("All blocks are written");
                    IsCompleted = true;
                    return;
                }

                _writer.WriteBlock(block);
                Logger.Trace($"Block #{block.Id} is written");
            }
        }
    }
}
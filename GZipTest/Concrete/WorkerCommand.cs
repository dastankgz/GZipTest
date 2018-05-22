using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GZipTest.Abstract;
using NLog;

namespace GZipTest.Concrete
{
    public class WorkerCommand : ICommand
    {
        private readonly BlockSynchronizer _readSync;
        private readonly BlockSynchronizer _writeSync;
        private readonly IBlockHandler _handler;
        private readonly int _threadsCount;
        private readonly List<Thread> _threads;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public WorkerCommand(BlockSynchronizer readSync, BlockSynchronizer writeSync, IBlockHandler handler, int threadsCount)
        {
            _readSync = readSync;
            _writeSync = writeSync;
            _threadsCount = threadsCount;
            _threads = new List<Thread>(_threadsCount);
            _handler = handler;
        }

        public void Execute()
        {
            for (int i = 0; i < _threadsCount; i++)
            {
                var thread = new Thread(CompressBlock);
                _threads.Add(thread);
                thread.Start();
                Logger.Trace($"Worker thread #{thread.ManagedThreadId} started");
            }
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public bool IsCompleted
        {
            get { return _threads.TrueForAll(x => !x.IsAlive); }
        }

        private void CompressBlock()
        {
            while (true)
            {
                var block = _readSync.GetNext();

                if (block == Block.Null)
                {
                    _writeSync.SetDone();

                    if (_threads.Count(x => !x.IsAlive) - 1 == 0)
                        Logger.Trace("All blocks are handled");

                    return;
                }

                block = _handler.Handle(block);
                _writeSync.Set(block);
                Logger.Trace($"Worker thread #{Thread.CurrentThread.ManagedThreadId} | Block #{block.Id} is handled");
            }
        }
    }
}
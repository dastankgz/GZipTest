using System;
using System.Collections.Generic;
using System.Threading;

namespace GZipTest
{
    public class BlockSynchronizer
    {
        private readonly Dictionary<int, Block> _list = new Dictionary<int, Block>();
        private readonly object _lockObj = new object();
        private int _id;
        private int _maxSize;
        private bool _isDone;

        public BlockSynchronizer(int initialIdVal, int maxSize)
        {
            _id = initialIdVal;
            _maxSize = maxSize;
        }

        public void SetDone()
        {
            if (_isDone)
                return;

            lock (_lockObj)
            {
                _isDone = true;
            }
        }

        public void Set(Block block)
        {
            if (block == Block.Null)
                throw new InvalidOperationException("Block.Null was inserted");

            lock (_lockObj)
            {
                while (_list.Count >= _maxSize)
                    Monitor.Wait(_lockObj);

                _list.Add(block.Id, block);
                Monitor.PulseAll(_lockObj);
            }
        }

        public Block GetNext()
        {
            lock (_lockObj)
            {
                while (!_list.ContainsKey(_id))
                {
                    if (_isDone && _list.Count == 0)
                    {
                        Monitor.PulseAll(_lockObj);
                        return Block.Null;
                    }

                    Monitor.Wait(_lockObj);
                }

                var block = _list[_id];
                _list.Remove(_id);
                _id++;
                Monitor.PulseAll(_lockObj);
                return block;
            }
        }
    }
}
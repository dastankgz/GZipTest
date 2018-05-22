using System.IO;
using GZipTest.Abstract;

namespace GZipTest.Concrete
{
    public class RawBlockReader : IBlockReader
    {
        private readonly int _blockSize;
        private readonly FileStream _stream;
        private int _id;

        public RawBlockReader(FileStream stream, int blockSize, int initialIdVal)
        {
            _stream = stream;
            _blockSize = blockSize;
            _id = initialIdVal;
        }

        private int DefineBlockSize()
        {
            var size = _stream.Length - _stream.Position;
            if (size <= _blockSize)
                return (int) size;

            return _blockSize;
        }

        public Block ReadBlock()
        {
            if (_stream.Position >= _stream.Length)
                return Block.Null;

            var size = DefineBlockSize();
            var buffer = new byte[size];
            _stream.Read(buffer, 0, size);

            var block = new Block(_id++, buffer);
            return block;
        }
    }
}
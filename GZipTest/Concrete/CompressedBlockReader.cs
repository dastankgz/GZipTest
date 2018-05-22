using System;
using System.IO;
using GZipTest.Abstract;

namespace GZipTest.Concrete
{
    public class CompressedBlockReader : IBlockReader
    {
        private readonly FileStream _stream;
        private int _id;
        
        public CompressedBlockReader(FileStream stream, int initIdVal)
        {
            _stream = stream;
            _id = initIdVal;
        }

        private int DefineBlockSize()
        {
            var header = BitConverter.GetBytes((int) 1);
            var buffer = new byte[header.Length];
            _stream.Read(buffer, 0, header.Length);
            var size = BitConverter.ToInt32(buffer, 0);
            return size;
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
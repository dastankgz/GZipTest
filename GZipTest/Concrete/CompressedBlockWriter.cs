using System;
using System.IO;
using GZipTest.Abstract;

namespace GZipTest.Concrete
{
    public class CompressedBlockWriter : IBlockWriter
    {
        private readonly string _path;

        public CompressedBlockWriter(string path)
        {
            _path = path;
        }

        public void WriteBlock(Block block)
        {
            using (var stream = new FileStream(_path, FileMode.Append))
            {
                var size = block.Processed.Length;
                var header = BitConverter.GetBytes(size);
                stream.Write(header, 0, header.Length);
                stream.Write(block.Processed, 0, block.Processed.Length);
            }
        }
    }
}
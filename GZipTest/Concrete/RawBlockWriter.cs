using System.IO;
using GZipTest.Abstract;

namespace GZipTest.Concrete
{
    public class RawBlockWriter : IBlockWriter
    {
        private readonly string _path;

        public RawBlockWriter(string path)
        {
            _path = path;
        }

        public void WriteBlock(Block block)
        {
            using (var stream = new FileStream(_path, FileMode.Append))
                stream.Write(block.Processed, 0, block.Processed.Length);
        }
    }
}
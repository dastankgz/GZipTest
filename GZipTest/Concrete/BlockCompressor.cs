using System.IO;
using System.IO.Compression;
using GZipTest.Abstract;

namespace GZipTest.Concrete
{
    public class BlockCompressor : IBlockHandler
    {
        public Block Handle(Block block)
        {
            using (var stream = new MemoryStream())
            {
                using (var gz = new GZipStream(stream, CompressionMode.Compress))
                    gz.Write(block.Data, 0, block.Data.Length);

                block.Processed = stream.ToArray();
                return block;
            }
        }
    }
}
using System.IO;
using System.IO.Compression;
using GZipTest.Abstract;

namespace GZipTest.Concrete
{
    public class BlockDecompressor : IBlockHandler
    {
        public Block Handle(Block block)
        {
            using (var stream = new MemoryStream(block.Data))
            {
                using (var gz = new GZipStream(stream, CompressionMode.Decompress))
                {
                    var buffer = new byte[4096];
                    using (var memory = new MemoryStream())
                    {
                        int numRead;
                        while ((numRead = gz.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            memory.Write(buffer, 0, numRead);
                        }

                        block.Processed = memory.ToArray();
                        return block;
                    }
                }
            }
        }
    }
}
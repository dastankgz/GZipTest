using System;
using System.Collections.Generic;
using System.IO;
using GZipTest.Abstract;
using GZipTest.Concrete;

namespace GZipTest
{
    public class CompressorFactory
    {
        private readonly int _threadsCount = Environment.ProcessorCount;
        private readonly int _blockSize = 1024 * 1024;
        private readonly int _maxSize = 10000;    // todo define free memory and calculate maxSize
        private readonly int _initIdVal = 0;
        
        public ICommand CreateCompressor(string input, string output)
        {
            var reader = new RawBlockReader(new FileStream(input, FileMode.Open), _blockSize, _initIdVal);
            var writer = new CompressedBlockWriter(output);
            var compressor = new BlockCompressor();

            return Create(reader, writer, compressor);
        }

        public ICommand CreateDecompressor(string input, string output)
        {
            var reader = new CompressedBlockReader(new FileStream(input, FileMode.Open), _initIdVal);
            var writer = new RawBlockWriter(output);
            var decompressor = new BlockDecompressor();
            
            return Create(reader, writer, decompressor);
        }

        private ICommand Create(IBlockReader blockReader, IBlockWriter blockWriter, IBlockHandler handler)
        {
            var readSync = new BlockSynchronizer(_initIdVal, _maxSize);
            var writeSync = new BlockSynchronizer(_initIdVal, _maxSize);
            var reader = new ReaderCommand(blockReader, readSync);
            
            var worker = new WorkerCommand(readSync, writeSync, handler, _threadsCount);
            var writer = new WriterCommand(writeSync, blockWriter);
            var executor = new CommandsExecutor(new List<ICommand> { reader, worker, writer });

            return executor;
        }
    }
}
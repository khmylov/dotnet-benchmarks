using System.IO;
using System.IO.Compression;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    public class CompressionBench
    {
        private byte[] _inputData250K;
        private byte[] _deflate250K;
        private byte[] _gzip250K;
        private byte[] _inputData25K;
        private byte[] _deflate25K;
        private byte[] _gzip25K;

        [GlobalSetup]
        public void SetUp()
        {
            _inputData250K = File.ReadAllBytes("data/changed-resource-250K.json");
            _deflate250K = GetDeflateBytes(_inputData250K);
            _gzip250K = GetGzipBytes(_inputData250K);
            _inputData25K = File.ReadAllBytes("data/changed-resource-25K.json");
            _deflate25K = GetDeflateBytes(_inputData25K);
            _gzip25K = GetGzipBytes(_inputData25K);
        }

        /*
         * BenchmarkDotNet=v0.12.0, OS=Windows 10.0.19042
           Intel Core i7-8700 CPU 3.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
             [Host]     : .NET Framework 4.8 (4.8.4300.0), X86 LegacyJIT
             DefaultJob : .NET Framework 4.8 (4.8.4300.0), X86 LegacyJIT


           |                Method |         Mean |      Error |     StdDev |
           |---------------------- |-------------:|-----------:|-----------:|
           |   CompressDeflate250K | 3,769.702 us | 21.6736 us | 19.2131 us |
           |    CompressDeflate25K |   285.673 us |  0.6066 us |  0.5377 us |
           | DecompressDeflate250K |   732.009 us |  3.7550 us |  3.5125 us |
           |  DecompressDeflate25K |     3.529 us |  0.0349 us |  0.0326 us |
         */

        [Benchmark]
        public void CompressDeflate250KOptimal()
        {
            CompressViaDeflate(_inputData250K, CompressionLevel.Optimal);
        }

        [Benchmark]
        public void CompressDeflate25KOptimal()
        {
            CompressViaDeflate(_inputData25K, CompressionLevel.Optimal);
        }

        [Benchmark]
        public void CompressDeflate250KFastest()
        {
            CompressViaDeflate(_inputData250K, CompressionLevel.Fastest);
        }

        [Benchmark]
        public void CompressDeflate25KFastest()
        {
            CompressViaDeflate(_inputData25K, CompressionLevel.Fastest);
        }

        [Benchmark]
        public void CompressGzip250KOptimal()
        {
            CompressViaGzip(_inputData250K, CompressionLevel.Optimal);
        }

        [Benchmark]
        public void CompressGzip25KOptimal()
        {
            CompressViaGzip(_inputData25K, CompressionLevel.Optimal);
        }

        [Benchmark]
        public void CompressViaGzip250KFastest()
        {
            CompressViaGzip(_inputData250K, CompressionLevel.Fastest);
        }

        [Benchmark]
        public void CompressViaGzip25KFastest()
        {
            CompressViaGzip(_inputData25K, CompressionLevel.Fastest);
        }


        [Benchmark]
        public void DecompressDeflate250K()
        {
            DecompressViaDeflate(_deflate250K);
        }

        [Benchmark]
        public void DecompressDeflate25K()
        {
            DecompressViaDeflate(_deflate25K);
        }

        [Benchmark]
        public void DecompressGzip250K()
        {
            DecompressViaGzip(_gzip250K);
        }

        [Benchmark]
        public void DecompressGzip25K()
        {
            DecompressViaGzip(_gzip25K);
        }

        private static void CompressViaDeflate(byte[] input, CompressionLevel level)
        {
            using var compressedStream = new MemoryStream();
            using var compressionStream = new DeflateStream(compressedStream, level);
            compressionStream.Write(input, 0, input.Length);
        }

        private static void CompressViaGzip(byte[] input, CompressionLevel level)
        {
            using var compressedStream = new MemoryStream();
            using var compressionStream = new GZipStream(compressedStream, level);
            compressionStream.Write(input, 0, input.Length);
        }

        private static void DecompressViaDeflate(byte[] compressed)
        {
            using var inputStream = new MemoryStream(compressed);
            using var outputStream = new MemoryStream();
            using var decompressionStream = new DeflateStream(inputStream, CompressionMode.Decompress);
            decompressionStream.CopyTo(outputStream);
        }

        private static void DecompressViaGzip(byte[] compressed)
        {
            using var inputStream = new MemoryStream(compressed);
            using var outputStream = new MemoryStream();
            using var decompressionStream = new GZipStream(inputStream, CompressionMode.Decompress);
            decompressionStream.CopyTo(outputStream);
        }

        private static byte[] GetDeflateBytes(byte[] input)
        {
            using var compressedStream = new MemoryStream();
            using var compressionStream = new DeflateStream(compressedStream, CompressionLevel.Optimal);
            compressionStream.Write(input, 0, input.Length);
            return compressedStream.ToArray();
        }

        private static byte[] GetGzipBytes(byte[] input)
        {
            using var compressedStream = new MemoryStream();
            using var compressionStream = new GZipStream(compressedStream, CompressionLevel.Optimal);
            compressionStream.Write(input, 0, input.Length);
            return compressedStream.ToArray();
        }
    }
}

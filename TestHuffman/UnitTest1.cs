using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO.Compression;
using System.IO;
using System.Linq;
using HuffmanLib;

namespace TestHuffman
{
    [TestClass]
    public class UnitTest1
    {
        private byte[] TestArray
        {
            get
            {
                const int c_size = 4 * 1024 * 1024;
                const int c_fragment_size = 64;

                byte[] arr = new byte[c_size];
                Random rnd = new Random();
                byte[] fragment = new byte[c_fragment_size];
                rnd.NextBytes(fragment);

                for (int i = 0; i < c_size; i++)
                {
                    arr[i] = fragment[i % c_fragment_size];
                }

                return arr;
            }
        }

        [TestMethod]
        public void SingleHuffman()
        {
            byte[] testArray = TestArray;
            byte[] encodeArray;

            Huffman.Encode(ref testArray, out encodeArray);

            byte[] decodeArray;

            Huffman.Decode(ref encodeArray, out decodeArray);

            Assert.IsTrue(testArray.Length == decodeArray.Length);

            bool check = true;

            for (int i = 0; i < testArray.Length; i++)
            {
                if (testArray[i] != decodeArray[i])
                {
                    check = false;
                    break;
                }
            }

            Assert.IsTrue(check);
        }

        [TestMethod]
        public void MultiThreadHuffman()
        {
            MultiThreadHuffman mh = new MultiThreadHuffman();

            byte[] testArray = TestArray;
            byte[] encodeArray;

            mh.Encode(ref testArray, out encodeArray);

            byte[] decodeArray;

            mh.Decode(ref encodeArray, out decodeArray);

            Assert.IsTrue(testArray.Length == decodeArray.Length);

            bool check = true;

            for (int i = 0; i < testArray.Length; i++)
            {
                if (testArray[i] != decodeArray[i])
                {
                    check = false;
                    break;
                }
            }

            Assert.IsTrue(check);
        }

        [TestMethod]
        public void TestZipStream()
        {
            byte[] testAr = TestArray;

            // compress
            MemoryStream ms = new MemoryStream(testAr);
            MemoryStream outStream = new MemoryStream();

            using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress))
            {
                ms.CopyTo(zipStream);
            }

            byte[] bOut = outStream.ToArray();

            ms.Close();
            outStream.Close();

            // decompress
            GZipStream bigStream = new GZipStream(new MemoryStream(bOut), CompressionMode.Decompress);
            MemoryStream bigStreamOut = new MemoryStream();
            bigStream.CopyTo(bigStreamOut);

            byte[] decodeAr = bigStreamOut.ToArray();
            var dif = testAr.Zip(decodeAr, (a, b) => a - b);

            Assert.IsFalse(dif.Any(x => x != 0));
        }
    }
}

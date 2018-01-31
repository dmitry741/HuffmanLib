using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                const int c_size = 1 * 1024 * 1024;
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
    }
}

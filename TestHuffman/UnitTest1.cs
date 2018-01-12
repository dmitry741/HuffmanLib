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
                const int c_size = 1 * 1024 * 1024; // 4 Mb
                byte[] arr = new byte[c_size];

                for (int i = 0; i < c_size; i++)
                {
                    arr[i] = Convert.ToByte(i % 32);
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

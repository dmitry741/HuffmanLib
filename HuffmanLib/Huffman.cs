using System;
using System.Collections.Generic;

namespace HuffmanLib
{
    public class Huffman
    {
        static void GetHistogram(ref byte[] pBufIn, int lenIn, out HistogramNode[] pHN, out int lenOut)
        {
            int i;
            HistogramNode[] pHFull = new HistogramNode[256];

            for (i = 0; i < 256; i++)
            {
                pHFull[i] = new HistogramNode(Convert.ToByte(i));
            }

            for (i = 0; i < lenIn; i++)
            {
                pHFull[pBufIn[i]].Inc();
            }

            lenOut = 0;

            for (i = 0; i < 256; i++)
            {
                if (pHFull[i].Frequency > 0)
                {
                    lenOut++;
                }
            }

            pHN = new HistogramNode[lenOut];

            for (i = 0; i < lenOut; i++)
            {
                pHN[i] = new HistogramNode();
            }

            int index = -1;

            for (i = 0; i < 256; i++)
            {
                if (pHFull[i].Frequency > 0)
                {
                    index++;
                    pHN[index].Value = pHFull[i].Value;
                    pHN[index].Frequency = pHFull[i].Frequency;
                }
            }
        }

        static void Store(ref byte[] pBufIn, int lenIn, out byte[] pBufOut, out int lenOut)
        {
            lenOut = 0;

            lenOut++; // compress or uncompress
            lenOut += sizeof(int); // uncompressed size
            lenOut += lenIn; // uncompressed data

            pBufOut = new byte[lenOut];
            int offset = 0;

            pBufOut[0] = 0; offset++;
            byte[] ar = BitConverter.GetBytes(lenIn);
            ar.CopyTo(pBufOut, offset); offset += sizeof(int);

            pBufIn.CopyTo(pBufOut, offset);
        }

        static void EncodeArray(ref byte[] pBufIn, out byte[] pBufOut)
        {
            int lenIn = pBufIn.Length;
            int lenOut = 0;

            if (lenIn <= 1024)
            {
                Store(ref pBufIn, lenIn, out pBufOut, out lenOut);
                return;
            }

            HistogramNode[] pHN = null;
            int lHOut;
            int offset = 0;
            int nHuffmanTableSize;

            // 1 === create histogram ===
            GetHistogram(ref pBufIn, lenIn, out pHN, out lHOut);
            // ==========================

            // 1.1 === there is only 1 (one) value ===
            if (lHOut == 1)
            {
                lenOut = 0;

                lenOut++; // compress or uncompress
                lenOut += sizeof(int); // lenght of huffman table
                lenOut += sizeof(int); // uncompressed size
                lenOut++; // value

                pBufOut = new byte[lenOut];
                byte Value = pHN[0].Value;

                nHuffmanTableSize = 1;

                pBufOut[0] = 1; offset++;
                byte[] ar1 = BitConverter.GetBytes(nHuffmanTableSize);
                byte[] ar2 = BitConverter.GetBytes(lenIn);

                ar1.CopyTo(pBufOut, offset); offset += sizeof(int);
                ar2.CopyTo(pBufOut, offset); offset += sizeof(int);

                pBufOut[offset] = Value;

                return;
            }
            // ========================================

            // 2 === create zero level of tree ===
            List<CHNode> arCHZeroLevel = new List<CHNode>();
            int i;

            for (i = 0; i < lHOut; i++)
            {
                arCHZeroLevel.Add(new CHNode(pHN[i].Value, pHN[i].Frequency));
            }
            // ===================================

            // 3 === create tree ===
            List<CHNode> arCHTree = new List<CHNode>();
            int lCur = lHOut;
            int nTreeSize;

            for (i = 0; i < lHOut; i++)
            {
                arCHTree.Add(arCHZeroLevel[i]);
            }

            while (lCur > 1)
            {
                int index1 = 0;
                int index2 = 0;
                CHNode pHuffman;
                int min = 0xFFFFFFF;
                nTreeSize = arCHTree.Count;

                for (i = 0; i < nTreeSize; i++)
                {
                    pHuffman = arCHTree[i];

                    if (pHuffman.Mark)
                        continue;

                    if (pHuffman.Weight < min)
                    {
                        index1 = i;
                        min = pHuffman.Weight;
                    }
                }

                min = 0xFFFFFFF;

                for (i = 0; i < nTreeSize; i++)
                {
                    pHuffman = arCHTree[i];

                    if (pHuffman.Mark)
                        continue;

                    if (i == index1)
                        continue;

                    if (pHuffman.Weight < min)
                    {
                        index2 = i;
                        min = pHuffman.Weight;
                    }
                }

                CHNode p1 = arCHTree[index1];
                CHNode p2 = arCHTree[index2];
                CHNode pNewNode = new CHNode();
                CHNode emptyNode = null;

                pNewNode.Weight = p1.Weight + p2.Weight;
                pNewNode.SetLeft(ref p1);
                pNewNode.SetRight(ref p2);
                pNewNode.SetRoot(ref emptyNode);

                arCHTree.Add(pNewNode);

                p1.SetRoot(ref pNewNode);
                p1.SetLeftType();
                p1.Mark = true;

                p2.SetRoot(ref pNewNode);
                p2.SetRightType();
                p2.Mark = true;

                lCur--;
            }
            // =====================================

            // 4 === compute size for compressed data ===
            int nCompressedDataSize = 0;
            nHuffmanTableSize = 0;
            HuffmanTableNode[] pHuffmanNode = new HuffmanTableNode[arCHZeroLevel.Count];

            for (i = 0; i < arCHZeroLevel.Count; i++)
            {
                pHuffmanNode[i] = new HuffmanTableNode();
            }

            for (i = 0; i < arCHZeroLevel.Count; i++)
            {
                CHNode pNode = arCHZeroLevel[i];

                while (pNode.GetRoot() != null)
                {
                    if (pNode.IsRightType)
                    {
                        pHuffmanNode[i].AddBitOne(); // 1
                    }
                    else
                    {
                        pHuffmanNode[i].AddBitZero(); // 0
                    }

                    pNode = pNode.GetRoot();
                }

                pNode = arCHZeroLevel[i];
                pHuffmanNode[i].Value = pNode.Value;
                pHuffmanNode[i].Invert();

                nCompressedDataSize += (pHuffmanNode[i].Len * pNode.Weight); // measured in bits
            }

            if (nCompressedDataSize % 8 == 0)
            {
                nCompressedDataSize /= 8; // measured in bytes
            }
            else
            {
                nCompressedDataSize = nCompressedDataSize / 8 + 1; // measured in bytes
            }
            // ==========================================

            // 5 === compute total memory ===
            lenOut = 0;

            lenOut++; // compress or uncompress
            lenOut += sizeof(int); // lenght of huffman table
            lenOut += lHOut * HistogramNode.Size; // histogram size
            lenOut += sizeof(int); // uncompressed size
            lenOut += nCompressedDataSize;
            // ==============================

            // 6 === Check for total size ===
            if (lenOut > lenIn)
            {
                Store(ref pBufIn, lenIn, out pBufOut, out lenOut);
                return;
            }
            // ================================

            // 7 === Encoding ===
            pBufOut = new byte[lenOut];
            byte[] p = pBufOut;
            Array.Clear(p, 0, p.Length);

            offset = 0;
            int index, j;
            int nBits = 8;
            byte Code, CodeForWriting;
            int CodeLen;
            int nZeroLevel = arCHZeroLevel.Count;
            byte[] arInt = null;

            p[0] = 1; offset++;
            arInt = BitConverter.GetBytes(nZeroLevel);
            arInt.CopyTo(p, offset); offset += sizeof(int);

            // === store histogram ===
            for (i = 0; i < nZeroLevel; i++)
            {
                offset = pHN[i].ToArray(ref p, offset);
            }
            // ======================

            arInt = BitConverter.GetBytes(lenIn);
            arInt.CopyTo(p, offset);
            offset += sizeof(int);

            byte[] pIndex = new byte[256];

            for (i = 0; i < 256; i++)
            {
                for (j = 0; j < nZeroLevel; j++)
                {
                    if (i == pHuffmanNode[j].Value)
                    {
                        pIndex[i] = Convert.ToByte(j);
                        break;
                    }
                }
            }

            for (i = 0; i < lenIn; i++)
            {
                index = pIndex[pBufIn[i]];
                CodeLen = pHuffmanNode[index].Len;

                for (j = 0; j < CodeLen; j++)
                {
                    if (nBits == 0)
                    {
                        nBits = 8;
                        offset++;
                    }

                    Code = pHuffmanNode[index].GetBit(j);
                    CodeForWriting = Convert.ToByte(Code << (nBits - 1));
                    p[offset] |= CodeForWriting;

                    nBits--;
                }
            }
        }

        static void DecodeArray(ref byte[] pBufIn, out byte[] pBufOut)
        {
            int lenIn = pBufIn.Length;
            int lenOut = 0;
            int offset = 1;

            // 0 === check for compressed or uncompressed ===
            if (pBufIn[0] == 0)
            {
                lenOut = BitConverter.ToInt32(pBufIn, offset); offset += sizeof(int);
                pBufOut = new byte[lenOut];

                for (int k = 0; k < lenOut; k++)
                {
                    pBufOut[k] = pBufIn[k + offset];
                }

                return;
            }
            // ==============================================

            // 1 === Get Huffman table ===
            int i;
            int nHaffmanTableSize = BitConverter.ToInt32(pBufIn, offset);
            offset += sizeof(int);

            if (nHaffmanTableSize == 1)
            {
                int uncompressed = BitConverter.ToInt32(pBufIn, offset);
                offset += sizeof(int);

                byte Value = pBufIn[offset];

                lenOut = uncompressed;
                pBufOut = new byte[lenOut];

                for (i = 0; i < lenOut; i++)
                {
                    pBufOut[i] = Value;
                }

                return;
            }

            // === extract histogram ===
            HistogramNode[] pHN = new HistogramNode[nHaffmanTableSize];

            for (i = 0; i < nHaffmanTableSize; i++)
            {
                pHN[i] = new HistogramNode();
                offset = pHN[i].FromArray(ref pBufIn, offset);
            }
            // =========================

            // 2 === create zero level of tree ===
            List<CHNode> arCHZeroLevel = new List<CHNode>();
            int lHOut = nHaffmanTableSize;

            for (i = 0; i < lHOut; i++)
            {
                arCHZeroLevel.Add(new CHNode(pHN[i].Value, pHN[i].Frequency));
            }
            // ===================================

            // 3 === create tree ===
            List<CHNode> arCHTree = new List<CHNode>();
            int lCur = lHOut;
            int nTreeSize;
            CHNode pRoot = null;

            for (i = 0; i < lHOut; i++)
            {
                arCHTree.Add(arCHZeroLevel[i]);
            }

            while (lCur > 1)
            {
                int index1 = 0;
                int index2 = 0;

                CHNode emptyNode = null;
                CHNode pHaffman = null;
                int min = 0xFFFFFFF;
                nTreeSize = arCHTree.Count;

                for (i = 0; i < nTreeSize; i++)
                {
                    pHaffman = arCHTree[i];

                    if (pHaffman.Mark)
                        continue;

                    if (pHaffman.Weight < min)
                    {
                        index1 = i;
                        min = pHaffman.Weight;
                    }
                }

                min = 0xFFFFFFF;

                for (i = 0; i < nTreeSize; i++)
                {
                    pHaffman = arCHTree[i];

                    if (pHaffman.Mark)
                        continue;

                    if (i == index1)
                        continue;

                    if (pHaffman.Weight < min)
                    {
                        index2 = i;
                        min = pHaffman.Weight;
                    }
                }

                CHNode p1 = arCHTree[index1];
                CHNode p2 = arCHTree[index2];
                CHNode pNewNode = new CHNode();

                pNewNode.Weight = p1.Weight + p2.Weight;
                pNewNode.SetLeft(ref p1);
                pNewNode.SetRight(ref p2);
                pNewNode.SetRoot(ref emptyNode);
                pRoot = pNewNode;

                arCHTree.Add(pNewNode);

                p1.SetRoot(ref pNewNode);
                p1.SetLeftType();
                p1.Mark = true;

                p2.SetRoot(ref pNewNode);
                p2.SetRightType();
                p2.Mark = true;

                lCur--;
            }

            // === end: tree from encode ====

            // 2 == get output size ===
            lenOut = BitConverter.ToInt32(pBufIn, offset); offset += sizeof(int);
            pBufOut = new byte[lenOut];
            Array.Clear(pBufOut, 0, lenOut);
            // ========================

            // 3 === decoding ===
            int offsetOut = 0;
            int nBits = 8;
            byte Digit;
            CHNode pHaffman1;

            while (offsetOut < lenOut)
            {
                pHaffman1 = pRoot;

                while (true)
                {
                    Digit = Convert.ToByte(pBufIn[offset] & (1 << (nBits - 1)));
                    Digit = Convert.ToByte(Digit >> (nBits - 1));
                    nBits--;

                    // === fast search huffman table index ===
                    pHaffman1 = (Digit == 0) ? pHaffman1.GetLeft() : pHaffman1.GetRight();
                    // =======================================

                    if (nBits == 0)
                    {
                        offset++;
                        nBits = 8;
                    }

                    if (pHaffman1.GetRight() == null)
                        break;
                }

                if (pHaffman1.GetRight() == null)
                {
                    pBufOut[offsetOut++] = pHaffman1.Value;
                }
                else
                {
                    break;
                }
            }
        }

        static public void Encode(ref byte[] pBufIn, out byte[] pBufOut)
        {
            byte[] pOut = null;
            byte[] pIn = pBufIn;
            byte counter = 0;

            while (true)
            {
                counter++;
                EncodeArray(ref pIn, out pOut);

                if (pIn.Length < pOut.Length)
                {
                    pBufOut = new byte[pOut.Length + 1];
                    pBufOut[0] = counter;

                    pOut.CopyTo(pBufOut, 1);
                    break;
                }

                pIn = new byte[pOut.Length];
                pOut.CopyTo(pIn, 0);
            }
        }

        static public void Decode(ref byte[] pBufIn, out byte[] pBufOut)
        {
            int counter = pBufIn[0];
            byte[] pOut = null;
            byte[] pIn = null;
            int i;

            pIn = new byte[pBufIn.Length - 1];
            Array.Copy(pBufIn, 1, pIn, 0, pIn.Length);

            for (i = 0; i < counter; i++)
            {
                DecodeArray(ref pIn, out pOut);

                pIn = new byte[pOut.Length];
                pOut.CopyTo(pIn, 0);
                pOut = null;
            }

            pBufOut = pIn;
        }
    }
}

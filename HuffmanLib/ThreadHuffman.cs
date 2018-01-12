using System;

namespace HuffmanLib
{
    public class MultiThreadHuffman
    {
        int m_ProcessCount = Environment.ProcessorCount;
        System.Threading.Thread[] m_thread = null;
        ThreadRoutine[] m_threadRoutine = null;

        void Start()
        {
            m_thread = new System.Threading.Thread[m_ProcessCount];
            m_threadRoutine = new ThreadRoutine[m_ProcessCount];

            for (int i = 0; i < m_ProcessCount; i++)
            {
                m_threadRoutine[i] = new ThreadRoutine
                {
                    m_Command = 0
                };

                System.Threading.ThreadStart threadDelegate = new System.Threading.ThreadStart(m_threadRoutine[i].DoWork);
                m_thread[i] = new System.Threading.Thread(threadDelegate);

                m_thread[i].Start();
            }
        }

        void Stop()
        {
            for (int i = 0; i < m_ProcessCount; i++)
            {
                m_threadRoutine[i].m_Command = -1;
                m_threadRoutine[i].ewhStart.Set();
            }
        }

        public void Encode(ref byte[] pBufIn, out byte[] pBufOut)
        {
            if (pBufIn.Length < 1024 * 1024)
            {
                byte[] pOut = null;
                Huffman.Encode(ref pBufIn, out pOut);

                pBufOut = new byte[pOut.Length + 1];
                pBufOut[0] = 0;
                pOut.CopyTo(pBufOut, 1);

                return;
            }

            // === start threads ===
            Start();
            // =====================

            pBufOut = null;

            int i, j;
            int PieSize = pBufIn.Length / m_ProcessCount;
            int StartIndex = 0;
            int EndIndex = 0;
            byte[][] aIn = new byte[m_ProcessCount][];

            for (i = 0; i < m_ProcessCount; i++)
            {
                EndIndex = StartIndex + PieSize;

                if (EndIndex >= pBufIn.Length)
                {
                    EndIndex = pBufIn.Length - 1;
                }

                aIn[i] = new byte[EndIndex - StartIndex + 1];

                for (j = 0; j < aIn[i].Length; j++)
                {
                    aIn[i][j] = pBufIn[j + StartIndex];
                }

                StartIndex = EndIndex + 1;
            }

            for (i = 0; i < m_ProcessCount; i++)
            {
                m_threadRoutine[i].ArrayIn = aIn[i];
                m_threadRoutine[i].m_Command = 0;
                m_threadRoutine[i].ewhStart.Set();
            }

            int outSize = 0;

            for (i = 0; i < m_ProcessCount; i++)
            {
                m_threadRoutine[i].ewhStop.WaitOne();
                outSize += m_threadRoutine[i].ArrayOut.Length;
                outSize += sizeof(int);
            }

            outSize++;

            pBufOut = new byte[outSize];
            pBufOut[0] = Convert.ToByte(m_ProcessCount);
            StartIndex = 1;

            for (i = 0; i < m_ProcessCount; i++)
            {
                byte[] aOut = m_threadRoutine[i].ArrayOut;

                byte[] aSize = BitConverter.GetBytes(aOut.Length);

                for (j = 0; j < aSize.Length; j++)
                {
                    pBufOut[j + StartIndex] = aSize[j];
                }

                StartIndex += aSize.Length;

                for (j = 0; j < aOut.Length; j++)
                {
                    pBufOut[j + StartIndex] = aOut[j];
                }

                StartIndex += aOut.Length;
            }

            // === stop threads ===
            Stop();
            // ====================
        }

        public void Decode(ref byte[] pBufIn, out byte[] pBufOut)
        {
            int i;
            pBufOut = null;

            if (pBufIn[0] == 0)
            {
                byte[] aIn = new byte[pBufIn.Length - 1];

                for (i = 0; i < aIn.Length; i++)
                {
                    aIn[i] = pBufIn[i + 1];
                }

                Huffman.Decode(ref aIn, out pBufOut);

                return;
            }

            // === start threads ===
            Start();
            // =====================

            int j;
            int StartIndex = 1;

            for (i = 0; i < m_ProcessCount; i++)
            {
                int size = BitConverter.ToInt32(pBufIn, StartIndex);
                StartIndex += sizeof(int);

                byte[] aIn = new byte[size];

                for (j = 0; j < aIn.Length; j++)
                {
                    aIn[j] = pBufIn[j + StartIndex];
                }

                StartIndex += size;

                m_threadRoutine[i].ArrayIn = aIn;
            }

            for (i = 0; i < m_ProcessCount; i++)
            {
                m_threadRoutine[i].m_Command = 1;
                m_threadRoutine[i].ewhStart.Set();
            }

            int outSize = 0;

            for (i = 0; i < m_ProcessCount; i++)
            {
                m_threadRoutine[i].ewhStop.WaitOne();
                outSize += m_threadRoutine[i].ArrayOut.Length;
            }

            pBufOut = new byte[outSize];
            StartIndex = 0;

            for (i = 0; i < m_ProcessCount; i++)
            {
                byte[] aOut = m_threadRoutine[i].ArrayOut;

                for (j = 0; j < aOut.Length; j++)
                {
                    pBufOut[j + StartIndex] = aOut[j];
                }

                StartIndex += aOut.Length;
            }

            // === stop threads ===
            Stop();
            // ====================
        }
    }
}

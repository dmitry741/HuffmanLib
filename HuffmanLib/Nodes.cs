using System;
using System.Collections.Generic;

namespace HuffmanLib
{
    class BaseNode
    {
        protected byte m_Value;

        public BaseNode()
        {
            m_Value = 0;
        }

        public byte Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
    }

    class HistogramNode : BaseNode
    {
        int m_Frequency;

        public HistogramNode()
        {
            m_Frequency = 0;
        }

        public HistogramNode(byte val)
        {
            m_Frequency = 0;
            m_Value = val;
        }

        public void Inc()
        {
            m_Frequency++;
        }

        public int Frequency
        {
            get { return m_Frequency; }
            set { m_Frequency = value; }
        }

        public int GetSize()
        {
            int size = 0;

            size += sizeof(int); // m_Frequency
            size++; // m_Value

            return size;
        }

        public int ToArray(ref byte[] pBuf, int offset)
        {
            byte[] arFrequency = BitConverter.GetBytes(m_Frequency);

            pBuf[offset++] = m_Value;
            arFrequency.CopyTo(pBuf, offset); offset += sizeof(int);

            return offset;
        }

        public int FromArray(ref byte[] pBuf, int offset)
        {
            m_Value = pBuf[offset++];
            m_Frequency = BitConverter.ToInt32(pBuf, offset); offset += sizeof(int);

            return offset;
        }
    }

    class CHNode : BaseNode
    {
        int m_weight;
        CHNode m_pLeft;
        CHNode m_pRight;
        CHNode m_pRoot;
        int m_Type;
        bool m_bMark;

        public CHNode()
        {
            m_weight = 0;
            m_Type = -1;
            m_pLeft = m_pRight = m_pRoot = null;
            m_bMark = false;
        }

        public CHNode(byte Value, int weight)
        {
            m_weight = weight;
            m_Value = Value;
            m_Type = -1;
            m_pLeft = m_pRight = m_pRoot = null;
            m_bMark = false;
        }

        public int Weight
        {
            get { return m_weight; }
            set { m_weight = value; }
        }

        public CHNode GetLeft()
        {
            return m_pLeft;
        }

        public CHNode GetRight()
        {
            return m_pRight;
        }

        public CHNode GetRoot()
        {
            return m_pRoot;
        }

        public void SetLeft(ref CHNode p)
        {
            m_pLeft = p;
        }

        public void SetRight(ref CHNode p)
        {
            m_pRight = p;
        }

        public void SetRoot(ref CHNode p)
        {
            m_pRoot = p;
        }

        public void SetLeftType()
        {
            m_Type = 0;
        }

        public void SetRightType()
        {
            m_Type = 1;
        }

        public bool IsRightType => (m_Type == 1);

        public void SetMark()
        {
            m_bMark = true;
        }

        public bool GetMark()
        {
            return m_bMark;
        }
    }

    class HuffmanTableNode : BaseNode
    {
        byte[] m_Code; // Haffman code
        byte m_Size; // length in bytes
        byte m_len; // length in bits
        byte[] m_mask;

        const int c_codeSize = 32;

        public HuffmanTableNode()
        {
            m_Code = new byte[c_codeSize];
            m_mask = new byte[8];

            Empty();

            m_mask[0] = 1;

            for (int i = 1; i < 8; i++)
            {
                m_mask[i] = Convert.ToByte(m_mask[i - 1] << 1);
            }
        }

        public void Empty()
        {
            m_len = m_Size = 0;
            Array.Clear(m_Code, 0, m_Code.Length);
        }

        public void Invert()
        {
            byte Bit1, Bit2;

            for (int i = 0; i < m_len / 2; i++)
            {
                Bit1 = m_Code[i / 8];
                Bit1 = Convert.ToByte(Bit1 & m_mask[i % 8]);
                Bit1 = Convert.ToByte(Bit1 >> (i % 8));

                Bit2 = m_Code[(m_len - i - 1) / 8];
                Bit2 = Convert.ToByte(Bit2 & m_mask[(m_len - i - 1) % 8]);
                Bit2 = Convert.ToByte(Bit2 >> ((m_len - i - 1) % 8));

                if (Bit2 == 0)
                {
                    m_Code[i / 8] &= Convert.ToByte(0xFF & (~m_mask[i % 8]));
                }
                else
                {
                    m_Code[i / 8] |= m_mask[i % 8];
                }

                if (Bit1 == 0)
                {
                    m_Code[(m_len - i - 1) / 8] &= Convert.ToByte(0xFF & (~m_mask[(m_len - i - 1) % 8]));
                }
                else
                {
                    m_Code[(m_len - i - 1) / 8] |= m_mask[(m_len - i - 1) % 8];
                }
            }
        }

        public void AddBitOne()
        {
            if (m_len % 8 == 0)
            {
                m_Size++;
                m_Code[m_Size - 1] = 1;
            }
            else
            {
                m_Code[m_Size - 1] |= m_mask[m_len % 8];
            }

            m_len++;
        }

        public void AddBitZero()
        {
            if (m_len % 8 == 0)
            {
                m_Size++;
                m_Code[m_Size - 1] = 0;
            }

            m_len++;
        }

        public byte GetBit(int Pos)
        {
            byte Code = m_Code[Pos / 8];
            Code = Convert.ToByte(Code & m_mask[Pos % 8]);
            Code = Convert.ToByte(Code >> (Pos % 8));

            return Code;
        }

        public byte Len => m_len;

        static public bool operator ==(HuffmanTableNode Node1, HuffmanTableNode Node2)
        {
            if (Node1.m_len != Node2.m_len)
                return false;

            bool result = true;

            for (int i = 0; i < Node1.m_Size; i++)
            {
                if (Node1.m_Code[i] != Node2.m_Code[i])
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        static public bool operator !=(HuffmanTableNode Node1, HuffmanTableNode Node2)
        {
            bool result = (Node1 == Node2);

            return (!result);
        }

        public override bool Equals(object ob)
        {
            if (!(ob is HuffmanTableNode))
            {
                return false;
            }

            HuffmanTableNode Node2 = ob as HuffmanTableNode;

            if (m_len != Node2.m_len)
                return false;

            bool result = true;

            for (int i = 0; i < m_Size; i++)
            {
                if (m_Code[i] != Node2.m_Code[i])
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public override int GetHashCode()
        {
            var hashCode = 134578053;

            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(m_Code);
            hashCode = hashCode * -1521134295 + m_Size.GetHashCode();
            hashCode = hashCode * -1521134295 + m_len.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(m_mask);

            return hashCode;
        }
    }

    //public interface IHuffman
    //{
    //    void Encode(ref byte[] pBufIn, out byte[] pBufOut);

    //    void Decode(ref byte[] pBufIn, out byte[] pBufOut);
    //}
}

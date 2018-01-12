namespace HuffmanLib
{
    class ThreadRoutine
    {
        public int m_Command = -1;
        byte[] m_aIn = null;
        byte[] m_aOut = null;

        // === sync ===
        public System.Threading.EventWaitHandle ewhStart = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset);
        public System.Threading.EventWaitHandle ewhStop = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset);
        // ============

        public byte[] ArrayIn
        {
            get { return m_aIn; }
            set { m_aIn = value; }
        }

        public byte[] ArrayOut
        {
            get { return m_aOut; }
            set { m_aOut = value; }
        }

        public void DoWork()
        {
            while (true)
            {
                ewhStart.WaitOne();

                if (m_Command == 0) // encode
                {
                    Huffman.Encode(ref m_aIn, out m_aOut);
                }
                else if (m_Command == 1) // decode
                {
                    Huffman.Decode(ref m_aIn, out m_aOut);
                }
                else
                {
                    ewhStop.Set();
                    break;
                }

                ewhStop.Set();
            }
        }
    }
}

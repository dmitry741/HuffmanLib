using System;
using System.Windows.Forms;
using HuffmanLib;

// develpoped by Dmitry Pavlov 2018 email: dmitrypavlov74@gmail.com

namespace SamleHuffman
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        byte[] TestArray
        {
            get
            {
                const int c_size = 2 * 1024 * 1024;
                int fragment_size = (int)cmbChars.SelectedItem;
                byte[] arr = new byte[c_size];

                for (int i = 0; i < c_size; i++)
                {
                    arr[i] = Convert.ToByte(i % fragment_size);
                }

                return arr;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MultiThreadHuffman mh = new MultiThreadHuffman();

            byte[] testArray = TestArray;
            byte[] encodeArray;

            DateTime dtStart = DateTime.Now;

            // encode
            mh.Encode(ref testArray, out encodeArray);

            byte[] decodeArray;

            // decode
            mh.Decode(ref encodeArray, out decodeArray);

            TimeSpan ts = DateTime.Now - dtStart;

            lblSourceArray.Text = string.Format("{0} Kb", testArray.Length / 1024);
            lblEncodeArray.Text = string.Format("{0} Kb", encodeArray.Length / 1024);

            double percent = 100 - 100.0 / testArray.Length * encodeArray.Length;
            lblPercent.Text = string.Format("{0}%", Convert.ToInt32(percent));

            lblTime.Text = string.Format("{0}:{1} s", ts.Seconds, ts.Milliseconds);
        }

        void Clearcontrols()
        {
            lblSourceArray.Text = string.Empty;
            lblEncodeArray.Text = string.Empty;
            lblPercent.Text = string.Empty;
            lblTime.Text = string.Empty;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Clearcontrols();

            int val = 2;

            while (val <= 128)
            {
                cmbChars.Items.Add(val);
                val *= 2;
            }

            cmbChars.SelectedIndex = 5;
        }
    }
}

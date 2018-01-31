using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HuffmanLib;

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
                const int c_size = 4 * 1024 * 1024;
                int fragment_size = (int)cmbChars.SelectedItem;

                byte[] arr = new byte[c_size];
                Random rnd = new Random();
                byte[] fragment = new byte[fragment_size];
                rnd.NextBytes(fragment);

                for (int i = 0; i < c_size; i++)
                {
                    arr[i] = fragment[i % fragment_size];
                }

                return arr;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MultiThreadHuffman mh = new MultiThreadHuffman();

            byte[] testArray = TestArray;
            byte[] encodeArray;

            mh.Encode(ref testArray, out encodeArray);

            byte[] decodeArray;

            mh.Decode(ref encodeArray, out decodeArray);

            bool check = true;

            for (int i = 0; i < testArray.Length; i++)
            {
                if (testArray[i] != decodeArray[i])
                {
                    check = false;
                    break;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblSourceArray.Text = lblEncodeArray.Text = string.Empty;

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

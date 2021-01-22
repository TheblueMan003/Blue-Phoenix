using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSharp
{
    public partial class CompileOrder : Form
    {
        public ListBox.ObjectCollection Content;
        public int fixedFile=1;
        public CompileOrder(ListBox.ObjectCollection order, int fixedFile)
        {
            InitializeComponent();
            listBox1.Items.AddRange(order);
            this.fixedFile = fixedFile;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                object a = listBox1.SelectedItem;
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.Insert(fixedFile, a);
                listBox1.SelectedIndex = fixedFile;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= fixedFile)
            {
                object a = listBox1.SelectedItem;
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.Add(a);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= fixedFile)
            {
                object a = listBox1.SelectedItem;
                int i = listBox1.SelectedIndex;
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.Insert(Math.Max(i - 1, fixedFile-1), a);
                listBox1.SelectedIndex = Math.Max(i - 1, fixedFile-1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= fixedFile)
            {
                object a = listBox1.SelectedItem;
                int i = listBox1.SelectedIndex;
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.Insert(Math.Min(i + 1, listBox1.Items.Count), a);
                listBox1.SelectedIndex = Math.Min(i + 1, listBox1.Items.Count);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Content = listBox1.Items;
            Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= fixedFile)
            {
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }
    }
}

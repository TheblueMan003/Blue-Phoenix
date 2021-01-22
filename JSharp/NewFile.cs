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
    public partial class NewFile : Form
    {
        public string filename;
        public Type type;
        public Type[] typeList = new Type[] { Type.EMPTY, Type.STRUCTURE };
        public enum Type
        {
            EMPTY,
            STRUCTURE
        }

        public NewFile()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            filename = textBox1.Text;
            DialogResult = DialogResult.OK;
            if (listBox1.SelectedIndex > -1)
                type = typeList[listBox1.SelectedIndex];
            else
                type = Type.EMPTY;
            Close();
        }
    }
}

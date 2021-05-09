using System;
using System.Windows.Forms;

namespace BluePhoenix
{
    public partial class NewItem : Form
    {
        public string FileName;
        public NewItem()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileName = textBox1.Text;
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}

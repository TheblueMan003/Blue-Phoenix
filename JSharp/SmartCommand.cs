using System;
using System.Windows.Forms;

namespace JSharp
{
    public partial class SmartCommand : Form
    {
        public RichTextBox CodeBox;
        public SmartCommand(RichTextBox CodeBox)
        {
            InitializeComponent();
            this.CodeBox = CodeBox;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EffectForm form = new EffectForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                CodeBox.Text += form.Content;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GameruleForm form = new GameruleForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                CodeBox.Text += form.Content;
            }
        }
    }
}

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
    public partial class EffectForm : Form
    {
        public string Content;
        public EffectForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "effect(give, " + EntityTextbox.Text + ", " + EffectTextbox.Text + ", " + DurationTextbox.Text + ", "
                + AmplierTextbox.Text + ", " + (ParticuleTextbox.Checked ? "true" : "false") + ")"+'\n';
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "effect(clear, " + EntityTextbox.Text + ", " + EffectTextbox.Text + ")"+'\n';
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Content = richTextBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Content = "";
            DialogResult = DialogResult.Abort;
            Close();
        }

        private void EffectForm_Load(object sender, EventArgs e)
        {
            foreach(string effect in CommandParser.effects)
            {
                EffectTextbox.Items.Add(effect);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            Formatter.reformat(richTextBox1,this,false);
        }
    }
}

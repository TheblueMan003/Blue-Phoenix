using System;
using System.Windows.Forms;

namespace JSharp
{
    public partial class GameruleForm : Form
    {
        public string Content;

        public GameruleForm()
        {
            InitializeComponent();

            foreach (CommandParser.Gamerule gr in CommandParser.gamerulesObj)
            {
                GameruleTextbox.Items.Add(gr);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            Formatter.reformat(richTextBox1, this, false);
        }

        private void GameruleTextbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameruleTextbox.SelectedIndex > -1)
            {
                label2.Text = ((CommandParser.Gamerule)GameruleTextbox.SelectedItem).desc;
                ValueTextBox.Text = ((CommandParser.Gamerule)GameruleTextbox.SelectedItem).val;

                if (ValueTextBox.Text == "true")
                    ValueTextBox.Text = "false";
                else if (ValueTextBox.Text == "false")
                    ValueTextBox.Text = "true";
            }
            else
                label2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += "gamerule(" + GameruleTextbox.Text + ", " + ValueTextBox.Text + ")" + '\n';
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
    }
}

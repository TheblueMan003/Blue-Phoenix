using System;
using System.Windows.Forms;

namespace JSharp
{
    public partial class Find_and_Replace : Form
    {
        private RichTextBox Codebox;
        public Find_and_Replace(RichTextBox Codebox)
        {
            InitializeComponent();
            this.Codebox = Codebox;
        }

        private void Find_and_Replace_Load(object sender, EventArgs e)
        {

        }
        private void Find()
        {
            Codebox.Focus();

            if (Codebox.Text.IndexOf(textBox1.Text, Codebox.SelectionStart) > -1)
                Codebox.Select(Codebox.Text.IndexOf(textBox1.Text, Codebox.SelectionStart), textBox1.Text.Length);
            else if (Codebox.Text.IndexOf(textBox1.Text, 0) > -1)
                Codebox.Select(Codebox.Text.IndexOf(textBox1.Text, 0), textBox1.Text.Length);

            this.Focus();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Codebox.SelectedText == textBox1.Text)
            {
                int s = Codebox.SelectionStart;
                string a = Codebox.Text.Substring(0, Codebox.SelectionStart);
                string b = Codebox.Text.Substring(Codebox.SelectionStart + Codebox.SelectionLength, Codebox.Text.Length - (Codebox.SelectionStart + Codebox.SelectionLength));

                Codebox.Text = a + textBox2.Text + b;
                Formatter.reformat(Codebox, this, false);
                Find();

                Codebox.SelectionStart = s;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int s = Codebox.SelectionStart;
            Codebox.Text = Codebox.Text.Replace(textBox1.Text, textBox2.Text);
            Formatter.reformat(Codebox, this, false);
            Codebox.SelectionStart = s;
        }
    }
}

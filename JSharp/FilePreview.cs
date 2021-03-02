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
    public partial class FilePreview : Form
    {
        List<Compiler.File> files;
        public bool closed = false;

        public FilePreview(List<Compiler.File> files)
        {
            InitializeComponent();
            this.files = files;
            if (files.Count > 0)
            {
                foreach (var f in files)
                {
                    listBox1.Items.Add(f);
                }
                richTextBox1.Text = files[0].content;
                Text = "File Preview: " + files[0].name;
            }
            FormatterCommand.reformat(richTextBox1,this,false);
        }

        private void FilePreview_Load(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            FormatterCommand.reformat(richTextBox1, this, false);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                richTextBox1.Text = ((Compiler.File)listBox1.Items[listBox1.SelectedIndex]).content;
                Text = "File Preview: "+ ((Compiler.File)listBox1.Items[listBox1.SelectedIndex]).name;
            }
        }

        private void FilePreview_FormClosed(object sender, FormClosedEventArgs e)
        {
            closed = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            if (files.Count > 0)
            {
                foreach (var f in files)
                {
                    if (f.name.Contains(textBox1.Text))
                        listBox1.Items.Add(f);
                }
            }
        }
    }
}

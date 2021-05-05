using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSharp
{
    public partial class NewProjectForm : Form
    {
        public string ProjectName;

        public NewProjectForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProjectName = textBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void NewProjectForm_Load(object sender, EventArgs e)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
            if (File.Exists(path+"project.old"))
            {
                foreach (string s in File.ReadAllLines(path + "project.old")) listBox1.Items.Add(s);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProjectName = listBox1.SelectedItem.ToString();
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ProjectName = openFileDialog1.FileName;
                DialogResult = DialogResult.Yes;
                Close();
            }
            else
                return;
        }
    }
}

using System;
using System.IO;
using System.Windows.Forms;

namespace JSharp
{
    public partial class StructureImport : Form
    {
        public string projectDirectory;

        public StructureImport(string projectDirectory)
        {
            InitializeComponent();
            this.projectDirectory = projectDirectory + (projectDirectory.EndsWith("/") ? "structures/" : "/structures/");
            if (!Directory.Exists(this.projectDirectory))
                Directory.CreateDirectory(this.projectDirectory);
            Reload();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (StructureOpen.ShowDialog() == DialogResult.OK)
            {
                foreach (string source in StructureOpen.FileNames)
                {
                    string filename = Path.GetFileName(source);
                    if (File.Exists(projectDirectory + filename))
                        File.Delete(projectDirectory + filename);
                    File.Copy(source, projectDirectory + filename);
                    Reload();
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                File.Delete(projectDirectory + listBox1.SelectedItem.ToString());
                Reload();
            }
        }

        public void Reload()
        {
            listBox1.Items.Clear();
            string[] files = Directory.GetFiles(this.projectDirectory);
            foreach (string file in files)
            {
                listBox1.Items.Add(Path.GetFileName(file));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

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

namespace BluePhoenix
{
    public partial class LibImport : Form
    {
        public List<string> import = new List<string>();
        public LibImport(List<string> paths, List<string> imported = null)
        {
            InitializeComponent();
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
            if (imported != null)
            {
                imported.ForEach(x => ImportedLB.Items.Add(x));
            }
            else
            {
                ImportedLB.Items.Add("standard.java");
                ImportedLB.Items.Add("standard.entity_id");
                ImportedLB.Items.Add("standard.object");
            }
            foreach(string p in paths.Select(x => x.StartsWith("./") ? path + x.Replace("./", "") : x))
            {
                foreach (string lib in Directory.EnumerateFiles(p, "*.tbms", SearchOption.AllDirectories)){
                    var lib2 = lib.Replace(p, "").Replace("\\", ".").Replace("/", ".").Replace(".tbms", "");
                    if (!AvailableLB.Items.Contains(lib2) &&
                        !ImportedLB.Items.Contains(lib2))
                    {
                        AvailableLB.Items.Add(lib2);
                    }
                }
            }
            AvailableLB.Sorted = true;
        }

        private void AvailableLB_DoubleClick(object sender, EventArgs e)
        {
            if (AvailableLB.SelectedIndex >= 0)
            {
                ImportedLB.Items.Add(AvailableLB.Items[AvailableLB.SelectedIndex]);
                AvailableLB.Items.RemoveAt(AvailableLB.SelectedIndex);
            }
        }

        private void ImportedLB_DoubleClick(object sender, EventArgs e)
        {
            if (ImportedLB.SelectedIndex >= 0)
            {
                AvailableLB.Items.Add(ImportedLB.Items[ImportedLB.SelectedIndex]);
                ImportedLB.Items.RemoveAt(ImportedLB.SelectedIndex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (string s in ImportedLB.Items)
            {
                import.Add(s);
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

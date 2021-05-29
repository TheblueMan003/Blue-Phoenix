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
        Dictionary<string, Compiler.File> filesDic;
        public bool closed = false;

        private void UpdateList(){
            if (files.Count > 0)
            {
                filesDic = files.ToDictionary(x => x.type == "json" ? x.name : ("functions/" + x.name));
                ReloadTree();
                richTextBox1.Text = files[0].content;
            }
        }

        public FilePreview(List<Compiler.File> files)
        {
            InitializeComponent();
            this.files = files;
            UpdateList();
        }

        private void FilePreview_Load(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length < 10000)
            {
                FormatterCommand.reformat(richTextBox1, this, false);
            }
        }

        private void FilePreview_FormClosed(object sender, FormClosedEventArgs e)
        {
            closed = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (files.Count > 0)
            {
                ReloadTree();
            }
        }

        private void ReloadTree()
        {
            treeView1.Nodes.Clear();
            var paths = files.Select(x => x.type == "json"?x.name:("functions/" +x.name)).Where(x=>x.Contains(textBox1.Text)).ToList();
            BuildTree(paths, "", treeView1.Nodes);
        }

        private void BuildTree(List<string> paths, string parent, TreeNodeCollection addInMe, int rec = 0)
        {
            if (rec > 100)
            {
                paths.ForEach(x => addInMe.Add(x));
            }
            else
            {
                paths.Where(x => x.Contains("/"))
                     .Select(x => Compiler.smartSplit(x, '/', 1))
                     .GroupBy(x => x[0]).ToList()
                     .ForEach(x =>
                     {
                         TreeNode curNode = addInMe.Add(x.Key);
                         BuildTree(x.Select(z => z.Last())
                                                 .ToList(),
                                                 parent, curNode.Nodes, rec + 1);

                     });
                paths.Where(x => !x.Contains("/")).ToList().ForEach(x => addInMe.Add(x));
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string fullPath = treeView1.SelectedNode.FullPath;
            int l = 0;
            
            if (filesDic.ContainsKey(fullPath))
            {
                richTextBox1.Text = filesDic[fullPath].content;
            }
        }
    }
}

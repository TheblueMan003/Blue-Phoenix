using Newtonsoft.Json;
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
    public partial class FilePreview : Form
    {
        List<Compiler.File> files;
        Dictionary<string, Compiler.File> filesDic;
        public bool closed = false;

        private Image minusPath;
        private Image plusPath;
        private Image filePath;
        private Image fileCSVPath;
        private Image fileINIPath;
        private Image fileTXTPath;

        private void UpdateList(){
            if (files.Count > 0)
            {
                filesDic = new Dictionary<string, Compiler.File>();
                files.ForEach(x => filesDic[(x.type == "json") ? x.name : ("functions/" + x.name)]=x);
                foreach (string key in Compiler.blockTags.Keys)
                {
                    filesDic["tags/blocks/" + key.Substring(key.IndexOf(".") + 1, key.Length - key.IndexOf(".") - 1)
                        .Replace(".", "/")] = new Compiler.File("",
                            JsonConvert.SerializeObject(Compiler.blockTags[key]));
                }
                foreach (string key in Compiler.entityTags.Keys)
                {
                    filesDic["tags/entity_types/" + key.Substring(key.IndexOf(".") + 1, key.Length - key.IndexOf(".") - 1)
                        .Replace(".", "/")] = new Compiler.File("",
                            JsonConvert.SerializeObject(Compiler.entityTags[key]));
                }
                foreach (string key in Compiler.itemTags.Keys)
                {
                    filesDic["tags/items/" + key.Substring(key.IndexOf(".")+1, key.Length - key.IndexOf(".")-1)
                        .Replace(".", "/")] = new Compiler.File("",
                            JsonConvert.SerializeObject(Compiler.itemTags[key]));
                }
                ReloadTree();
                richTextBox1.Text = files[0].content;
            }
        }

        public FilePreview(List<Compiler.File> files)
        {
            InitializeComponent();
            this.files = files;

            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
            minusPath = Image.FromFile(path + "assets/folder_open.png");
            plusPath = Image.FromFile(path + "assets/folder_closed.png");
            filePath = Image.FromFile(path + "assets/file.png");
            fileCSVPath = Image.FromFile(path + "assets/file_csv.png");
            fileINIPath = Image.FromFile(path + "assets/file_ini.png");
            fileTXTPath = Image.FromFile(path + "assets/file_txt.png");

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
            var paths = filesDic.Select(x => x.Key).Where(x => x.Contains(textBox1.Text)).ToList();
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

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Rectangle nodeRect = e.Node.Bounds;

            /*--------- 1. draw expand/collapse icon ---------*/
            Point ptExpand = new Point(nodeRect.Location.X - 10, nodeRect.Location.Y - 2);
            Image expandImg = null;

            if (e.Node.IsExpanded)
                expandImg = minusPath;
            else
                expandImg = plusPath;

            Graphics g = Graphics.FromImage(expandImg);

            IntPtr imgPtr = g.GetHdc();
            g.ReleaseHdc();
            if (e.Node.Nodes.Count > 0)
            {
                e.Graphics.DrawImage(expandImg, ptExpand);
            }


            /*--------- 2. draw node icon ---------*/
            Point ptNodeIcon = new Point(nodeRect.Location.X - 4, nodeRect.Location.Y - 2);
            Image nodeImg = filePath;
            if (e.Node.FullPath.EndsWith(".csv"))
            {
                nodeImg = fileCSVPath;
            }
            if (e.Node.FullPath.EndsWith(".ini"))
            {
                nodeImg = fileINIPath;
            }
            if (e.Node.FullPath.EndsWith(".txt"))
            {
                nodeImg = fileTXTPath;
            }
            if (e.Node.FullPath.EndsWith(".json"))
            {
                nodeImg = fileINIPath;
            }

            g = Graphics.FromImage(nodeImg);
            imgPtr = g.GetHdc();
            g.ReleaseHdc();
            if (e.Node.Nodes.Count == 0)
            {
                e.Graphics.DrawImage(nodeImg, ptNodeIcon);
            }

            /*--------- 3. draw node text ---------*/
            Font nodeFont = e.Node.NodeFont;
            if (nodeFont == null)
                nodeFont = ((TreeView)sender).Font;
            Brush textBrush = new SolidBrush(Color.White);
            //to highlight the text when selected
            if ((e.State & TreeNodeStates.Focused) != 0)
                textBrush = new SolidBrush(Color.Aqua);
            //Inflate to not be cut
            Rectangle textRect = nodeRect;
            //need to extend node rect
            textRect.Width += 40;
            e.Graphics.DrawString(e.Node.Text, nodeFont, textBrush,
                Rectangle.Inflate(textRect, -12, 0));
        }

        private void CollapseRec(TreeNode node, int rec = 0)
        {
            if (rec < 100)
            {
                foreach (TreeNode n in node.Nodes)
                {
                    CollapseRec(n, rec + 1);
                }
                node.Collapse();
            }
        }
        private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                CollapseRec(e.Node);
            }
        }
        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                e.Node.ExpandAll();
            }
        }
    }
}

using JSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BluePhoenix
{
    public partial class LibImport : Form
    {
        public List<string> import = new List<string>();
        private Image minusPath;
        private Image plusPath;
        private Image filePath;
        private Image fileCSVPath;
        private Image fileINIPath;
        private Image fileTXTPath;
        private Image fileImagePath;

        public LibImport(List<string> paths, List<string> imported = null)
        {
            InitializeComponent();
            
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/";
            minusPath = Image.FromFile(path + "assets/folder_open.png");
            plusPath = Image.FromFile(path + "assets/folder_closed.png");
            filePath = Image.FromFile(path + "assets/file.png");
            fileCSVPath = Image.FromFile(path + "assets/file_csv.png");
            fileINIPath = Image.FromFile(path + "assets/file_ini.png");
            fileTXTPath = Image.FromFile(path + "assets/file_txt.png");
            fileImagePath = Image.FromFile(path + "assets/file_image.png");

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
            List<string> lib = paths.Select(x => (Directory.EnumerateFiles(path + x, "*.tbms",SearchOption.AllDirectories), x))
                                    .Select(tu => tu.Item1.Select(x => x.Replace(path+tu.Item2, "")))
                                    .SelectMany(x => x)
                                    .Select(x => x.Replace("/",".").Replace(".tbms","").Replace("\\","."))
                                    .Distinct()
                                    .ToList();
            
            ReloadTree(lib);
        }

        private void ImportedLB_DoubleClick(object sender, EventArgs e)
        {
            if (ImportedLB.SelectedIndex >= 0)
            {
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

        private void ReloadTree(List<string> paths)
        {
            treeView1.Nodes.Clear();
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
                paths.Where(x => x.Contains("."))
                     .Select(x => Compiler.smartSplit(x, '.', 1))
                     .GroupBy(x => x[0]).ToList()
                     .ForEach(x =>
                     {
                         TreeNode curNode = addInMe.Add(x.Key);
                         BuildTree(x.Select(z => z.Last())
                                                 .ToList(),
                                                 parent, curNode.Nodes, rec + 1);

                     });
                paths.Where(x => !x.Contains(".")).ToList().ForEach(x => addInMe.Add(x));
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                ImportedLB.Items.Add(treeView1.SelectedNode.FullPath);
            }
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Rectangle nodeRect = e.Node.Bounds;

            if (!(nodeRect.Location.X == 0 && nodeRect.Location.Y == 0))
            {
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
        }
    }
}

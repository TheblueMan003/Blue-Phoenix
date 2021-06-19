using Cyotek.Data.Nbt;
using Cyotek.Data.Nbt.Serialization;
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
    public partial class StructureToCMD : Form
    {
        public StructureToCMD()
        {
            InitializeComponent();
        }

        private void StructureToCMD_Load(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog1.FileName;
                
                List<string> pallets = new List<string>();
                string cmds = "";
                BinaryTagReader file = new BinaryTagReader(new FileStream(path, FileMode.Open));
                var doc = file.ReadDocument();
                
                foreach (TagCompound tag in doc.GetList("palette").Value)
                {
                    string name = tag.GetStringValue("Name");
                    if (tag.Contains("Properties"))
                    {
                        name += "[";
                        name += tag.GetCompound("Properties").Value.Select(x => x.Name + "=" + x.GetValue().ToString()).Aggregate((x, y) => x + "," + y);
                        name += "]";
                    }
                    pallets.Add(name);
                }
                foreach (TagCompound tag in doc.GetList("blocks").Value)
                {
                    var pos2 = tag.GetList("pos").Value;
                    var pos = pos2.Select(x => (int)x.GetValue()).ToArray();
                    cmds += $"/setblock ~{pos[0]} ~{pos[1]} ~{pos[2]} {pallets[tag.GetInt("state").Value]}\n";
                }
                CodeBox.Text = cmds;
            }
        }
    }
}

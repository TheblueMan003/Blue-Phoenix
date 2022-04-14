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

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        public static List<Block> Convert(string path)
        {
            List<string> pallets = new List<string>();
            List<Dictionary<string,string>> data = new List<Dictionary<string, string>>();
            BinaryTagReader file = new BinaryTagReader(new FileStream(path, FileMode.Open));
            var doc = file.ReadDocument();

            foreach (TagCompound tag in doc.GetList("palette").Value)
            {
                string name = tag.GetStringValue("Name");
                pallets.Add(name);
                var dic = new Dictionary<string, string>();
                data.Add(dic);

                if (tag.Contains("Properties"))
                {
                    tag.GetCompound("Properties").Value.ToList().ForEach(x => dic.Add(x.Name, x.GetValue().ToString()));
                }
            }
            List<Block> lst = new List<Block>();
            foreach (TagCompound tag in doc.GetList("blocks").Value)
            {
                var pos2 = tag.GetList("pos").Value;
                var pos = pos2.Select(x => (int)x.GetValue()).ToArray();
                int state = tag.GetInt("state").Value;
                Block b = new Block(pos[0], pos[1], pos[2], pallets[state], data[state]);
                lst.Add(b);
            }
            return lst;
        }
        public class Block
        {
            public int x,y,z;
            public string block_id;
            public Dictionary<string, string> block_data;
            public string block_data_dict;
            public string block_data_mc;

            public Block(int x, int y, int z, string block_id, Dictionary<string, string> block_data)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.block_id = block_id;
                this.block_data = block_data;
                if (block_data.Count > 0)
                {
                    this.block_data_mc = $"[{block_data.Select(kv => kv.Key + "=" + kv.Value).Aggregate((a, b) => a + "," + b)}]";
                    this.block_data_dict = "{" + $"{block_data.Select(kv => kv.Key + ":" + kv.Value).Aggregate((a, b) => a + "," + b)}" + "}";
                }
                else
                {
                    this.block_data_mc = "[]";
                    this.block_data_dict = "{}";
                }
            }
        }
    }
}

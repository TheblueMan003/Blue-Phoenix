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

namespace BluePhoenix
{
    public partial class TilemapSetting : Form
    {
        private string path;
        public Tilemap Tilemap { get; private set; }
        public TilemapSetting(string path)
        {
            this.path = path;
            InitializeComponent();
        }
        public TilemapSetting(Tilemap t, string path)
        {
            this.path = path; 
            Tilemap = t;
            CB_Tilemap.Text = t.Name;
            InitializeComponent();
        }

        private void TilemapSetting_Load(object sender, EventArgs e)
        {
            Reload();
        }
        public void Reload()
        {
            CB_Tilemap.Items.Clear();
            CB_Tilemap.Items.AddRange(Directory.GetDirectories(path, "*",SearchOption.TopDirectoryOnly));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(path + "/" + TB_Tilemap_Name.Text);
            Reload();
            CB_Tilemap.Text = TB_Tilemap_Name.Text;
            TB_Tilemap_Name.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TB_Tileset.Text = openFileDialog1.FileName;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TB_Object.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Tilemap tilemap;
            if (Tilemap == null)
            {
                tilemap = new Tilemap(CB_Tilemap.Text,
                                    int.Parse(TB_Width.Text),
                                    int.Parse(TB_Height.Text),
                                    int.Parse(TB_TileWidth.Text),
                                    int.Parse(TB_Tile_Height.Text));
            }
            else
            {
                tilemap = new Tilemap(Tilemap,
                                    int.Parse(TB_Width.Text),
                                    int.Parse(TB_Height.Text),
                                    int.Parse(TB_TileWidth.Text),
                                    int.Parse(TB_Tile_Height.Text));
            }

            // Save everything
            if (File.Exists(TB_Tileset.Text))
                File.Copy(TB_Tileset.Text, path + "/" + CB_Tilemap.Text + "/tileset_terrain.png");
            if (File.Exists(TB_Object.Text))
                File.Copy(TB_Object.Text, path + "/" + CB_Tilemap.Text + "/tileset_object.png");
            File.WriteAllText(path + "/" + CB_Tilemap.Text + "/tilemap.json",
                                JsonConvert.SerializeObject(tilemap));

            DialogResult = DialogResult.OK;
            Tilemap = tilemap;
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CB_Tilemap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CB_Tilemap.SelectedIndex >= 0 && File.Exists(path + "/"+CB_Tilemap.SelectedItem+ "/tilemap.json"))
            {
                string f = File.ReadAllText(path + "/" + CB_Tilemap.SelectedItem + "/tilemap.json");
                Tilemap t = JsonConvert.DeserializeObject<Tilemap>(f);
                Tilemap = t;

                TB_Width.Text = t.Width.ToString();
                TB_Height.Text = t.Height.ToString();
                TB_TileWidth.Text = t.TileW.ToString();
                TB_Tile_Height.Text = t.TileH.ToString();
            }
        }
    }
}

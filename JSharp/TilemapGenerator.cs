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
    public partial class TilemapGenerator : Form
    {
        Tilemap tilemap;
        string path;
        Image Tileset_Terrain;
        Image Tileset_Object;

        int SelectedTileTerrainX;
        int SelectedTileTerrainY;

        int SelectedTileObjectX;
        int SelectedTileObjectY;
        Graphics g;

        int SelectionStartX = -1;
        int SelectionStartY = -1;

        public TilemapGenerator(string path)
        {
            this.path = path;
            InitializeComponent();
            g = panel1.CreateGraphics();
        }

        private void TilemapGenerator_Load(object sender, EventArgs e)
        {
            TilemapSetting setting = new TilemapSetting(path);
            if (setting.ShowDialog() == DialogResult.OK)
            {
                tilemap = setting.Tilemap;
                Reload();
            }
        }

        private void Reload()
        {
            if (File.Exists(path + "/" + tilemap.Name + "/tileset_terrain.png"))
                Tileset_Terrain = Image.FromFile(path + "/" + tilemap.Name + "/tileset_terrain.png");

            if (File.Exists(path + "/" + tilemap.Name + "/tileset_object.png"))
                Tileset_Object = Image.FromFile(path + "/" + tilemap.Name + "/tileset_object.png");

            panel1.Invalidate();
            panel2.Invalidate();
            panel3.Invalidate();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.FromArgb(30, 30, 30));
            if (Tileset_Object != null)
                g.DrawImage(Tileset_Object, 0, 0);
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.FromArgb(30,30,30));
            if (Tileset_Terrain != null)
                g.DrawImage(Tileset_Terrain, 0, 0);
            g.DrawRectangle(new Pen(Color.White, 2),
                new Rectangle(SelectedTileTerrainX * tilemap.TileW, SelectedTileTerrainY * tilemap.TileH,
                             tilemap.TileW, tilemap.TileH));
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            var coordinates = panel3.PointToClient(Cursor.Position);
            SelectedTileTerrainX = coordinates.X / tilemap.TileW;
            SelectedTileTerrainY = coordinates.Y / tilemap.TileH;
            panel3.Invalidate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            g.Clear(Color.FromArgb(30, 30, 30));
                
            if (Tileset_Terrain != null)
            {
                for (int i = 0; i < tilemap.Width; i++)
                {
                    for (int j = 0; j < tilemap.Height; j++)
                    {
                        DrawTile(i, j, tilemap.Cells[i, j].Tileset_X, tilemap.Cells[i, j].Tileset_Y);
                    }
                }
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            var coordinates = panel1.PointToClient(Cursor.Position);
            SelectionStartX = coordinates.X / tilemap.TileW;
            SelectionStartY = coordinates.Y / tilemap.TileH;
        }
        private void DrawTile(int x, int y, int cx, int cy)
        {
            tilemap.Cells[x, y].Tileset_X = cx;
            tilemap.Cells[x, y].Tileset_X = cy;
            g.DrawImage(Tileset_Terrain, new Rectangle(x * tilemap.TileW, y * tilemap.TileH, tilemap.TileW, tilemap.TileH),
                                cx * tilemap.TileW, cy * tilemap.TileH, tilemap.TileW, tilemap.TileH,
                                GraphicsUnit.Pixel);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var coordinates = panel1.PointToClient(Cursor.Position);
                int i = coordinates.X / tilemap.TileW;
                int j = coordinates.Y / tilemap.TileH;
                DrawTile(i, j, SelectedTileTerrainX, SelectedTileTerrainY);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Shift)
            {
                var coordinates = panel1.PointToClient(Cursor.Position);
                int i = coordinates.X / tilemap.TileW;
                int j = coordinates.Y / tilemap.TileH;
                for (int x = SelectionStartX; x <= i; x++)
                {
                    for (int y = SelectionStartY; y <= j; y++)
                    {
                        DrawTile(x, y, SelectedTileTerrainX, SelectedTileTerrainY);
                    }
                }
            }
        }
    }
}

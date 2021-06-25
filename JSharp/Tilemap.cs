using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluePhoenix
{
    public class Tilemap
    {
        public string Name;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Cell[,] Cells;
        public int[,] Objects;
        public List<Object> ObjectsTypes;
        public int TileW { get; private set; }
        public int TileH { get; private set; }

        public Tilemap(string name, int w, int h, int tilew, int tileh)
        {
            Name = name;
            Width = w;
            Height = h;
            TileW = tilew;
            TileH = tileh;

            Cells = new Cell[w,h];
            Objects = new int[w, h];
            ObjectsTypes = new List<Object>();
        }
        public Tilemap(Tilemap tilemap, int w, int h, int tilew, int tileh)
        {
            Name = tilemap.Name;
            Width = w;
            Height = h;
            TileW = tilew;
            TileH = tileh;

            Cells = new Cell[w, h];
            Objects = new int[w, h];
            ObjectsTypes = tilemap.ObjectsTypes.ToList();

            for (int i = 0; i < Math.Min(tilemap.Width, Width); i++)
            {
                for (int j = 0; j < Math.Min(tilemap.Height, Height); j++)
                {
                    Cells[i,j] = tilemap.Cells[i, j];
                    Objects[i, j] = tilemap.Objects[i, j];
                }
            }
        }

        public struct Cell
        {
            public int Tileset_X;
            public int Tileset_Y;
        }
        public class Object
        {
            public string cmd;
            public int Tileset_X;
            public int Tileset_Y;
        }
    }
}
